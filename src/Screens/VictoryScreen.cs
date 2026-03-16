using CloneTato.Core;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class VictoryScreen
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
        Raylib.ClearBackground(new Color(20, 30, 15, 255));

        string title = "VICTORY!";
        int titleW = Raylib.MeasureText(title, 24);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 40, 24, Color.Gold);

        int cy = 80;
        int cx = Constants.LogicalWidth / 2 - 60;
        UIRenderer.DrawTextSmall($"All {Constants.MaxWaves} waves survived!", cx, cy, Color.Green);
        UIRenderer.DrawTextSmall($"Enemies Killed: {state.TotalEnemiesKilled}", cx, cy + 16, Color.White);
        UIRenderer.DrawTextSmall($"Damage Dealt: {state.TotalDamageDealt}", cx, cy + 30, Color.White);
        UIRenderer.DrawTextSmall($"Time Survived: {state.TotalTimeSurvived:F1}s", cx, cy + 44, Color.White);
        UIRenderer.DrawTextSmall($"Final Level: {state.Level}", cx, cy + 58, Color.SkyBlue);
        UIRenderer.DrawTextSmall($"Weapons: {state.EquippedWeapons.Count}", cx, cy + 72, Color.Orange);

        int btnW = 80, btnH = 18;
        if (UIRenderer.DrawButton("PLAY AGAIN", Constants.LogicalWidth / 2 - btnW / 2, 195, btnW, btnH,
            new Color(60, 100, 60, 255)))
        {
            manager.TransitionTo(GameScreen.CharacterSelect);
        }

        if (UIRenderer.DrawButton("MENU", Constants.LogicalWidth / 2 - btnW / 2, 220, btnW, btnH,
            new Color(100, 60, 60, 255)))
        {
            manager.TransitionTo(GameScreen.MainMenu);
        }

        UIRenderer.DrawTextSmall("Enter = Play Again, Esc = Menu", Constants.LogicalWidth / 2 - 65,
            Constants.LogicalHeight - 15, Color.Gray);
    }
}
