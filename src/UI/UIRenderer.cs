using System.Numerics;
using CloneTato.Assets;
using CloneTato.Core;
using CloneTato.Entities;
using Raylib_cs;

namespace CloneTato.UI;

public static class UIRenderer
{
    public static void DrawHUD(GameState state)
    {
        // Health bar (top left)
        int barX = 8, barY = 6;
        int barW = 80, barH = 8;
        float hpPct = (float)state.Player.CurrentHP / state.Player.ComputedStats.MaxHP;
        hpPct = Math.Clamp(hpPct, 0f, 1f);

        Raylib.DrawRectangle(barX - 1, barY - 1, barW + 2, barH + 2, Color.Black);
        Raylib.DrawRectangle(barX, barY, barW, barH, Color.DarkGray);
        Color hpColor = hpPct > 0.5f ? Color.Green : hpPct > 0.25f ? Color.Orange : Color.Red;
        Raylib.DrawRectangle(barX, barY, (int)(barW * hpPct), barH, hpColor);
        DrawTextSmall($"{state.Player.CurrentHP}/{state.Player.ComputedStats.MaxHP}",
            barX + barW + 4, barY, Color.White);

        // XP bar (below health)
        int xpY = barY + barH + 3;
        float xpPct = (float)state.XP / state.XPToNextLevel;
        Raylib.DrawRectangle(barX - 1, xpY - 1, barW + 2, 5, Color.Black);
        Raylib.DrawRectangle(barX, xpY, barW, 3, Color.DarkGray);
        Raylib.DrawRectangle(barX, xpY, (int)(barW * xpPct), 3, Color.SkyBlue);
        DrawTextSmall($"Lv {state.Level}", barX + barW + 4, xpY - 1, Color.SkyBlue);

        // Wave info (top center)
        string waveText = $"Wave {state.CurrentWave}/{Constants.MaxWaves}";
        int textW = waveText.Length * 5;
        DrawTextSmall(waveText, Constants.LogicalWidth / 2 - textW / 2, 4, Color.White);

        if (state.WaveActive && state.WaveTimer > 0)
        {
            string timerText = $"{state.WaveTimer:F0}s";
            int timerW = timerText.Length * 5;
            DrawTextSmall(timerText, Constants.LogicalWidth / 2 - timerW / 2, 14, Color.Gold);
        }

        // Gold (top right)
        string goldText = $"${state.Gold}";
        DrawTextSmall(goldText, Constants.LogicalWidth - goldText.Length * 5 - 8, 4, Color.Gold);

        // Enemies remaining
        int remaining = state.ActiveEnemyCount();
        if (state.WaveActive && remaining > 0)
        {
            string enemyText = $"{remaining} enemies";
            DrawTextSmall(enemyText, Constants.LogicalWidth - enemyText.Length * 5 - 8, 14, Color.Red);
        }

        // Dash cooldown indicator (below XP bar)
        int dashY = barY + barH + 3 + 6;
        float dashCD = Math.Max(0.15f, Constants.DashCooldown - state.Player.ComputedStats.DashCooldownReduction);
        if (state.Player.IsDashing)
        {
            DrawTextSmall("DASH", barX, dashY - 1, Color.White);
        }
        else if (state.Player.DashCooldownTimer > 0)
        {
            float dashPct = 1f - state.Player.DashCooldownTimer / dashCD;
            Raylib.DrawRectangle(barX - 1, dashY - 1, 42, 5, Color.Black);
            Raylib.DrawRectangle(barX, dashY, 40, 3, Color.DarkGray);
            Raylib.DrawRectangle(barX, dashY, (int)(40 * dashPct), 3, Color.Purple);
            DrawTextSmall("DASH", barX + 44, dashY - 1, Color.Gray);
        }
        else
        {
            DrawTextSmall("DASH [SPACE]", barX, dashY - 1, Color.Purple);
        }

        // Post-dash buff indicator
        if (state.Player.DashBuffTimer > 0)
        {
            int buffY = dashY + 8;
            float buffPct = state.Player.DashBuffTimer / Player.DashBuffDuration;
            Raylib.DrawRectangle(barX - 1, buffY - 1, 42, 5, Color.Black);
            Raylib.DrawRectangle(barX, buffY, 40, 3, Color.DarkGray);
            Raylib.DrawRectangle(barX, buffY, (int)(40 * buffPct), 3, Color.Gold);
            DrawTextSmall("BUFF", barX + 44, buffY - 1, Color.Gold);
        }

        // Weapon icons (bottom left)
        int weaponY = Constants.LogicalHeight - 28;
        for (int i = 0; i < state.EquippedWeapons.Count; i++)
        {
            var weapon = state.EquippedWeapons[i];
            int wx = 4 + i * 26;

            // Dim background if reloading
            bool isReloading = state.WeaponReloadTimers[i] > 0;
            Raylib.DrawRectangle(wx - 1, weaponY - 1, 26, 26, new Color(0, 0, 0, 150));

            Color weapTint = isReloading ? new Color((byte)100, (byte)100, (byte)100, (byte)255) : Color.White;
            state.Assets.Weapons.Draw(weapon.Def.SpriteIndex, wx + 1, weaponY + 1, weapTint);

            // Reload bar overlay
            if (isReloading)
            {
                float reloadPct = 1f - state.WeaponReloadTimers[i] / weapon.ReloadTime;
                Raylib.DrawRectangle(wx, weaponY + 22, (int)(24 * reloadPct), 2, Color.SkyBlue);
            }

            // Clip ammo counter
            if (weapon.ClipSize > 0)
            {
                string ammoText = isReloading ? "R" : $"{state.WeaponClipAmmo[i]}";
                Color ammoColor = isReloading ? Color.SkyBlue :
                    state.WeaponClipAmmo[i] <= weapon.ClipSize / 4 ? Color.Red : Color.White;
                Raylib.DrawText(ammoText, wx + 1, weaponY - 8, 6, ammoColor);
            }

            // Upgrade level indicator
            if (weapon.UpgradeLevel > 0)
            {
                string lvl = $"+{weapon.UpgradeLevel}";
                Raylib.DrawText(lvl, wx + 16, weaponY + 18, 6, Color.Gold);
            }
        }
    }

    public static void DrawTextSmall(string text, int x, int y, Color color)
    {
        Raylib.DrawText(text, x, y, 8, color);
    }

    public static void DrawTextMedium(string text, int x, int y, Color color)
    {
        Raylib.DrawText(text, x, y, 12, color);
    }

    public static void DrawTextLarge(string text, int x, int y, Color color)
    {
        Raylib.DrawText(text, x, y, 20, color);
    }

    public static bool DrawButton(string text, int x, int y, int w, int h, Color bgColor, bool selected = false)
    {
        var mouse = Display.ScreenToLogical(Raylib.GetMousePosition());

        bool hovered = mouse.X >= x && mouse.X <= x + w && mouse.Y >= y && mouse.Y <= y + h;
        bool clicked = hovered && Raylib.IsMouseButtonPressed(MouseButton.Left);

        bool highlighted = hovered || selected;
        Color bg = highlighted
            ? new Color((byte)Math.Min(255, bgColor.R + 30), (byte)Math.Min(255, bgColor.G + 30),
                (byte)Math.Min(255, bgColor.B + 30), bgColor.A)
            : bgColor;
        Raylib.DrawRectangle(x, y, w, h, bg);

        Color borderColor = selected ? Color.Gold : Color.White;
        Raylib.DrawRectangleLines(x, y, w, h, borderColor);

        int textW = text.Length * 5; // approximate
        DrawTextSmall(text, x + w / 2 - textW / 2, y + h / 2 - 4, Color.White);

        return clicked;
    }

    public static Vector2 GetLogicalMouse()
    {
        return Display.ScreenToLogical(Raylib.GetMousePosition());
    }
}
