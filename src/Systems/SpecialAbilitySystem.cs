using System.Numerics;
using CloneTato.Core;
using CloneTato.Data;
using CloneTato.Entities;
using Raylib_cs;

namespace CloneTato.Systems;

public static class SpecialAbilitySystem
{
    // Cooldowns per hero type
    public const float GunslingerCooldown = 8f;
    public const float BladeDancerCooldown = 10f;
    public const float DrifterCooldown = 12f;

    // Shockwave parameters
    private const float ShockwaveRadius = 80f;
    private const int ShockwaveDamage = 30;
    private const float ShockwaveKnockback = 400f;

    // Blade Storm parameters
    private const float BladeStormRadius = 45f;
    private const int BladeStormHits = 3;
    private const float BladeStormInvulnTime = 0.4f;

    // Companion Surge parameters
    private const int SurgeProjectiles = 8;
    private const int SurgeDamage = 15;
    private const float SurgeSpeed = 180f;

    public static float GetMaxCooldown(HeroType hero) => hero switch
    {
        HeroType.Gunslinger => GunslingerCooldown,
        HeroType.BladeDancer => BladeDancerCooldown,
        HeroType.Drifter => DrifterCooldown,
        _ => 10f,
    };

    public static void Update(GameState state)
    {
        if (!state.IsSpecialActivated) return;
        if (state.SpecialCooldown > 0) return;
        if (state.Player.CurrentHP <= 0) return;

        var player = state.Player;
        state.SpecialMaxCooldown = GetMaxCooldown(player.HeroType);
        state.SpecialCooldown = state.SpecialMaxCooldown;

        switch (player.HeroType)
        {
            case HeroType.Gunslinger:
                ActivateShockwave(state);
                break;
            case HeroType.BladeDancer:
                ActivateBladeStorm(state);
                break;
            case HeroType.Drifter:
                ActivateCompanionSurge(state);
                break;
        }
    }

    private static void ActivateShockwave(GameState state)
    {
        var player = state.Player;
        float dmgMult = player.ComputedStats.DamageMultiplier;
        int hitCount = 0;

        for (int e = 0; e < state.Enemies.Count; e++)
        {
            var enemy = state.Enemies[e];
            if (!enemy.Active || enemy.IsDying || enemy.IsBurrowed) continue;

            float dist = Vector2.Distance(player.Position, enemy.Position);
            if (dist > ShockwaveRadius + enemy.Radius) continue;

            // Damage falls off with distance
            float falloff = 1f - (dist / (ShockwaveRadius + enemy.Radius)) * 0.4f;
            int damage = (int)(ShockwaveDamage * dmgMult * falloff);

            bool crit = Random.Shared.NextSingle() < player.ComputedStats.CritChance;
            if (crit) damage = (int)(damage * player.ComputedStats.CritDamage);

            enemy.CurrentHP -= damage;
            enemy.FlashTimer = 0.15f;
            state.TotalDamageDealt += damage;
            hitCount++;

            // Strong knockback away from player
            Vector2 knockDir = dist > 1f
                ? Vector2.Normalize(enemy.Position - player.Position)
                : new Vector2(Random.Shared.NextSingle() - 0.5f, Random.Shared.NextSingle() - 0.5f);
            float knockResist = enemy.IsBoss ? 0.3f
                : enemy.Behavior == EnemyBehavior.Tank ? 0.5f
                : 1f;
            enemy.KnockbackVelocity = knockDir * ShockwaveKnockback * knockResist;
            enemy.KnockbackTimer = 0.25f;

            var dmgNum = state.GetInactiveDamageNumber();
            dmgNum?.Init(enemy.Position, damage.ToString(),
                crit ? Color.Yellow : Color.Orange);

            if (enemy.CurrentHP <= 0 && !enemy.IsDying)
                state.HandleEnemyDeath(enemy);
        }

        // VFX: explosion at player position
        state.SpawnExplosionVFX(player.Position, ShockwaveRadius);
        state.RequestScreenShake(0.15f, 3f);
        state.RequestHitstop(0.06f);
        state.Assets.PlaySoundVariant("explosion", 0.6f);
    }

    private static void ActivateBladeStorm(GameState state)
    {
        var player = state.Player;
        float dmgMult = player.ComputedStats.DamageMultiplier * player.ComputedStats.MeleeDamageMultiplier;
        int totalHits = 0;

        // Full 360° sweep, multiple hits
        for (int hit = 0; hit < BladeStormHits; hit++)
        {
            for (int e = 0; e < state.Enemies.Count; e++)
            {
                var enemy = state.Enemies[e];
                if (!enemy.Active || enemy.IsDying || enemy.IsBurrowed) continue;

                float dist = Vector2.Distance(player.Position, enemy.Position);
                if (dist > BladeStormRadius + enemy.Radius) continue;

                int damage = (int)(20 * dmgMult);
                bool crit = Random.Shared.NextSingle() < player.ComputedStats.CritChance;
                if (crit) damage = (int)(damage * player.ComputedStats.CritDamage);

                enemy.CurrentHP -= damage;
                enemy.FlashTimer = 0.1f;
                state.TotalDamageDealt += damage;
                totalHits++;

                // Knockback outward
                Vector2 knockDir = dist > 1f
                    ? Vector2.Normalize(enemy.Position - player.Position)
                    : new Vector2(MathF.Cos(hit * MathF.PI * 0.67f), MathF.Sin(hit * MathF.PI * 0.67f));
                float knockResist = enemy.IsBoss ? 0.3f : 1f;
                enemy.KnockbackVelocity = knockDir * 250f * knockResist;
                enemy.KnockbackTimer = 0.12f;

                var dmgNum = state.GetInactiveDamageNumber();
                dmgNum?.Init(enemy.Position, damage.ToString(),
                    crit ? Color.Yellow : Color.White);

                if (enemy.CurrentHP <= 0 && !enemy.IsDying)
                {
                    state.HandleEnemyDeath(enemy);
                    state.RequestHitstop(0.04f);
                }
            }
        }

        // Brief invulnerability
        player.InvincibilityTimer = MathF.Max(player.InvincibilityTimer, BladeStormInvulnTime);

        // Trigger melee animation
        player.MeleeAnimTimer = 0.5f;
        player.MeleeAttackCount++;

        // Spawn swipe visual — full circle
        var swipe = state.GetInactiveMeleeSwipe();
        swipe?.Init(player.Position, 0f, MathF.PI * 2f, BladeStormRadius,
            new Color((byte)255, (byte)100, (byte)200, (byte)255)); // magenta storm

        state.RequestScreenShake(0.12f, 2.5f);
        if (totalHits >= 3) state.RequestHitstop(0.05f);
        state.Assets.PlaySoundVariant("hurt", 0.4f);
    }

    private static void ActivateCompanionSurge(GameState state)
    {
        var player = state.Player;

        // Fire homing projectiles in all directions
        for (int i = 0; i < SurgeProjectiles; i++)
        {
            var proj = state.GetInactiveProjectile();
            if (proj == null) break;

            float angle = (MathF.PI * 2f / SurgeProjectiles) * i
                + (Random.Shared.NextSingle() - 0.5f) * 0.3f;
            Vector2 dir = new(MathF.Cos(angle), MathF.Sin(angle));

            int damage = (int)(SurgeDamage * player.ComputedStats.DamageMultiplier);
            float lifetime = 3f;
            proj.Init(player.Position, dir * SurgeSpeed, damage, lifetime,
                0, Color.Lime, 0f);
            proj.IsHoming = true;

            // Find nearest enemy to assign as target
            float bestDist = float.MaxValue;
            int bestIdx = -1;
            for (int e = 0; e < state.Enemies.Count; e++)
            {
                if (!state.Enemies[e].Active || state.Enemies[e].IsDying) continue;
                float d = Vector2.Distance(player.Position, state.Enemies[e].Position);
                if (d < bestDist) { bestDist = d; bestIdx = e; }
            }
            // Spread targets among enemies
            if (bestIdx >= 0)
            {
                // Find i-th closest enemy for spread
                proj.TargetEnemyIndex = bestIdx;
            }
            proj.TurnRate = 4f;
        }

        state.RequestScreenShake(0.08f, 1.5f);
        state.Assets.PlaySoundVariant("shoot", 0.5f);
    }
}
