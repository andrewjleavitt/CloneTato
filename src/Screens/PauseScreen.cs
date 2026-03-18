using CloneTato.Core;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class PauseScreen
{
    private int _selected;
    private const int ItemCount = 4;

    private static readonly string[] MenuItems = { "CONTINUE", "SETTINGS", "END RUN", "EXIT GAME" };
    private static readonly Color[] MenuColors =
    {
        new(60, 100, 60, 255),
        new(60, 70, 100, 255),
        new(100, 80, 40, 255),
        new(100, 60, 60, 255),
    };

    public void Reset() => _selected = 0;

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        int vDir = InputHelper.GetMenuVertical();
        if (vDir != 0)
            _selected = (_selected + vDir + ItemCount) % ItemCount;

        if (InputHelper.IsConfirmPressed())
            ActivateItem(_selected, state, manager);

        if (InputHelper.IsPausePressed() || InputHelper.IsCancelPressed())
            manager.Unpause();
    }

    private static void ActivateItem(int index, GameState state, GameStateManager manager)
    {
        switch (index)
        {
            case 0: // Continue
                manager.Unpause();
                break;
            case 1: // Settings
                manager.OpenSettings();
                break;
            case 2: // End Run
                manager.Unpause();
                manager.TransitionTo(GameScreen.GameOver);
                break;
            case 3: // Exit Game
                manager.QuitRequested = true;
                break;
        }
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.DrawRectangle(0, 0, Constants.LogicalWidth, Constants.LogicalHeight, new Color(0, 0, 0, 160));

        string title = "PAUSED";
        int titleW = Raylib.MeasureText(title, 24);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 80, 24, Color.White);

        int btnW = 120, btnH = 24;
        int btnX = Constants.LogicalWidth / 2 - btnW / 2;
        int startY = 130;

        for (int i = 0; i < ItemCount; i++)
        {
            int btnY = startY + i * 32;
            if (UIRenderer.DrawButton(MenuItems[i], btnX, btnY, btnW, btnH, MenuColors[i], _selected == i))
                ActivateItem(i, state, manager);
        }

        string hint = InputHelper.GamepadAvailable
            ? "D-Pad navigate, A select, Start unpause"
            : "Arrows navigate, Enter select, ESC unpause";
        int hintW = hint.Length * 5;
        UIRenderer.DrawTextSmall(hint, Constants.LogicalWidth / 2 - hintW / 2, Constants.LogicalHeight - 20, Color.Gray);
    }
}
