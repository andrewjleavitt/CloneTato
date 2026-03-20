using CloneTato.Core;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class CreditsScreen
{
    public void Update(float dt, GameState state, GameStateManager manager)
    {
        if (InputHelper.IsCancelPressed() || InputHelper.IsConfirmPressed())
        {
            manager.TransitionTo(GameScreen.MainMenu);
        }
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(30, 25, 20, 255));

        int cx = Constants.LogicalWidth / 2;
        int y = 40;

        // Title
        string title = "CREDITS";
        int titleW = Raylib.MeasureText(title, 20);
        Raylib.DrawText(title, cx - titleW / 2, y, 20, Color.Gold);
        y += 40;

        // Game
        DrawCentered("CLONETATO", cx, y, 12, Color.Orange);
        y += 18;
        DrawCentered("Desert Survivor", cx, y, 8, Color.White);
        y += 30;

        // Author
        DrawCentered("-- CREATED BY --", cx, y, 10, Color.Gold);
        y += 18;
        DrawCentered("Andy Leavitt", cx, y, 10, Color.White);
        y += 14;
        DrawCentered("Game Design, Programming", cx, y, 8, Color.Gray);
        y += 30;

        // Art
        DrawCentered("-- ART --", cx, y, 10, Color.Gold);
        y += 18;
        DrawCentered("STRANDED Art Pack by Penusbmic", cx, y, 8, Color.White);
        y += 14;
        DrawCentered("Characters, enemies, terrain, UI, and bosses", cx, y, 8, Color.Gray);
        y += 20;
        DrawCentered("Desert Shooter Pack by Kenney", cx, y, 8, Color.White);
        y += 14;
        DrawCentered("Tilemaps, weapons, and sound effects", cx, y, 8, Color.Gray);
        y += 30;

        // Tech
        DrawCentered("-- TECH --", cx, y, 10, Color.Gold);
        y += 18;
        DrawCentered("Built with Raylib + C# (.NET 9)", cx, y, 8, Color.White);
        y += 14;
        DrawCentered("Raylib-cs bindings by ChrisDill", cx, y, 8, Color.Gray);
        y += 30;

        // Back hint
        string hint = InputHelper.GamepadAvailable
            ? "Press B to return"
            : "Press ESC to return";
        UIRenderer.DrawTextSmall(hint, cx - hint.Length * 5 / 2, Constants.LogicalHeight - 20, Color.Gray);
    }

    private static void DrawCentered(string text, int cx, int y, int fontSize, Color color)
    {
        int w = Raylib.MeasureText(text, fontSize);
        Raylib.DrawText(text, cx - w / 2, y, fontSize, color);
    }
}
