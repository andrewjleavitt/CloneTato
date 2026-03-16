using System.Numerics;
using CloneTato.Core;
using CloneTato.Systems;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class PlayingScreen
{
    private readonly GameCamera _camera = new();

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        // Update all systems
        PlayerSystem.Update(dt, state);
        WeaponSystem.Update(dt, state);
        EnemySystem.Update(dt, state);
        WaveSystem.Update(dt, state);

        // Update projectiles
        for (int i = 0; i < state.Projectiles.Count; i++)
            if (state.Projectiles[i].Active)
                state.Projectiles[i].Update(dt);

        // Update XP orbs
        for (int i = 0; i < state.XPOrbs.Count; i++)
            if (state.XPOrbs[i].Active)
                state.XPOrbs[i].Update(dt);

        // Update damage numbers
        for (int i = 0; i < state.DamageNumbers.Count; i++)
            if (state.DamageNumbers[i].Active)
                state.DamageNumbers[i].Update(dt);

        // Collision detection
        CollisionSystem.ProcessCollisions(state);

        // Camera
        _camera.Update(state.Player.Position, dt);

        // Check player death
        if (state.Player.CurrentHP <= 0)
        {
            state.Assets.PlaySoundVariant("lose", 0.5f);
            manager.TransitionTo(GameScreen.GameOver);
            return;
        }

        // Check level up
        if (state.LevelUpPending)
        {
            manager.TransitionTo(GameScreen.LevelUp);
            return;
        }

        // Check wave complete
        if (!state.WaveActive)
        {
            if (state.CurrentWave >= Constants.MaxWaves)
            {
                manager.TransitionTo(GameScreen.Victory);
            }
            else
            {
                manager.TransitionTo(GameScreen.Shop);
            }
        }
    }

    public void Draw(GameState state)
    {
        Raylib.ClearBackground(new Color(45, 30, 20, 255));

        Raylib.BeginMode2D(_camera.Camera);

        // Draw arena floor
        DrawArenaFloor(state);

        // Draw arena border
        Raylib.DrawRectangleLinesEx(
            new Rectangle(0, 0, Constants.ArenaWidth, Constants.ArenaHeight),
            2f, Color.Brown);

        // Draw XP orbs
        for (int i = 0; i < state.XPOrbs.Count; i++)
        {
            var orb = state.XPOrbs[i];
            if (!orb.Active) continue;
            Raylib.DrawCircleV(orb.Position, 3f, Color.Lime);
            Raylib.DrawCircleV(orb.Position, 1.5f, Color.White);
        }

        // Draw enemies
        for (int i = 0; i < state.Enemies.Count; i++)
        {
            var enemy = state.Enemies[i];
            if (!enemy.Active) continue;
            Color tint = enemy.FlashTimer > 0 ? Color.White : Color.RayWhite;
            state.Assets.Enemies.DrawCentered(enemy.SpriteIndex, enemy.Position.X, enemy.Position.Y, tint);

            // Health bar for damaged enemies
            if (enemy.CurrentHP < enemy.MaxHP)
            {
                float pct = (float)enemy.CurrentHP / enemy.MaxHP;
                int bw = 20;
                Raylib.DrawRectangle((int)enemy.Position.X - bw / 2, (int)enemy.Position.Y - 16, bw, 3, Color.DarkGray);
                Raylib.DrawRectangle((int)enemy.Position.X - bw / 2, (int)enemy.Position.Y - 16,
                    (int)(bw * pct), 3, Color.Red);
            }
        }

        // Draw player
        DrawPlayer(state);

        // Draw projectiles
        for (int i = 0; i < state.Projectiles.Count; i++)
        {
            var proj = state.Projectiles[i];
            if (!proj.Active) continue;
            Raylib.DrawCircleV(proj.Position, proj.Radius, proj.ProjectileColor);
        }

        // Draw damage numbers
        for (int i = 0; i < state.DamageNumbers.Count; i++)
        {
            var dn = state.DamageNumbers[i];
            if (!dn.Active) continue;
            Color c = new(dn.BaseColor.R, dn.BaseColor.G, dn.BaseColor.B, (byte)(255 * dn.Alpha));
            Raylib.DrawText(dn.Text, (int)dn.Position.X - 4, (int)dn.Position.Y, 8, c);
        }

        Raylib.EndMode2D();

        // Draw HUD (screen space)
        UIRenderer.DrawHUD(state);
    }

    private void DrawArenaFloor(GameState state)
    {
        // Calculate visible area to avoid drawing off-screen tiles
        float camLeft = _camera.Camera.Target.X - Constants.LogicalWidth / 2f;
        float camTop = _camera.Camera.Target.Y - Constants.LogicalHeight / 2f;
        float camRight = camLeft + Constants.LogicalWidth;
        float camBottom = camTop + Constants.LogicalHeight;

        int startCol = Math.Max(0, (int)(camLeft / Constants.TileSize) - 1);
        int startRow = Math.Max(0, (int)(camTop / Constants.TileSize) - 1);
        int endCol = Math.Min(Constants.ArenaWidth / Constants.TileSize, (int)(camRight / Constants.TileSize) + 2);
        int endRow = Math.Min(Constants.ArenaHeight / Constants.TileSize, (int)(camBottom / Constants.TileSize) + 2);

        for (int row = startRow; row < endRow; row++)
        {
            for (int col = startCol; col < endCol; col++)
            {
                // Alternate between a few sand tiles for variety
                int tileIdx = ((col + row * 3) % 4) switch
                {
                    0 => 0,  // sand
                    1 => 1,  // sand variant
                    2 => 18, // more sand
                    _ => 0,  // sand
                };
                state.Assets.Tiles.Draw(tileIdx, col * Constants.TileSize, row * Constants.TileSize, Color.White);
            }
        }
    }

    private void DrawPlayer(GameState state)
    {
        var player = state.Player;

        // Skip rendering during invincibility flash
        if (player.InvincibilityTimer > 0 && ((int)(player.InvincibilityTimer * 10) % 2 == 0))
            return;

        Color tint = player.FlashTimer > 0 ? Color.Red : Color.White;
        int spriteIdx = player.GetDisplaySprite();

        // Flip if facing left
        var src = state.Assets.Players.GetSourceRect(spriteIdx);
        if (player.FacingLeft) src.Width = -src.Width;

        var dest = new Rectangle(player.Position.X, player.Position.Y, Constants.SpriteSize, Constants.SpriteSize);
        var origin = new Vector2(Constants.SpriteSize / 2f, Constants.SpriteSize / 2f);
        Raylib.DrawTexturePro(state.Assets.Players.Texture, src, dest, origin, 0f, tint);
    }
}
