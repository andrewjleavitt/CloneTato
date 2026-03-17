using System.Numerics;
using Raylib_cs;

namespace DesertEngine.UI;

public static class UIHelper
{
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

    public static bool DrawButton(string text, int x, int y, int w, int h, Color bgColor, int windowScale)
    {
        var mouse = Raylib.GetMousePosition();
        mouse.X /= windowScale;
        mouse.Y /= windowScale;

        bool hovered = mouse.X >= x && mouse.X <= x + w && mouse.Y >= y && mouse.Y <= y + h;
        bool clicked = hovered && Raylib.IsMouseButtonPressed(MouseButton.Left);

        Color bg = hovered
            ? new Color((byte)Math.Min(255, bgColor.R + 30), (byte)Math.Min(255, bgColor.G + 30),
                (byte)Math.Min(255, bgColor.B + 30), bgColor.A)
            : bgColor;
        Raylib.DrawRectangle(x, y, w, h, bg);
        Raylib.DrawRectangleLines(x, y, w, h, Color.White);

        int textW = text.Length * 5;
        DrawTextSmall(text, x + w / 2 - textW / 2, y + h / 2 - 4, Color.White);

        return clicked;
    }

    public static void DrawBar(int x, int y, int w, int h, float pct, Color fgColor, Color bgColor)
    {
        pct = Math.Clamp(pct, 0f, 1f);
        Raylib.DrawRectangle(x - 1, y - 1, w + 2, h + 2, Color.Black);
        Raylib.DrawRectangle(x, y, w, h, bgColor);
        Raylib.DrawRectangle(x, y, (int)(w * pct), h, fgColor);
    }
}
