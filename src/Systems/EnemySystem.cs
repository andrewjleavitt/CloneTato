using System.Numerics;
using CloneTato.Core;
using CloneTato.Data;
using CloneTato.Entities;

namespace CloneTato.Systems;

public static class EnemySystem
{
    // Auto-fire weapons that enemies can pick up (indices into WeaponDatabase)
    // Only Auto weapons make sense for enemies
    private static readonly int[] EnemyWeaponPool = GetAutoWeaponIndices();

    private static int[] GetAutoWeaponIndices()
    {
        var indices = new List<int>();
        for (int i = 0; i < WeaponDatabase.Weapons.Length; i++)
            if (WeaponDatabase.Weapons[i].Type == WeaponType.Auto)
                indices.Add(i);
        return indices.ToArray();
    }

    public static void Update(float dt, GameState state)
    {
        var playerPos = state.Player.Position;
        float time = (float)Raylib_cs.Raylib.GetTime();

        for (int i = 0; i < state.Enemies.Count; i++)
        {
            var enemy = state.Enemies[i];
            if (!enemy.Active) continue;

            // Death animation
            if (enemy.IsDying)
            {
                enemy.DeathTimer -= dt;
                if (enemy.DeathTimer <= 0)
                    enemy.Active = false;
                continue;
            }

            // Walk animation
            enemy.AnimTimer += dt;

            // Knockback
            if (enemy.KnockbackTimer > 0)
            {
                enemy.KnockbackTimer -= dt;
                enemy.Position += enemy.KnockbackVelocity * dt;
                enemy.KnockbackVelocity *= 0.85f;
                continue;
            }

            Vector2 dir = playerPos - enemy.Position;
            float dist = dir.Length();

            // Separation force — push apart from nearby enemies to prevent clumping
            Vector2 separation = Vector2.Zero;
            const float separationRadius = 30f;
            const float separationForce = 80f;
            for (int j = 0; j < state.Enemies.Count; j++)
            {
                if (i == j) continue;
                var other = state.Enemies[j];
                if (!other.Active || other.IsDying) continue;
                Vector2 diff = enemy.Position - other.Position;
                float d = diff.Length();
                if (d > 0.1f && d < separationRadius)
                {
                    // Stronger push the closer they are
                    separation += (diff / d) * (1f - d / separationRadius);
                }
            }

            // Loot enemy despawn timer
            if (enemy.IsLootEnemy)
            {
                enemy.FleeTimer -= dt;
                // Flash when about to despawn (last 3 seconds)
                if (enemy.FleeTimer < 3f)
                {
                    float flashRate = MathF.Max(0.08f, enemy.FleeTimer * 0.1f);
                    if ((int)(enemy.AnimTimer / flashRate) % 3 == 0)
                        enemy.FlashTimer = 0.04f;
                }
                if (enemy.FleeTimer <= 0)
                {
                    enemy.Active = false; // despawn — no loot, no death, just gone
                    continue;
                }
            }

            if (dist > 1f)
            {
                dir /= dist;

                // Flee behavior — run away from player with erratic dodging
                if (enemy.Behavior == EnemyBehavior.Flee)
                {
                    // Base: run directly away from player
                    Vector2 fleeDir = -dir;
                    // Add erratic swerving to make it harder to track
                    Vector2 perp = new(-fleeDir.Y, fleeDir.X);
                    float swerve = MathF.Sin(time * 5f + enemy.SineOffset) * 0.5f;
                    fleeDir = Vector2.Normalize(fleeDir + perp * swerve);

                    // Bounce off arena edges — steer toward center when near walls
                    float margin = 60f;
                    Vector2 center = new(Constants.ArenaWidth * 0.5f, Constants.ArenaHeight * 0.5f);
                    if (enemy.Position.X < margin || enemy.Position.X > Constants.ArenaWidth - margin
                        || enemy.Position.Y < margin || enemy.Position.Y > Constants.ArenaHeight - margin)
                    {
                        Vector2 toCenter = Vector2.Normalize(center - enemy.Position);
                        fleeDir = Vector2.Normalize(fleeDir + toCenter * 1.5f);
                    }

                    enemy.Velocity = fleeDir * enemy.Speed;
                }
                // Armed enemies hang back at preferred range
                else if (enemy.IsArmed)
                {
                    if (dist < enemy.PreferredRange * 0.7f)
                        enemy.Velocity = -dir * enemy.Speed; // back away
                    else if (dist > enemy.PreferredRange * 1.3f)
                        enemy.Velocity = dir * enemy.Speed; // close in
                    else
                    {
                        // Strafe at range — each enemy orbits in its own direction
                        Vector2 perp = new(-dir.Y, dir.X);
                        float sine = MathF.Sin(time * 2f + enemy.SineOffset);
                        enemy.Velocity = perp * sine * enemy.Speed * 0.7f;
                    }
                }
                // Unarmed: use original behavior with flanking
                else if (enemy.Behavior == EnemyBehavior.Erratic)
                {
                    float sine = MathF.Sin(time * 4f + enemy.SineOffset) * 0.6f;
                    Vector2 perp = new(-dir.Y, dir.X);
                    var erraticDir = dir + perp * sine;
                    enemy.Velocity = Vector2.Normalize(erraticDir) * enemy.Speed;
                }
                else
                {
                    // Chase/FastChase/Tank — offset approach angle for flanking
                    // Each enemy uses SineOffset to pick a unique angle offset
                    float flankAngle = MathF.Sin(enemy.SineOffset) * 0.6f; // ±0.6 radians (~34°)
                    float baseAngle = MathF.Atan2(dir.Y, dir.X) + flankAngle;

                    // When close to player, commit to direct approach
                    float flankBlend = Math.Clamp((dist - 40f) / 80f, 0f, 1f);
                    var flankDir = new Vector2(MathF.Cos(baseAngle), MathF.Sin(baseAngle));
                    var finalDir = Vector2.Normalize(Vector2.Lerp(dir, flankDir, flankBlend));

                    enemy.Velocity = finalDir * enemy.Speed;
                }
            }

            // Apply separation force
            if (separation != Vector2.Zero)
                enemy.Velocity += separation * separationForce;

            // Armed enemies shoot
            if (enemy.IsArmed)
            {
                enemy.ShootTimer -= dt;
                if (enemy.ShootTimer <= 0 && dist < enemy.PreferredRange * 2.5f)
                {
                    enemy.ShootTimer = enemy.ShootCooldown;
                    // Play attack animation
                    enemy.IsAttacking = true;
                    enemy.AttackAnimTimer = enemy.AttackAnimDuration > 0 ? enemy.AttackAnimDuration : 0.35f;

                    Vector2 baseDir = Vector2.Normalize(playerPos - enemy.Position);
                    float baseAngle = MathF.Atan2(baseDir.Y, baseDir.X);

                    for (int s = 0; s < enemy.ProjectileCount; s++)
                    {
                        var proj = state.GetInactiveEnemyProjectile();
                        if (proj == null) break;

                        float spreadAngle;
                        if (enemy.ProjectileCount > 1)
                        {
                            // Fan spread: evenly distribute across spread arc
                            float t = (float)s / (enemy.ProjectileCount - 1) - 0.5f;
                            spreadAngle = baseAngle + t * enemy.ProjectileSpread;
                        }
                        else
                        {
                            // Single shot with small random spread
                            spreadAngle = baseAngle + (Random.Shared.NextSingle() - 0.5f) * 0.15f;
                        }

                        var shotDir = new Vector2(MathF.Cos(spreadAngle), MathF.Sin(spreadAngle));
                        proj.Init(
                            enemy.Position + shotDir * (enemy.Radius + 4f),
                            shotDir * enemy.ProjectileSpeed,
                            enemy.ProjectileDamage,
                            2.5f, 0,
                            Raylib_cs.Color.Orange
                        );
                    }
                }
                // Tick attack animation
                if (enemy.IsAttacking)
                {
                    enemy.AttackAnimTimer -= dt;
                    if (enemy.AttackAnimTimer <= 0)
                        enemy.IsAttacking = false;
                }
            }

            // Boss melee attack
            if (enemy.HasMeleeAttack)
            {
                if (enemy.IsAttacking)
                {
                    enemy.AttackAnimTimer -= dt;
                    enemy.Velocity *= 0.3f; // slow down during swing

                    // Deal damage at the midpoint of the swing
                    if (!enemy.MeleeAttackHit && enemy.AttackAnimTimer < enemy.AttackAnimDuration * 0.5f)
                    {
                        enemy.MeleeAttackHit = true;
                        if (dist < enemy.MeleeAttackRange && state.Player.InvincibilityTimer <= 0)
                        {
                            int dmg = Math.Max(1, enemy.MeleeAttackDamage - state.Player.ComputedStats.Armor);
                            state.Player.CurrentHP -= dmg;
                            state.Player.InvincibilityTimer = Constants.PlayerInvincibilityTime;
                            state.Player.FlashTimer = 0.15f;

                            // Player knockback from melee attack
                            Vector2 knockDir = Vector2.Normalize(state.Player.Position - enemy.Position);
                            float knockForce = enemy.IsBoss ? 350f : 200f;
                            state.Player.KnockbackVelocity = knockDir * knockForce;
                            state.Player.KnockbackTimer = enemy.IsBoss ? 0.2f : 0.12f;

                            var dmgNum = state.GetInactiveDamageNumber();
                            dmgNum?.Init(state.Player.Position, dmg.ToString(), Raylib_cs.Color.Red);
                            state.RequestScreenShake(0.1f, enemy.IsBoss ? 3.0f : 1.8f);
                            state.RequestHitstop(enemy.IsBoss ? 0.06f : 0.03f);
                            state.Assets.PlaySoundVariant("hurt", 0.5f);
                        }
                    }

                    if (enemy.AttackAnimTimer <= 0)
                        enemy.IsAttacking = false;
                }
                else
                {
                    enemy.MeleeAttackTimer -= dt;
                    if (enemy.MeleeAttackTimer <= 0 && dist < enemy.MeleeAttackRange)
                    {
                        enemy.IsAttacking = true;
                        enemy.AttackAnimTimer = enemy.AttackAnimDuration;
                        enemy.MeleeAttackTimer = enemy.MeleeAttackCooldown;
                        enemy.MeleeAttackHit = false;
                    }
                    // Telegraph: slow down and flash when about to attack (within 0.2s of cooldown ending)
                    else if (enemy.MeleeAttackTimer < 0.2f && dist < enemy.MeleeAttackRange * 1.3f)
                    {
                        enemy.Velocity *= 0.15f; // near-stop during wind-up
                        enemy.FlashTimer = 0.03f; // brief white flash as telegraph
                    }
                }
            }

            // Kamikaze fuse countdown
            if (enemy.IsKamikaze)
            {
                enemy.FuseTimer -= dt;

                // Accelerating flash — pulses faster as fuse runs down
                float fuseProgress = 1f - (enemy.FuseTimer / enemy.FuseDuration);
                float flashInterval = MathF.Max(0.05f, 0.4f - fuseProgress * 0.35f);
                if ((int)(enemy.AnimTimer / flashInterval) % 2 == 0)
                    enemy.FlashTimer = 0.05f;

                // Speed boost in final second — frantic rush
                if (enemy.FuseTimer < 1f)
                    enemy.Velocity *= 1.4f;

                // Explode when fuse runs out
                if (enemy.FuseTimer <= 0)
                {
                    Explode(enemy, state);
                    continue;
                }
            }

            // Tick attack animation for non-boss, non-armed enemies (contact attack visual)
            if (enemy.IsAttacking && !enemy.HasMeleeAttack && !enemy.IsArmed)
            {
                enemy.AttackAnimTimer -= dt;
                if (enemy.AttackAnimTimer <= 0)
                    enemy.IsAttacking = false;
            }

            // Terrain zone speed modifier for enemies too
            float terrainMult = CollisionSystem.GetTerrainSpeedMultiplier(state, enemy.Position);
            if (terrainMult < 1f)
                enemy.Velocity *= terrainMult;

            enemy.Position += enemy.Velocity * dt;

            // Clamp to arena + obstacle collision
            enemy.Position.X = Math.Clamp(enemy.Position.X, -20f, Constants.ArenaWidth + 20f);
            enemy.Position.Y = Math.Clamp(enemy.Position.Y, -20f, Constants.ArenaHeight + 20f);
            CollisionSystem.ResolveObstacleCollision(state, ref enemy.Position, enemy.Radius);

            // Ooze damage to enemies
            float oozeDmg = CollisionSystem.GetTerrainDamageRate(state, enemy.Position);
            if (oozeDmg > 0)
            {
                int dmg = Math.Max(1, (int)(oozeDmg * dt + 0.5f));
                enemy.CurrentHP -= dmg;
                enemy.FlashTimer = 0.08f;
                if (enemy.CurrentHP <= 0 && !enemy.IsDying)
                    state.HandleEnemyDeath(enemy);
            }

            if (enemy.FlashTimer > 0) enemy.FlashTimer -= dt;
        }
    }

    public static void SpawnEnemy(GameState state, int waveNumber)
    {
        var config = state.CurrentWaveConfig;
        if (config == null) return;

        var enemy = state.GetInactiveEnemy();
        if (enemy == null) return;

        // Pick enemy type using spawn weights if available, otherwise uniform random
        int typeIndex;
        if (config.SpawnWeights != null && config.SpawnWeights.Length == config.EnemyTypeIndices.Length)
        {
            float roll = Random.Shared.NextSingle();
            float cumulative = 0f;
            typeIndex = config.EnemyTypeIndices[^1]; // fallback to last
            for (int i = 0; i < config.SpawnWeights.Length; i++)
            {
                cumulative += config.SpawnWeights[i];
                if (roll <= cumulative)
                {
                    typeIndex = config.EnemyTypeIndices[i];
                    break;
                }
            }
        }
        else
        {
            typeIndex = config.EnemyTypeIndices[Random.Shared.Next(config.EnemyTypeIndices.Length)];
        }
        var def = EnemyDatabase.Enemies[typeIndex];

        // Scale stats based on wave (+8% per wave)
        float scaleFactor = 1f + (waveNumber - 1) * 0.08f;

        // Spawn from random arena edge
        Vector2 spawnPos = GetEdgeSpawnPosition(state.Player.Position);

        // Boss spawning — boss wave spawns the boss as the first enemy
        bool spawnBoss = config.IsBossWave && state.EnemiesSpawnedThisWave == 0;

        // Mini-bosses starting wave 7, every 15th enemy
        if (!spawnBoss && waveNumber >= 7 && state.EnemiesSpawnedThisWave > 0
            && state.EnemiesSpawnedThisWave % 15 == 0)
        {
            spawnBoss = true;
            scaleFactor *= 2f;
        }

        if (spawnBoss)
        {
            scaleFactor *= 4f;
            enemy.InitAsBoss(def, spawnPos, scaleFactor);
            enemy.DefIndex = typeIndex;
            // Boss sprite per biome: Dust Warrior (1), Blowfish (2), Tarnished Widow (3)
            enemy.BossSpriteType = state.CurrentBiome switch
            {
                1 => 0, // Dust Warrior
                2 => 1, // Blowfish
                3 => 2, // Tarnished Widow
                _ => 0,
            };
        }
        else
        {
            enemy.Init(def, spawnPos, scaleFactor);
            enemy.DefIndex = typeIndex;

            // Chance to arm with a weapon — only for bump-type enemies, starts at wave 4
            if (!enemy.IsArmed && !enemy.HasMeleeAttack && !enemy.IsKamikaze
                && waveNumber >= 4 && EnemyWeaponPool.Length > 0)
            {
                // 10% at wave 4, up to ~50% by wave 15+
                float armChance = Math.Clamp((waveNumber - 3) * 0.04f, 0f, 0.5f);
                if (Random.Shared.NextSingle() < armChance)
                {
                    int weapIdx = EnemyWeaponPool[Random.Shared.Next(EnemyWeaponPool.Length)];
                    enemy.ArmWithWeapon(WeaponDatabase.Weapons[weapIdx], scaleFactor);
                }
            }
        }

        state.EnemiesSpawnedThisWave++;
    }

    public static void Explode(Enemy enemy, GameState state)
    {
        var pos = enemy.Position;
        float radius = enemy.ExplosionRadius;
        int damage = enemy.ExplosionDamage;

        // Kill self
        state.HandleEnemyDeath(enemy);

        // Damage player if in blast radius
        float playerDist = Vector2.Distance(pos, state.Player.Position);
        if (playerDist < radius && state.Player.InvincibilityTimer <= 0)
        {
            // Damage falloff: full at center, 40% at edge
            float falloff = 1f - (playerDist / radius) * 0.6f;
            int dmg = Math.Max(1, (int)(damage * falloff) - state.Player.ComputedStats.Armor);
            state.Player.CurrentHP -= dmg;
            state.Player.InvincibilityTimer = Constants.PlayerInvincibilityTime;
            state.Player.FlashTimer = 0.15f;
            var dmgNum = state.GetInactiveDamageNumber();
            dmgNum?.Init(state.Player.Position, dmg.ToString(), Raylib_cs.Color.Red);
            state.Assets.PlaySoundVariant("hurt", 0.6f);
        }

        // Chain-detonate nearby kamikaze enemies
        for (int j = 0; j < state.Enemies.Count; j++)
        {
            var other = state.Enemies[j];
            if (!other.Active || other.IsDying || other == enemy) continue;

            float d = Vector2.Distance(pos, other.Position);
            if (d < radius)
            {
                // Damage all enemies in blast
                int edm = Math.Max(1, (int)(damage * 0.5f));
                other.CurrentHP -= edm;
                other.FlashTimer = 0.1f;

                // Chain-detonate other kamikaze enemies
                if (other.IsKamikaze && other.FuseTimer > 0.2f)
                    other.FuseTimer = 0.2f; // force near-immediate detonation

                if (other.CurrentHP <= 0 && !other.IsDying)
                    state.HandleEnemyDeath(other);
            }
        }

        // Screen shake
        state.Assets.PlaySoundVariant("explosion", 0.5f);
    }

    private static Vector2 GetEdgeSpawnPosition(Vector2 playerPos)
    {
        int edge = Random.Shared.Next(4);
        float margin = 30f;
        return edge switch
        {
            0 => new Vector2(Random.Shared.NextSingle() * Constants.ArenaWidth, -margin),
            1 => new Vector2(Random.Shared.NextSingle() * Constants.ArenaWidth, Constants.ArenaHeight + margin),
            2 => new Vector2(-margin, Random.Shared.NextSingle() * Constants.ArenaHeight),
            _ => new Vector2(Constants.ArenaWidth + margin, Random.Shared.NextSingle() * Constants.ArenaHeight),
        };
    }
}
