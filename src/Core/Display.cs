using System.Numerics;
using Raylib_cs;

namespace CloneTato.Core;

public static class Display
{
    public static float Scale { get; private set; } = Constants.WindowScale;
    public static float OffsetX { get; private set; }
    public static float OffsetY { get; private set; }
    public static Rectangle DestRect { get; private set; }

    public static void Update()
    {
        int screenW = Raylib.GetScreenWidth();
        int screenH = Raylib.GetScreenHeight();

        float scaleX = (float)screenW / Constants.LogicalWidth;
        float scaleY = (float)screenH / Constants.LogicalHeight;
        Scale = MathF.Min(scaleX, scaleY);

        float renderW = Constants.LogicalWidth * Scale;
        float renderH = Constants.LogicalHeight * Scale;
        OffsetX = (screenW - renderW) / 2f;
        OffsetY = (screenH - renderH) / 2f;

        DestRect = new Rectangle(OffsetX, OffsetY, renderW, renderH);
    }

    public static Vector2 ScreenToLogical(Vector2 screenPos)
    {
        return new Vector2(
            (screenPos.X - OffsetX) / Scale,
            (screenPos.Y - OffsetY) / Scale);
    }
}
