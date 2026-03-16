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

        // Input
        Vector2 input = Vector2.Zero;
        if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up)) input.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down)) input.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left)) input.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right)) input.X += 1;

        if (input.LengthSquared() > 0)
        {
            input = Vector2.Normalize(input);
            if (input.X < 0) player.FacingLeft = true;
            else if (input.X > 0) player.FacingLeft = false;
        }

        player.Velocity = input * player.ComputedStats.MoveSpeed;
        player.Position += player.Velocity * dt;

        // Clamp to arena
        player.Position.X = Math.Clamp(player.Position.X, 12f, Constants.ArenaWidth - 12f);
        player.Position.Y = Math.Clamp(player.Position.Y, 12f, Constants.ArenaHeight - 12f);

        // Timers
        if (player.InvincibilityTimer > 0) player.InvincibilityTimer -= dt;
        if (player.FlashTimer > 0) player.FlashTimer -= dt;

        // Animation
        if (player.Velocity.LengthSquared() > 1f)
            player.AnimTimer += dt;

        state.TotalTimeSurvived += dt;
    }
}
