using CloneTato.Core;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class GameOverScreen
{
    public void Update(float dt, GameState state, GameStateManager manager)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space))
            manager.TransitionTo(GameScreen.CharacterSelect);
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            manager.TransitionTo(GameScreen.MainMenu);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(30, 15, 15, 255));

        string title = "GAME OVER";
        int titleW = Raylib.MeasureText(title, 24);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 40, 24, Color.Red);

        int cy = 80;
        int cx = Constants.LogicalWidth / 2 - 60;
        UIRenderer.DrawTextSmall($"Wave Reached: {state.CurrentWave}/{Constants.MaxWaves}", cx, cy, Color.White);
        UIRenderer.DrawTextSmall($"Enemies Killed: {state.TotalEnemiesKilled}", cx, cy + 14, Color.White);
        UIRenderer.DrawTextSmall($"Damage Dealt: {state.TotalDamageDealt}", cx, cy + 28, Color.White);
        UIRenderer.DrawTextSmall($"Time Survived: {state.TotalTimeSurvived:F1}s", cx, cy + 42, Color.White);
        UIRenderer.DrawTextSmall($"Level: {state.Level}", cx, cy + 56, Color.SkyBlue);

        int btnW = 80, btnH = 18;
        if (UIRenderer.DrawButton("RETRY", Constants.LogicalWidth / 2 - btnW / 2, 190, btnW, btnH,
            new Color(60, 100, 60, 255)))
        {
            manager.TransitionTo(GameScreen.CharacterSelect);
        }

        if (UIRenderer.DrawButton("MENU", Constants.LogicalWidth / 2 - btnW / 2, 215, btnW, btnH,
            new Color(100, 60, 60, 255)))
        {
            manager.TransitionTo(GameScreen.MainMenu);
        }

        UIRenderer.DrawTextSmall("Enter = Retry, Esc = Menu", Constants.LogicalWidth / 2 - 55,
            Constants.LogicalHeight - 15, Color.Gray);
    }
}
