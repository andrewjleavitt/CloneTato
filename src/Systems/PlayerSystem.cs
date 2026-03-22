using System.Numerics;
using CloneTato.Core;
using CloneTato.Entities;
using Raylib_cs;

namespace CloneTato.Systems;

public static class PlayerSystem
{
    public static void Update(float dt, GameState state)
    {
        var player = state.Player;

        // Dash cooldown
        if (player.DashCooldownTimer > 0)
            player.DashCooldownTimer -= dt;

        // Post-dash buff timer
        if (player.DashBuffTimer > 0)
            player.DashBuffTimer -= dt;

        // Currently dashing — fully invincible, no damage
        if (player.IsDashing)
        {
            player.DashTimer -= dt;
            if (player.DashTimer <= 0)
            {
                player.IsDashing = false;
                float dashCD = Math.Max(0.15f, Constants.DashCooldown - player.ComputedStats.DashCooldownReduction);
                player.DashCooldownTimer = dashCD;

                // Post-dash buffs (only if player has earned them via upgrades)
                bool hasAnyDashBuff = player.ComputedStats.PostDashAttackSpeed > 0
                    || player.ComputedStats.PostDashMoveSpeed > 0
                    || player.ComputedStats.PostDashInvuln > 0;
                if (hasAnyDashBuff)
                    player.DashBuffTimer = Player.DashBuffDuration;

                // Post-dash invulnerability (from upgrade)
                if (player.ComputedStats.PostDashInvuln > 0)
                    player.InvincibilityTimer = player.ComputedStats.PostDashInvuln;
            }
            else
            {
                player.Velocity = player.DashDirection * (Constants.DashSpeed + player.ComputedStats.DashSpeedBonus);
                player.Position += player.Velocity * dt;

                // Clamp to arena + obstacle collision
                player.Position.X = Math.Clamp(player.Position.X, 12f, Constants.ArenaWidth - 12f);
                player.Position.Y = Math.Clamp(player.Position.Y, 12f, Constants.ArenaHeight - 12f);
                CollisionSystem.ResolveObstacleCollision(state, ref player.Position, player.Radius);

                // Stay invincible during entire dash — don't tick down InvincibilityTimer
                if (player.FlashTimer > 0) player.FlashTimer -= dt;
                state.TotalTimeSurvived += dt;
                return;
            }
        }

        // Input
        Vector2 input = InputHelper.GetMoveInput();

        // Gamepad aim: right stick overrides mouse for aim direction
        Vector2 gamepadAim = InputHelper.GetAimInput();
        if (gamepadAim.LengthSquared() > 0)
        {
            // Set mouse world position relative to player for weapon aiming
            state.MouseWorldPosition = player.Position + Vector2.Normalize(gamepadAim) * 80f;
        }

        // Dash initiation
        if (InputHelper.IsDashPressed() && player.DashCooldownTimer <= 0)
        {
            Vector2 dashDir = input.LengthSquared() > 0.1f ? Vector2.Normalize(input) : Vector2.Zero;
            // If not moving, dash toward aim direction
            if (dashDir == Vector2.Zero)
            {
                Vector2 toAim = state.MouseWorldPosition - player.Position;
                if (toAim.LengthSquared() > 1f)
                    dashDir = Vector2.Normalize(toAim);
            }
            if (dashDir != Vector2.Zero)
            {
                player.IsDashing = true;
                float dashDur = Constants.DashDuration + player.ComputedStats.DashDurationBonus;
                player.DashTimer = dashDur;
                player.DashDirection = dashDir;
                player.InvincibilityTimer = dashDur; // i-frames during dash
                return;
            }
        }

        // Face toward aim (with deadzone to prevent flicker)
        Vector2 faceDir = state.MouseWorldPosition - player.Position;
        if (MathF.Abs(faceDir.X) > 8f)
            player.FacingLeft = faceDir.X < 0;

        // Post-dash move speed buff
        float moveSpeed = player.ComputedStats.MoveSpeed;
        if (player.DashBuffTimer > 0 && player.ComputedStats.PostDashMoveSpeed > 0)
            moveSpeed *= (1f + player.ComputedStats.PostDashMoveSpeed);

        // Terrain zone speed modifier
        float terrainMult = CollisionSystem.GetTerrainSpeedMultiplier(state, player.Position);
        moveSpeed *= terrainMult;

        // Apply knockback if active — overrides movement input
        if (player.KnockbackTimer > 0)
        {
            player.KnockbackTimer -= dt;
            player.Position += player.KnockbackVelocity * dt;
            player.KnockbackVelocity *= 0.85f; // friction decay
        }

        player.Velocity = input * moveSpeed;
        player.Position += player.Velocity * dt;

        // Clamp to arena + obstacle collision
        player.Position.X = Math.Clamp(player.Position.X, 12f, Constants.ArenaWidth - 12f);
        player.Position.Y = Math.Clamp(player.Position.Y, 12f, Constants.ArenaHeight - 12f);
        CollisionSystem.ResolveObstacleCollision(state, ref player.Position, player.Radius);

        // Terrain healing (oasis)
        float healRate = CollisionSystem.GetTerrainHealRate(state, player.Position);
        if (healRate > 0 && player.CurrentHP < player.ComputedStats.MaxHP)
        {
            player.CurrentHP = Math.Min(player.CurrentHP + (int)(healRate * dt + 0.5f),
                player.ComputedStats.MaxHP);
        }

        // Terrain damage (ooze)
        if (player.InvincibilityTimer <= 0)
        {
            float oozeDmg = CollisionSystem.GetTerrainDamageRate(state, player.Position);
            if (oozeDmg > 0)
            {
                int dmg = Math.Max(1, (int)(oozeDmg * dt + 0.5f) - player.ComputedStats.Armor);
                player.CurrentHP -= dmg;
                player.FlashTimer = 0.08f;
            }
        }

        // Timers
        if (player.InvincibilityTimer > 0) player.InvincibilityTimer -= dt;
        if (player.FlashTimer > 0) player.FlashTimer -= dt;
        if (player.MeleeAnimTimer > 0) player.MeleeAnimTimer -= dt;

        // Animation
        if (player.Velocity.LengthSquared() > 1f)
            player.AnimTimer += dt;

        state.TotalTimeSurvived += dt;

        // Combo timer decay
        if (state.ComboTimer > 0)
        {
            state.ComboTimer -= dt;
            if (state.ComboTimer <= 0)
                state.ComboCount = 0;
        }
    }
}
