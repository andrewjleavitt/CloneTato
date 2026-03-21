using System.Numerics;
using CloneTato.Entities;
using CloneTato.Systems;
using Raylib_cs;

namespace CloneTato.Core;

public static class CollisionSystem
{
    public static bool CircleOverlap(Vector2 a, float ra, Vector2 b, float rb)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        float distSq = dx * dx + dy * dy;
        float rSum = ra + rb;
        return distSq <= rSum * rSum;
    }

    /// <summary>
    /// Push an entity out of any overlapping obstacles. Returns true if any collision occurred.
    /// </summary>
    public static bool ResolveObstacleCollision(GameState state, ref Vector2 position, float radius)
    {
        bool hit = false;

        // Static obstacles
        for (int i = 0; i < state.Obstacles.Count; i++)
        {
            var obs = state.Obstacles[i];
            if (!obs.Active) continue;
            hit |= PushOut(ref position, radius, obs.Position, obs.Radius);
        }

        // Barrels also block movement
        for (int i = 0; i < state.Barrels.Count; i++)
        {
            var barrel = state.Barrels[i];
            if (!barrel.Active) continue;
            hit |= PushOut(ref position, radius, barrel.Position, barrel.Radius);
        }

        return hit;
    }

    private static bool PushOut(ref Vector2 position, float radius, Vector2 obstaclePos, float obstacleRadius)
    {
        float dx = position.X - obstaclePos.X;
        float dy = position.Y - obstaclePos.Y;
        float distSq = dx * dx + dy * dy;
        float minDist = radius + obstacleRadius;

        if (distSq < minDist * minDist && distSq > 0.001f)
        {
            float dist = MathF.Sqrt(distSq);
            float overlap = minDist - dist;
            position.X += (dx / dist) * overlap;
            position.Y += (dy / dist) * overlap;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Returns the terrain speed multiplier at a given position (1.0 = normal).
    /// </summary>
    public static float GetTerrainSpeedMultiplier(GameState state, Vector2 position)
    {
        float mult = 1f;
        for (int i = 0; i < state.TerrainZones.Count; i++)
        {
            var zone = state.TerrainZones[i];
            if (!zone.Active) continue;
            if (Vector2.Distance(position, zone.Position) < zone.Radius)
            {
                if (zone.SpeedMultiplier < mult)
                    mult = zone.SpeedMultiplier;
            }
        }
        return mult;
    }

    /// <summary>
    /// Returns heal-per-second at a given position (0 = no healing).
    /// </summary>
    public static float GetTerrainHealRate(GameState state, Vector2 position)
    {
        float heal = 0f;
        for (int i = 0; i < state.TerrainZones.Count; i++)
        {
            var zone = state.TerrainZones[i];
            if (!zone.Active) continue;
            if (Vector2.Distance(position, zone.Position) < zone.Radius)
                heal += zone.HealPerSecond;
        }
        return heal;
    }

    /// <summary>
    /// Returns damage-per-second at a given position (0 = safe).
    /// </summary>
    public static float GetTerrainDamageRate(GameState state, Vector2 position)
    {
        float dmg = 0f;
        for (int i = 0; i < state.TerrainZones.Count; i++)
        {
            var zone = state.TerrainZones[i];
            if (!zone.Active) continue;
            if (zone.DamagePerSecond > 0 && Vector2.Distance(position, zone.Position) < zone.Radius)
                dmg += zone.DamagePerSecond;
        }
        return dmg;
    }

    /// <summary>
    /// Tick temporary terrain zone durations. Call once per frame.
    /// </summary>
    public static void UpdateTerrainZones(float dt, GameState state)
    {
        for (int i = state.TerrainZones.Count - 1; i >= 0; i--)
        {
            var zone = state.TerrainZones[i];
            if (!zone.Active) continue;
            if (zone.Duration > 0)
            {
                zone.Duration -= dt;
                if (zone.Duration <= 0)
                {
                    state.TerrainZones.RemoveAt(i);
                }
            }
        }
    }

    private static void DestroyBarrel(GameState state, Barrel barrel)
    {
        barrel.Active = false;

        if (barrel.Type == BarrelType.Explosive)
        {
            // Big explosion — damages enemies AND player
            WeaponSystem.ExplodeAt(state, barrel.Position, 60, 50f);

            // Also damage player if in range
            float distToPlayer = Vector2.Distance(barrel.Position, state.Player.Position);
            if (distToPlayer < 50f && state.Player.InvincibilityTimer <= 0)
            {
                float falloff = 1f - distToPlayer / 50f * 0.5f;
                int dmg = Math.Max(1, (int)(40 * falloff) - state.Player.ComputedStats.Armor);
                state.Player.CurrentHP -= dmg;
                state.Player.InvincibilityTimer = Constants.PlayerInvincibilityTime;
                state.Player.FlashTimer = 0.15f;
                var dmgNum = state.GetInactiveDamageNumber();
                dmgNum?.Init(state.Player.Position, dmg.ToString(), Color.Red);
            }

            state.Assets.PlaySoundVariant("explosion", 0.6f);
        }
        else // Toxic
        {
            // Spawn ooze pool
            state.TerrainZones.Add(new TerrainZone
            {
                Position = barrel.Position,
                Radius = 35f,
                Type = TerrainType.Ooze,
                Active = true,
                Duration = 8f,
                DamagePerSecond = 12f,
            });
            state.Assets.PlaySoundVariant("hurt", 0.4f);
        }
    }

    public static void ProcessCollisions(GameState state)
    {
        var player = state.Player;

        // Projectile vs Enemy
        for (int p = 0; p < state.Projectiles.Count; p++)
        {
            var proj = state.Projectiles[p];
            if (!proj.Active) continue;

            for (int e = 0; e < state.Enemies.Count; e++)
            {
                var enemy = state.Enemies[e];
                if (!enemy.Active) continue;

                if (enemy.IsDying) continue;
                if (CircleOverlap(proj.Position, proj.Radius, enemy.Position, enemy.Radius))
                {
                    // Explosive projectile: AOE on impact, skip normal damage
                    if (proj.IsExplosive)
                    {
                        WeaponSystem.ExplodeAt(state, proj.Position, proj.Damage, proj.ExplosionRadius);
                        proj.Active = false;
                        break;
                    }

                    // Normal projectile damage
                    int damage = proj.Damage;
                    bool crit = Random.Shared.NextSingle() < player.ComputedStats.CritChance;
                    if (crit) damage = (int)(damage * player.ComputedStats.CritDamage);
                    damage = (int)(damage * player.ComputedStats.DamageMultiplier);

                    enemy.CurrentHP -= damage;
                    enemy.FlashTimer = 0.1f;
                    state.TotalDamageDealt += damage;

                    Vector2 knockDir = Vector2.Normalize(enemy.Position - proj.Position);
                    enemy.KnockbackVelocity = knockDir * Constants.KnockbackForce;
                    enemy.KnockbackTimer = Constants.KnockbackDuration;

                    var dmgNum = state.GetInactiveDamageNumber();
                    dmgNum?.Init(enemy.Position, damage.ToString(),
                        crit ? Color.Yellow : Color.White);

                    if (enemy.CurrentHP <= 0 && !enemy.IsDying)
                    {
                        state.HandleEnemyDeath(enemy);
                        state.Assets.PlaySoundVariant("explosion", 0.3f);
                    }

                    if (proj.PierceCount <= 0)
                    {
                        proj.Active = false;
                        break;
                    }
                    proj.PierceCount--;
                }
            }
        }

        // Enemy vs Player
        if (player.InvincibilityTimer <= 0)
        {
            for (int e = 0; e < state.Enemies.Count; e++)
            {
                var enemy = state.Enemies[e];
                if (!enemy.Active) continue;

                if (enemy.IsDying) continue;
                if (enemy.IsLootEnemy) continue; // loot enemies don't hurt player
                if (CircleOverlap(player.Position, player.Radius, enemy.Position, enemy.Radius))
                {
                    // Kamikaze enemies explode on contact
                    if (enemy.IsKamikaze)
                    {
                        Systems.EnemySystem.Explode(enemy, state);
                        break;
                    }

                    // Dodge check
                    if (Random.Shared.NextSingle() < player.ComputedStats.DodgeChance)
                    {
                        var dmg = state.GetInactiveDamageNumber();
                        dmg?.Init(player.Position, "DODGE", Color.SkyBlue);
                        continue;
                    }

                    int damage = Math.Max(1, enemy.ContactDamage - player.ComputedStats.Armor);
                    player.CurrentHP -= damage;
                    player.InvincibilityTimer = Constants.PlayerInvincibilityTime;
                    player.FlashTimer = 0.15f;

                    // Player knockback away from enemy
                    Vector2 knockDir = Vector2.Normalize(player.Position - enemy.Position);
                    float knockForce = enemy.Behavior == Entities.EnemyBehavior.Tank ? 250f : 150f;
                    player.KnockbackVelocity = knockDir * knockForce;
                    player.KnockbackTimer = 0.12f;

                    // Trigger attack animation on contact
                    if (!enemy.IsAttacking)
                    {
                        enemy.IsAttacking = true;
                        enemy.AttackAnimTimer = 0.4f;
                        enemy.AttackAnimDuration = 0.4f;
                    }

                    var dmgNum = state.GetInactiveDamageNumber();
                    dmgNum?.Init(player.Position, damage.ToString(), Color.Red);

                    state.RequestScreenShake(0.08f, 1.5f);
                    state.Assets.PlaySoundVariant("hurt", 0.5f);
                    break;
                }
            }
        }

        // Enemy Projectile vs Player
        if (player.InvincibilityTimer <= 0)
        {
            for (int p = 0; p < state.EnemyProjectiles.Count; p++)
            {
                var proj = state.EnemyProjectiles[p];
                if (!proj.Active) continue;

                if (CircleOverlap(proj.Position, proj.Radius, player.Position, player.Radius))
                {
                    // Dodge check
                    if (Random.Shared.NextSingle() < player.ComputedStats.DodgeChance)
                    {
                        var dmg = state.GetInactiveDamageNumber();
                        dmg?.Init(player.Position, "DODGE", Color.SkyBlue);
                        proj.Active = false;
                        continue;
                    }

                    int damage = Math.Max(1, proj.Damage - player.ComputedStats.Armor);
                    player.CurrentHP -= damage;
                    player.InvincibilityTimer = Constants.PlayerInvincibilityTime;
                    player.FlashTimer = 0.15f;

                    // Player knockback from projectile direction
                    if (proj.Velocity.Length() > 0.1f)
                    {
                        player.KnockbackVelocity = Vector2.Normalize(proj.Velocity) * 120f;
                        player.KnockbackTimer = 0.1f;
                    }

                    proj.Active = false;

                    var dmgNum = state.GetInactiveDamageNumber();
                    dmgNum?.Init(player.Position, damage.ToString(), Color.Red);
                    state.RequestScreenShake(0.06f, 1.0f);
                    state.Assets.PlaySoundVariant("hurt", 0.5f);
                    break;
                }
            }
        }

        // Projectile vs Obstacles
        for (int p = 0; p < state.Projectiles.Count; p++)
        {
            var proj = state.Projectiles[p];
            if (!proj.Active) continue;

            for (int o = 0; o < state.Obstacles.Count; o++)
            {
                var obs = state.Obstacles[o];
                if (!obs.Active) continue;
                if (CircleOverlap(proj.Position, proj.Radius, obs.Position, obs.Radius))
                {
                    if (proj.IsExplosive)
                        WeaponSystem.ExplodeAt(state, proj.Position, proj.Damage, proj.ExplosionRadius);
                    proj.Active = false;
                    break;
                }
            }
        }

        // Enemy Projectile vs Obstacles
        for (int p = 0; p < state.EnemyProjectiles.Count; p++)
        {
            var proj = state.EnemyProjectiles[p];
            if (!proj.Active) continue;

            for (int o = 0; o < state.Obstacles.Count; o++)
            {
                var obs = state.Obstacles[o];
                if (!obs.Active) continue;
                if (CircleOverlap(proj.Position, proj.Radius, obs.Position, obs.Radius))
                {
                    proj.Active = false;
                    break;
                }
            }
        }

        // Projectile vs Barrels (player projectiles damage barrels)
        for (int p = 0; p < state.Projectiles.Count; p++)
        {
            var proj = state.Projectiles[p];
            if (!proj.Active) continue;

            for (int b = 0; b < state.Barrels.Count; b++)
            {
                var barrel = state.Barrels[b];
                if (!barrel.Active) continue;
                if (CircleOverlap(proj.Position, proj.Radius, barrel.Position, barrel.Radius))
                {
                    barrel.CurrentHP--;
                    barrel.FlashTimer = 0.1f;

                    if (proj.IsExplosive)
                    {
                        // Explosive projectile detonates and instantly kills barrel
                        barrel.CurrentHP = 0;
                        WeaponSystem.ExplodeAt(state, proj.Position, proj.Damage, proj.ExplosionRadius);
                    }

                    if (barrel.CurrentHP <= 0)
                        DestroyBarrel(state, barrel);

                    proj.Active = false;
                    break;
                }
            }
        }

        // Enemy Projectile vs Barrels (enemy fire also hits barrels)
        for (int p = 0; p < state.EnemyProjectiles.Count; p++)
        {
            var proj = state.EnemyProjectiles[p];
            if (!proj.Active) continue;

            for (int b = 0; b < state.Barrels.Count; b++)
            {
                var barrel = state.Barrels[b];
                if (!barrel.Active) continue;
                if (CircleOverlap(proj.Position, proj.Radius, barrel.Position, barrel.Radius))
                {
                    barrel.CurrentHP--;
                    barrel.FlashTimer = 0.1f;
                    if (barrel.CurrentHP <= 0)
                        DestroyBarrel(state, barrel);
                    proj.Active = false;
                    break;
                }
            }
        }

        // Player vs Health Pickups
        for (int i = 0; i < state.HealthPickups.Count; i++)
        {
            var pickup = state.HealthPickups[i];
            if (!pickup.Active) continue;
            if (CircleOverlap(player.Position, player.Radius + 4f, pickup.Position, pickup.Radius))
            {
                if (player.CurrentHP < player.ComputedStats.MaxHP)
                {
                    player.CurrentHP = Math.Min(player.CurrentHP + pickup.HealAmount,
                        player.ComputedStats.MaxHP);
                    pickup.Active = false;
                    var dmgNum = state.GetInactiveDamageNumber();
                    dmgNum?.Init(player.Position, $"+{pickup.HealAmount}", Color.Green);
                    state.Assets.PlaySoundVariant("coin", 0.4f);
                }
            }
        }

        // Player vs XP Orbs
        float pickupRange = player.ComputedStats.PickupRange;
        for (int i = 0; i < state.XPOrbs.Count; i++)
        {
            var orb = state.XPOrbs[i];
            if (!orb.Active || orb.SpawnTimer > 0) continue;

            float dist = Vector2.Distance(player.Position, orb.Position);

            if (dist < Constants.XPCollectRadius)
            {
                orb.Active = false;
                state.AddXP(orb.XPValue);
                state.Assets.PlaySoundVariant("coin", 0.3f);
            }
            else if (dist < pickupRange)
            {
                // Attract toward player
                Vector2 dir = Vector2.Normalize(player.Position - orb.Position);
                float attractSpeed = 150f + (1f - dist / pickupRange) * 200f;
                orb.Velocity = dir * attractSpeed;
            }
        }
    }
}
