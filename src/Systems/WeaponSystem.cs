using System.Numerics;
using CloneTato.Core;
using CloneTato.Data;
using CloneTato.Entities;

namespace CloneTato.Systems;

public static class WeaponSystem
{
    public static void Update(float dt, GameState state)
    {
        var player = state.Player;

        for (int w = 0; w < state.EquippedWeapons.Count; w++)
        {
            // Tick cooldown
            state.WeaponCooldowns[w] -= dt;
            if (state.WeaponCooldowns[w] > 0) continue;

            var weapon = state.EquippedWeapons[w];

            // Find nearest enemy
            Enemy? nearest = null;
            float nearestDist = weapon.Range;

            for (int e = 0; e < state.Enemies.Count; e++)
            {
                var enemy = state.Enemies[e];
                if (!enemy.Active) continue;
                float dist = Vector2.Distance(player.Position, enemy.Position);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = enemy;
                }
            }

            if (nearest == null) continue;

            // Fire!
            Vector2 dir = Vector2.Normalize(nearest.Position - player.Position);
            float cooldown = 1f / (weapon.FireRate * player.ComputedStats.AttackSpeedMultiplier);
            state.WeaponCooldowns[w] = cooldown;

            int damage = (int)weapon.BaseDamage;

            if (weapon.BurstCount > 1)
            {
                // Shotgun-style: fire multiple projectiles with spread
                float baseAngle = MathF.Atan2(dir.Y, dir.X);
                for (int b = 0; b < weapon.BurstCount; b++)
                {
                    float spreadAngle = baseAngle + (Random.Shared.NextSingle() - 0.5f) * weapon.Spread * 2f;
                    Vector2 spreadDir = new(MathF.Cos(spreadAngle), MathF.Sin(spreadAngle));
                    FireProjectile(state, player.Position, spreadDir, weapon, damage);
                }
            }
            else
            {
                // Single shot with optional small spread
                if (weapon.Spread > 0)
                {
                    float baseAngle = MathF.Atan2(dir.Y, dir.X);
                    float spreadAngle = baseAngle + (Random.Shared.NextSingle() - 0.5f) * weapon.Spread;
                    dir = new Vector2(MathF.Cos(spreadAngle), MathF.Sin(spreadAngle));
                }
                FireProjectile(state, player.Position, dir, weapon, damage);
            }

            state.Assets.PlaySoundVariant("shoot", 0.25f);
        }
    }

    private static void FireProjectile(GameState state, Vector2 origin, Vector2 dir, WeaponDef weapon, int damage)
    {
        var proj = state.GetInactiveProjectile();
        if (proj == null) return;

        float lifetime = weapon.Range / weapon.ProjectileSpeed;
        proj.Init(origin, dir * weapon.ProjectileSpeed, damage, lifetime, weapon.PierceCount, weapon.ProjectileColor);
    }
}
