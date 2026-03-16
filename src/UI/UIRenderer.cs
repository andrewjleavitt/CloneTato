using System.Numerics;
using CloneTato.Assets;
using CloneTato.Core;
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

        // Weapon icons (bottom left)
        int weaponY = Constants.LogicalHeight - 28;
        for (int i = 0; i < state.EquippedWeapons.Count; i++)
        {
            int wx = 4 + i * 26;
            Raylib.DrawRectangle(wx - 1, weaponY - 1, 26, 26, new Color(0, 0, 0, 150));
            state.Assets.Weapons.Draw(state.EquippedWeapons[i].SpriteIndex, wx + 1, weaponY + 1, Color.White);
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

    public static bool DrawButton(string text, int x, int y, int w, int h, Color bgColor)
    {
        // Convert mouse from window coords to logical coords
        var mouse = Raylib.GetMousePosition();
        mouse.X /= Constants.WindowScale;
        mouse.Y /= Constants.WindowScale;

        bool hovered = mouse.X >= x && mouse.X <= x + w && mouse.Y >= y && mouse.Y <= y + h;
        bool clicked = hovered && Raylib.IsMouseButtonPressed(MouseButton.Left);

        Color bg = hovered ? new Color(bgColor.R + 30, bgColor.G + 30, bgColor.B + 30, bgColor.A) : bgColor;
        Raylib.DrawRectangle(x, y, w, h, bg);
        Raylib.DrawRectangleLines(x, y, w, h, Color.White);

        int textW = text.Length * 5; // approximate
        DrawTextSmall(text, x + w / 2 - textW / 2, y + h / 2 - 4, Color.White);

        return clicked;
    }
}
