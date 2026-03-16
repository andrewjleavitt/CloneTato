using System.Numerics;
using CloneTato.Entities;
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

                if (CircleOverlap(proj.Position, proj.Radius, enemy.Position, enemy.Radius))
                {
                    // Apply damage
                    int damage = proj.Damage;

                    // Crit check
                    bool crit = Random.Shared.NextSingle() < player.ComputedStats.CritChance;
                    if (crit) damage = (int)(damage * player.ComputedStats.CritDamage);

                    damage = (int)(damage * player.ComputedStats.DamageMultiplier);

                    enemy.CurrentHP -= damage;
                    enemy.FlashTimer = 0.1f;
                    state.TotalDamageDealt += damage;

                    // Knockback
                    Vector2 knockDir = Vector2.Normalize(enemy.Position - proj.Position);
                    enemy.KnockbackVelocity = knockDir * Constants.KnockbackForce;
                    enemy.KnockbackTimer = Constants.KnockbackDuration;

                    // Damage number
                    var dmgNum = state.GetInactiveDamageNumber();
                    if (dmgNum != null)
                    {
                        dmgNum.Init(enemy.Position, damage.ToString(),
                            crit ? Color.Yellow : Color.White);
                    }

                    // Enemy death
                    if (enemy.CurrentHP <= 0)
                    {
                        enemy.Active = false;
                        state.TotalEnemiesKilled++;
                        state.EnemiesKilledThisWave++;

                        // Drop XP
                        int orbCount = 1 + enemy.XPValue / 3;
                        for (int o = 0; o < orbCount; o++)
                        {
                            var orb = state.GetInactiveXPOrb();
                            orb?.Init(enemy.Position, Math.Max(1, enemy.XPValue / orbCount));
                        }

                        // Drop gold
                        state.Gold += enemy.GoldValue;

                        state.Assets.PlaySoundVariant("explosion", 0.3f);
                    }

                    // Pierce
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

                if (CircleOverlap(player.Position, player.Radius, enemy.Position, enemy.Radius))
                {
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

                    var dmgNum = state.GetInactiveDamageNumber();
                    dmgNum?.Init(player.Position, damage.ToString(), Color.Red);

                    state.Assets.PlaySoundVariant("hurt", 0.5f);
                    break;
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
