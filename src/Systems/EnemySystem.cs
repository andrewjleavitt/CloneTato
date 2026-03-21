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

            if (dist > 1f)
            {
                dir /= dist;

                // Armed enemies hang back at preferred range
                if (enemy.IsArmed)
                {
                    if (dist < enemy.PreferredRange * 0.7f)
                        enemy.Velocity = -dir * enemy.Speed; // back away
                    else if (dist > enemy.PreferredRange * 1.3f)
                        enemy.Velocity = dir * enemy.Speed; // close in
                    else
                    {
                        // Strafe at range
                        Vector2 perp = new(-dir.Y, dir.X);
                        float sine = MathF.Sin(time * 2f + enemy.SineOffset);
                        enemy.Velocity = perp * sine * enemy.Speed * 0.7f;
                    }
                }
                // Unarmed: use original behavior
                else if (enemy.Behavior == EnemyBehavior.Erratic)
                {
                    float sine = MathF.Sin(time * 4f + enemy.SineOffset) * 0.6f;
                    Vector2 perp = new(-dir.Y, dir.X);
                    var erraticDir = dir + perp * sine;
                    enemy.Velocity = Vector2.Normalize(erraticDir) * enemy.Speed;
                }
                else
                {
                    enemy.Velocity = dir * enemy.Speed;
                }
            }

            // Armed enemies shoot
            if (enemy.IsArmed)
            {
                enemy.ShootTimer -= dt;
                if (enemy.ShootTimer <= 0 && dist < enemy.PreferredRange * 2.5f)
                {
                    enemy.ShootTimer = enemy.ShootCooldown;
                    // Play attack animation
                    enemy.IsAttacking = true;
                    enemy.AttackAnimTimer = 0.35f;
                    enemy.AttackAnimDuration = 0.35f;

                    var proj = state.GetInactiveEnemyProjectile();
                    if (proj != null)
                    {
                        Vector2 shotDir = Vector2.Normalize(playerPos - enemy.Position);
                        float spread = (Random.Shared.NextSingle() - 0.5f) * 0.2f;
                        float angle = MathF.Atan2(shotDir.Y, shotDir.X) + spread;
                        shotDir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));

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
                            var dmgNum = state.GetInactiveDamageNumber();
                            dmgNum?.Init(state.Player.Position, dmg.ToString(), Raylib_cs.Color.Red);
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

        // Pick random enemy type from this wave's allowed types
        int typeIndex = config.EnemyTypeIndices[Random.Shared.Next(config.EnemyTypeIndices.Length)];
        var def = EnemyDatabase.Enemies[typeIndex];

        // Scale stats based on wave
        float scaleFactor = 1f + (waveNumber - 1) * 0.12f;

        // Spawn from random arena edge
        Vector2 spawnPos = GetEdgeSpawnPosition(state.Player.Position);

        // Boss spawning
        bool spawnBoss = waveNumber >= 3 && config.IsBossWave
            && state.EnemiesSpawnedThisWave == config.TotalEnemies - 1;

        // Mini-bosses starting wave 5
        if (!spawnBoss && waveNumber >= 5 && state.EnemiesSpawnedThisWave > 0
            && state.EnemiesSpawnedThisWave % 20 == 0)
        {
            spawnBoss = true;
            scaleFactor *= 2f;
        }

        if (spawnBoss)
        {
            scaleFactor *= 4f;
            enemy.InitAsBoss(def, spawnPos, scaleFactor);
            enemy.DefIndex = typeIndex;
            // Cycle boss sprites: Dust Warrior, Blowfish, Tarnished Widow
            enemy.BossSpriteType = waveNumber switch
            {
                <= 5 => 0,                          // Dust Warrior early
                <= 9 => waveNumber % 2 == 0 ? 1 : 0, // Alternate Dust Warrior / Blowfish
                _ => waveNumber % 3 switch           // Cycle all three late game
                {
                    0 => 0, // Dust Warrior
                    1 => 1, // Blowfish
                    _ => 2, // Tarnished Widow
                },
            };
        }
        else
        {
            enemy.Init(def, spawnPos, scaleFactor);
            enemy.DefIndex = typeIndex;

            // Chance to arm with a weapon — starts at wave 4, increases over time
            if (waveNumber >= 4 && EnemyWeaponPool.Length > 0)
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
