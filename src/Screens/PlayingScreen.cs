using System.Numerics;
using CloneTato.Core;
using CloneTato.Entities;
using CloneTato.Systems;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class PlayingScreen
{
    private readonly GameCamera _camera = new();
    private float _screenShakeTimer;
    private float _screenShakeIntensity;

    private const float ReticleDistance = 60f; // fixed distance from player

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        // Compute mouse world position
        var mouseLogical = Display.ScreenToLogical(Raylib.GetMousePosition());
        state.MouseWorldPosition = Raylib.GetScreenToWorld2D(mouseLogical, _camera.Camera);
        state.IsFiring = InputHelper.IsFireDown();

        // Update all systems
        PlayerSystem.Update(dt, state);
        WeaponSystem.Update(dt, state);
        EnemySystem.Update(dt, state);
        WaveSystem.Update(dt, state);

        // Steer homing missiles before moving projectiles
        WeaponSystem.UpdateHomingProjectiles(dt, state);

        // Update projectiles + check expired explosives
        for (int i = 0; i < state.Projectiles.Count; i++)
        {
            var proj = state.Projectiles[i];
            if (!proj.Active) continue;

            bool wasActive = proj.Lifetime > 0;
            proj.Update(dt);

            // Explosive projectile expired (reached max range) — detonate
            if (wasActive && !proj.Active && proj.IsExplosive)
            {
                WeaponSystem.ExplodeAt(state, proj.Position, proj.Damage, proj.ExplosionRadius);
                TriggerScreenShake(0.15f, 2f);
            }
        }

        // Update enemy projectiles
        for (int i = 0; i < state.EnemyProjectiles.Count; i++)
            if (state.EnemyProjectiles[i].Active)
                state.EnemyProjectiles[i].Update(dt);

        // Update XP orbs
        for (int i = 0; i < state.XPOrbs.Count; i++)
            if (state.XPOrbs[i].Active)
                state.XPOrbs[i].Update(dt);

        // Update damage numbers
        for (int i = 0; i < state.DamageNumbers.Count; i++)
            if (state.DamageNumbers[i].Active)
                state.DamageNumbers[i].Update(dt);

        // Tick temporary terrain zones (ooze duration)
        CollisionSystem.UpdateTerrainZones(dt, state);

        // Tick barrel flash timers
        for (int i = 0; i < state.Barrels.Count; i++)
            if (state.Barrels[i].Active && state.Barrels[i].FlashTimer > 0)
                state.Barrels[i].FlashTimer -= dt;

        // Collision detection
        CollisionSystem.ProcessCollisions(state);

        // Check for crits that happened this frame (damage numbers colored yellow = crit)
        for (int i = 0; i < state.DamageNumbers.Count; i++)
        {
            var dn = state.DamageNumbers[i];
            if (dn.Active && dn.Lifetime >= Constants.DamageNumberDuration - 0.02f
                && dn.BaseColor.R == Color.Yellow.R && dn.BaseColor.G == Color.Yellow.G)
            {
                TriggerScreenShake(0.1f, 1.5f);
            }
        }

        // Camera
        _camera.Update(state.Player.Position, dt);

        // Screen shake (snap to integer pixels to prevent tile seams)
        if (_screenShakeTimer > 0)
        {
            _screenShakeTimer -= dt;
            float shake = _screenShakeIntensity * (_screenShakeTimer / 0.15f);
            _camera.Camera.Target += new Vector2(
                MathF.Round((Random.Shared.NextSingle() - 0.5f) * shake * 2f),
                MathF.Round((Random.Shared.NextSingle() - 0.5f) * shake * 2f));
        }

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
                manager.TransitionTo(GameScreen.Victory);
            else
                manager.TransitionTo(GameScreen.Shop);
        }
    }

    public void TriggerScreenShake(float duration, float intensity)
    {
        if (intensity > _screenShakeIntensity || _screenShakeTimer <= 0)
        {
            _screenShakeTimer = duration;
            _screenShakeIntensity = intensity;
        }
    }

    public void Draw(GameState state)
    {
        Raylib.ClearBackground(new Color(45, 30, 20, 255));

        Raylib.BeginMode2D(_camera.Camera);

        DrawArenaFloor(state);

        // Terrain zones (draw under everything)
        // Edge tile sets: [TL, T, TR, L, M, R, BL, B, BR, InnerTL, InnerTR, InnerBL, InnerBR]
        // Outer edges at (startCol, startRow) 3x3, inner corners at (startCol+3, startRow) 2x2
        int[] MakeEdgeSet(int startCol, int startRow)
        {
            return new[]
            {
                (startRow) * 18 + startCol,     (startRow) * 18 + startCol + 1,     (startRow) * 18 + startCol + 2,
                (startRow + 1) * 18 + startCol, (startRow + 1) * 18 + startCol + 1, (startRow + 1) * 18 + startCol + 2,
                (startRow + 2) * 18 + startCol, (startRow + 2) * 18 + startCol + 1, (startRow + 2) * 18 + startCol + 2,
                // Inner corners: TL+3, TL+4, then one row down
                (startRow) * 18 + startCol + 3,     (startRow) * 18 + startCol + 4,
                (startRow + 1) * 18 + startCol + 3, (startRow + 1) * 18 + startCol + 4,
            };
        }
        // 0=TL 1=T 2=TR 3=L 4=M 5=R 6=BL 7=B 8=BR 9=InnerTL 10=InnerTR 11=InnerBL 12=InnerBR
        int[] sandSet = MakeEdgeSet(10, 8);
        int[] greenGrassSet = MakeEdgeSet(5, 5);
        int[] purpleGrassSet = MakeEdgeSet(0, 5);
        int[] metallicSet = MakeEdgeSet(10, 5);

        float time = (float)Raylib.GetTime();
        for (int i = 0; i < state.TerrainZones.Count; i++)
        {
            var zone = state.TerrainZones[i];
            if (!zone.Active) continue;

            if (zone.Type == TerrainType.Sand || zone.Type == TerrainType.Decorative)
            {
                // Pick the right tile set
                int[] tileSet = zone.Type == TerrainType.Sand ? sandSet :
                    zone.DecoTileSet switch { 0 => greenGrassSet, 1 => purpleGrassSet, _ => metallicSet };

                int tileSize = Constants.TileSize;
                int left = (int)((zone.Position.X - zone.Radius) / tileSize) - 1;
                int right = (int)((zone.Position.X + zone.Radius) / tileSize) + 1;
                int top = (int)((zone.Position.Y - zone.Radius) / tileSize) - 1;
                int bottom = (int)((zone.Position.Y + zone.Radius) / tileSize) + 1;

                for (int row = top; row <= bottom; row++)
                {
                    for (int col = left; col <= right; col++)
                    {
                        float tx = col * tileSize + tileSize / 2f;
                        float ty = row * tileSize + tileSize / 2f;
                        if (Vector2.Distance(new Vector2(tx, ty), zone.Position) > zone.Radius)
                            continue;

                        bool nOut = Vector2.Distance(new Vector2(tx, ty - tileSize), zone.Position) > zone.Radius;
                        bool sOut = Vector2.Distance(new Vector2(tx, ty + tileSize), zone.Position) > zone.Radius;
                        bool wOut = Vector2.Distance(new Vector2(tx - tileSize, ty), zone.Position) > zone.Radius;
                        bool eOut = Vector2.Distance(new Vector2(tx + tileSize, ty), zone.Position) > zone.Radius;

                        int idx;
                        // Outer corners (convex)
                        if (nOut && wOut) idx = 0;       // TL
                        else if (nOut && eOut) idx = 2;  // TR
                        else if (sOut && wOut) idx = 6;  // BL
                        else if (sOut && eOut) idx = 8;  // BR
                        // Outer edges
                        else if (nOut) idx = 1;          // T
                        else if (sOut) idx = 7;          // B
                        else if (wOut) idx = 3;          // L
                        else if (eOut) idx = 5;          // R
                        else
                        {
                            // All cardinal neighbors inside — check diagonals for inner corners (concave)
                            bool nwOut = Vector2.Distance(new Vector2(tx - tileSize, ty - tileSize), zone.Position) > zone.Radius;
                            bool neOut = Vector2.Distance(new Vector2(tx + tileSize, ty - tileSize), zone.Position) > zone.Radius;
                            bool swOut = Vector2.Distance(new Vector2(tx - tileSize, ty + tileSize), zone.Position) > zone.Radius;
                            bool seOut = Vector2.Distance(new Vector2(tx + tileSize, ty + tileSize), zone.Position) > zone.Radius;

                            if (nwOut) idx = 9;       // Inner TL
                            else if (neOut) idx = 10;  // Inner TR
                            else if (swOut) idx = 11;  // Inner BL
                            else if (seOut) idx = 12;  // Inner BR
                            else idx = 4;              // M (center)
                        }

                        state.Assets.Tiles.Draw(tileSet[idx], col * tileSize, row * tileSize, Color.White);
                    }
                }
            }
            else if (zone.Type == TerrainType.Ooze)
            {
                byte pulse = (byte)(40 + (int)(15 * MathF.Sin(time * 4f)));
                Raylib.DrawCircleV(zone.Position, zone.Radius,
                    new Color((byte)30, pulse, (byte)10, (byte)80));
                Raylib.DrawCircleLinesV(zone.Position, zone.Radius,
                    new Color((byte)60, (byte)200, (byte)30, (byte)120));
            }
            else // Oasis
            {
                Raylib.DrawCircleV(zone.Position, zone.Radius,
                    new Color(60, 180, 120, 50));
                Raylib.DrawCircleLinesV(zone.Position, zone.Radius,
                    new Color(40, 160, 100, 80));
            }
        }

        Raylib.DrawRectangleLinesEx(
            new Rectangle(0, 0, Constants.ArenaWidth, Constants.ArenaHeight),
            2f, Color.Brown);

        // Mines
        for (int i = 0; i < state.Mines.Count; i++)
        {
            var mine = state.Mines[i];
            if (!mine.Active) continue;
            bool armed = mine.ArmTimer <= 0;
            Color mineColor = armed ? Color.Red : Color.Gray;
            Raylib.DrawCircleV(mine.Position, 5f, new Color((byte)40, (byte)40, (byte)40, (byte)200));
            Raylib.DrawCircleV(mine.Position, 3f, mineColor);
            if (armed)
            {
                // Blinking indicator
                float blink = MathF.Sin((float)Raylib.GetTime() * 8f);
                if (blink > 0) Raylib.DrawCircleV(mine.Position, 1.5f, Color.Yellow);
            }
        }

        // XP orbs
        for (int i = 0; i < state.XPOrbs.Count; i++)
        {
            var orb = state.XPOrbs[i];
            if (!orb.Active) continue;
            Raylib.DrawCircleV(orb.Position, 3f, Color.Lime);
            Raylib.DrawCircleV(orb.Position, 1.5f, Color.White);
        }

        // Health pickups — tile (6,12) = 12*18+6 = 222
        for (int i = 0; i < state.HealthPickups.Count; i++)
        {
            var pickup = state.HealthPickups[i];
            if (!pickup.Active) continue;
            state.Assets.Tiles.DrawCentered(12 * 18 + 6, pickup.Position.X, pickup.Position.Y, Color.White);
        }

        // Obstacles (trees — 2 tiles tall, position is at trunk/bottom)
        for (int i = 0; i < state.Obstacles.Count; i++)
        {
            var obs = state.Obstacles[i];
            if (!obs.Active) continue;
            // Bottom tile (trunk) centered on position
            state.Assets.Tiles.DrawCentered(obs.SpriteIndex + 18, obs.Position.X, obs.Position.Y, Color.White);
            // Top tile (canopy) 16px above
            state.Assets.Tiles.DrawCentered(obs.SpriteIndex, obs.Position.X, obs.Position.Y - 16, Color.White);
        }

        // Barrels
        for (int i = 0; i < state.Barrels.Count; i++)
        {
            var barrel = state.Barrels[i];
            if (!barrel.Active) continue;

            bool flashing = barrel.FlashTimer > 0;
            // Tile indices: explosive barrel at col 4 row 11, toxic at col 5 row 11
            int tileIdx = barrel.Type == BarrelType.Explosive ? (11 * 18 + 4) : (11 * 18 + 5);
            Color tint = flashing ? Color.Red : Color.White;
            state.Assets.Tiles.DrawCentered(tileIdx, barrel.Position.X, barrel.Position.Y, tint);
        }

        // Enemies
        for (int i = 0; i < state.Enemies.Count; i++)
        {
            var enemy = state.Enemies[i];
            if (!enemy.Active) continue;

            int spriteIdx = enemy.GetDisplaySprite();
            byte alpha = (byte)(enemy.DeathAlpha * 255);
            Color tint = enemy.FlashTimer > 0
                ? new Color((byte)255, (byte)80, (byte)80, alpha) // Red flash on hit
                : new Color((byte)229, (byte)229, (byte)229, alpha);

            if (enemy.Scale > 1f)
                state.Assets.Enemies.DrawScaled(spriteIdx, enemy.Position.X, enemy.Position.Y, enemy.Scale, tint);
            else
                state.Assets.Enemies.DrawCentered(spriteIdx, enemy.Position.X, enemy.Position.Y, tint);

            // Draw weapon on armed enemies
            if (enemy.IsArmed && !enemy.IsDying)
            {
                Vector2 toPlayer = state.Player.Position - enemy.Position;
                float weapAngle = toPlayer.LengthSquared() > 1f
                    ? MathF.Atan2(toPlayer.Y, toPlayer.X)
                    : 0f;
                float weapDist = 10f * enemy.Scale;
                Vector2 weapOffset = new(MathF.Cos(weapAngle) * weapDist, MathF.Sin(weapAngle) * weapDist);
                var weapTint = new Color((byte)229, (byte)229, (byte)229, alpha);
                if (enemy.Scale > 1f)
                    state.Assets.Weapons.DrawScaled(enemy.WeaponSpriteIndex,
                        enemy.Position.X + weapOffset.X, enemy.Position.Y + weapOffset.Y,
                        enemy.Scale, weapTint);
                else
                    state.Assets.Weapons.DrawCentered(enemy.WeaponSpriteIndex,
                        enemy.Position.X + weapOffset.X, enemy.Position.Y + weapOffset.Y, weapTint);
            }

            if (!enemy.IsDying && enemy.CurrentHP < enemy.MaxHP)
            {
                float pct = (float)enemy.CurrentHP / enemy.MaxHP;
                int bw = enemy.IsBoss ? 40 : 20;
                int yOff = (int)(16 * enemy.Scale);
                Raylib.DrawRectangle((int)enemy.Position.X - bw / 2, (int)enemy.Position.Y - yOff, bw, 3, Color.DarkGray);
                Raylib.DrawRectangle((int)enemy.Position.X - bw / 2, (int)enemy.Position.Y - yOff,
                    (int)(bw * pct), 3, enemy.IsBoss ? Color.Yellow : Color.Red);
            }
        }

        // Melee swipe arcs
        for (int i = 0; i < state.MeleeSwipes.Count; i++)
        {
            var swipe = state.MeleeSwipes[i];
            if (!swipe.Active) continue;
            DrawMeleeSwipe(swipe);
        }

        // Player
        DrawPlayer(state);

        // Orbiting weapons
        DrawWeapons(state);

        // Projectiles
        for (int i = 0; i < state.Projectiles.Count; i++)
        {
            var proj = state.Projectiles[i];
            if (!proj.Active) continue;

            if (proj.IsExplosive)
            {
                // Draw explosive projectiles larger with a trail
                Raylib.DrawCircleV(proj.Position, proj.Radius + 1f, proj.ProjectileColor);
                Raylib.DrawCircleV(proj.Position, proj.Radius - 1f, Color.Yellow);
            }
            else
            {
                Raylib.DrawCircleV(proj.Position, proj.Radius, proj.ProjectileColor);
            }
        }

        // Enemy projectiles
        for (int i = 0; i < state.EnemyProjectiles.Count; i++)
        {
            var proj = state.EnemyProjectiles[i];
            if (!proj.Active) continue;
            Raylib.DrawCircleV(proj.Position, proj.Radius, Color.Orange);
            Raylib.DrawCircleV(proj.Position, proj.Radius - 1f, Color.Red);
        }

        // Damage numbers
        for (int i = 0; i < state.DamageNumbers.Count; i++)
        {
            var dn = state.DamageNumbers[i];
            if (!dn.Active) continue;
            Color c = new(dn.BaseColor.R, dn.BaseColor.G, dn.BaseColor.B, (byte)(255 * dn.Alpha));

            // Crits are bigger and bolder
            bool isCrit = dn.BaseColor.R == Color.Yellow.R && dn.BaseColor.G == Color.Yellow.G
                          && dn.BaseColor.B == Color.Yellow.B;
            int fontSize = isCrit ? 10 : 8;

            if (isCrit)
            {
                // Shadow for crits
                Raylib.DrawText(dn.Text, (int)dn.Position.X - 3, (int)dn.Position.Y + 1, fontSize,
                    new Color((byte)0, (byte)0, (byte)0, (byte)(180 * dn.Alpha)));
            }

            Raylib.DrawText(dn.Text, (int)dn.Position.X - 4, (int)dn.Position.Y, fontSize, c);
        }

        // Targeting reticle (fixed distance from player)
        DrawReticle(state);

        Raylib.EndMode2D();

        UIRenderer.DrawHUD(state);
    }

    private void DrawMeleeSwipe(Entities.MeleeSwipe swipe)
    {
        float alpha = swipe.Alpha;
        byte a = (byte)(180 * alpha);
        Color color = new(swipe.SwipeColor.R, swipe.SwipeColor.G, swipe.SwipeColor.B, a);

        // Draw arc as a series of triangles
        int segments = 8;
        float halfArc = swipe.ArcWidth / 2f;
        float startAngle = swipe.ArcAngle - halfArc;
        float angleStep = swipe.ArcWidth / segments;

        // Expanding radius during the swipe
        float radius = swipe.SwipeRadius * (1.2f - alpha * 0.2f);

        for (int s = 0; s < segments; s++)
        {
            float a1 = startAngle + s * angleStep;
            float a2 = startAngle + (s + 1) * angleStep;

            Vector2 p1 = swipe.Position + new Vector2(MathF.Cos(a1), MathF.Sin(a1)) * radius * 0.3f;
            Vector2 p2 = swipe.Position + new Vector2(MathF.Cos(a1), MathF.Sin(a1)) * radius;
            Vector2 p3 = swipe.Position + new Vector2(MathF.Cos(a2), MathF.Sin(a2)) * radius;
            Vector2 p4 = swipe.Position + new Vector2(MathF.Cos(a2), MathF.Sin(a2)) * radius * 0.3f;

            Raylib.DrawTriangle(p1, p2, p3, color);
            Raylib.DrawTriangle(p1, p3, p4, color);
        }
    }

    private void DrawWeapons(GameState state)
    {
        for (int w = 0; w < state.EquippedWeapons.Count; w++)
        {
            var weapon = state.EquippedWeapons[w];
            Vector2 weaponPos = WeaponSystem.GetWeaponWorldPosition(state, w);
            float angle = WeaponSystem.GetWeaponDrawAngle(state, w);

            var src = state.Assets.Weapons.GetSourceRect(weapon.Def.SpriteIndex);
            float drawSize = Constants.SpriteSize * 0.7f;
            var dest = new Rectangle(weaponPos.X, weaponPos.Y, drawSize, drawSize);
            var origin = new Vector2(drawSize / 2f, drawSize / 2f);
            float angleDeg = angle * (180f / MathF.PI);

            Color tint = weapon.UpgradeLevel > 0
                ? new Color((byte)255, (byte)(255 - weapon.UpgradeLevel * 15),
                    (byte)(200 - weapon.UpgradeLevel * 30), (byte)255)
                : Color.White;

            Raylib.DrawTexturePro(state.Assets.Weapons.Texture, src, dest, origin, angleDeg, tint);

            if (weapon.UpgradeLevel > 0)
            {
                for (int s = 0; s < weapon.UpgradeLevel; s++)
                {
                    float starX = weaponPos.X - (weapon.UpgradeLevel * 2f) / 2f + s * 2.5f;
                    float starY = weaponPos.Y + drawSize / 2f + 2f;
                    Raylib.DrawCircleV(new Vector2(starX, starY), 0.8f, Color.Gold);
                }
            }
        }
    }

    private void DrawReticle(GameState state)
    {
        var player = state.Player;
        Vector2 mouseWorld = state.MouseWorldPosition;

        // Direction from player to mouse
        Vector2 dir = mouseWorld - player.Position;
        float dist = dir.Length();
        if (dist > 1f) dir /= dist;
        else dir = new Vector2(1, 0);

        // Reticle at fixed distance from player
        Vector2 reticlePos = player.Position + dir * ReticleDistance;

        bool firing = state.IsFiring;
        float time = (float)Raylib.GetTime();

        // Always red and pulsating
        float pulse = MathF.Sin(time * 12f) * 0.5f + 0.5f; // 0..1 pulsing
        float outerRadius = 7f + pulse * 2f;
        byte alpha = (byte)(180 + pulse * 75);
        Color ringColor = new((byte)255, (byte)60, (byte)60, alpha);
        Raylib.DrawCircleLinesV(reticlePos, outerRadius, ringColor);

        // Inner dot
        Raylib.DrawCircleV(reticlePos, 1.5f, new Color((byte)255, (byte)80, (byte)80, (byte)255));

        // Crosshair ticks
        float gap = outerRadius + 2f;
        float len = 4f;
        Raylib.DrawLineV(reticlePos + new Vector2(-gap - len, 0), reticlePos + new Vector2(-gap, 0), ringColor);
        Raylib.DrawLineV(reticlePos + new Vector2(gap, 0), reticlePos + new Vector2(gap + len, 0), ringColor);
        Raylib.DrawLineV(reticlePos + new Vector2(0, -gap - len), reticlePos + new Vector2(0, -gap), ringColor);
        Raylib.DrawLineV(reticlePos + new Vector2(0, gap), reticlePos + new Vector2(0, gap + len), ringColor);

        // Line from player to reticle (subtle red aim line)
        Raylib.DrawLineV(player.Position, reticlePos, new Color((byte)255, (byte)60, (byte)60, (byte)40));
    }

    private void DrawArenaFloor(GameState state)
    {
        float camLeft = _camera.Camera.Target.X - Constants.LogicalWidth / 2f;
        float camTop = _camera.Camera.Target.Y - Constants.LogicalHeight / 2f;
        float camRight = camLeft + Constants.LogicalWidth;
        float camBottom = camTop + Constants.LogicalHeight;

        int startCol = Math.Max(0, (int)(camLeft / Constants.TileSize) - 1);
        int startRow = Math.Max(0, (int)(camTop / Constants.TileSize) - 1);
        int endCol = Math.Min(Constants.ArenaWidth / Constants.TileSize, (int)(camRight / Constants.TileSize) + 2);
        int endRow = Math.Min(Constants.ArenaHeight / Constants.TileSize, (int)(camBottom / Constants.TileSize) + 2);

        // Base ground tiles: mostly (10,3), sprinkle in (11,3) for variety
        int baseTile0 = 3 * 18 + 10;
        int baseTile1 = 3 * 18 + 11;
        for (int row = startRow; row < endRow; row++)
            for (int col = startCol; col < endCol; col++)
            {
                // Pseudo-random scatter using bit mixing (avoids visible patterns)
                int h = col * 374761393 + row * 668265263;
                h = (h ^ (h >> 13)) * 1274126177;
                int tileIdx = ((h & 0xFF) < 40) ? baseTile1 : baseTile0;
                state.Assets.Tiles.Draw(tileIdx, col * Constants.TileSize, row * Constants.TileSize, Color.White);
            }
    }

    private void DrawPlayer(GameState state)
    {
        var player = state.Player;

        // Dash afterimage trail
        if (player.IsDashing)
        {
            float t = player.DashTimer / Constants.DashDuration;
            int spriteIdx = player.GetDisplaySprite();
            var src = state.Assets.Players.GetSourceRect(spriteIdx);
            if (player.FacingLeft) src.Width = -src.Width;

            // Draw 2 ghost images behind the player
            for (int g = 1; g <= 2; g++)
            {
                Vector2 ghostPos = player.Position - player.DashDirection * (g * 10f);
                byte ghostAlpha = (byte)(80 * t / g);
                var ghostTint = new Color((byte)150, (byte)200, (byte)255, ghostAlpha);
                var ghostDest = new Rectangle(ghostPos.X, ghostPos.Y, Constants.SpriteSize, Constants.SpriteSize);
                var ghostOrigin = new Vector2(Constants.SpriteSize / 2f, Constants.SpriteSize / 2f);
                Raylib.DrawTexturePro(state.Assets.Players.Texture, src, ghostDest, ghostOrigin, 0f, ghostTint);
            }
        }

        if (player.InvincibilityTimer > 0 && !player.IsDashing && ((int)(player.InvincibilityTimer * 10) % 2 == 0))
            return;

        Color tint = player.IsDashing
            ? new Color((byte)180, (byte)220, (byte)255, (byte)255) // Bright blue tint while dashing
            : player.FlashTimer > 0 ? Color.Red : Color.White;
        int sprIdx = player.GetDisplaySprite();

        var mainSrc = state.Assets.Players.GetSourceRect(sprIdx);
        if (player.FacingLeft) mainSrc.Width = -mainSrc.Width;

        var dest = new Rectangle(player.Position.X, player.Position.Y, Constants.SpriteSize, Constants.SpriteSize);
        var origin = new Vector2(Constants.SpriteSize / 2f, Constants.SpriteSize / 2f);
        Raylib.DrawTexturePro(state.Assets.Players.Texture, mainSrc, dest, origin, 0f, tint);
    }
}
