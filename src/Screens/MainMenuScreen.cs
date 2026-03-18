using CloneTato.Core;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class MainMenuScreen
{
    private int _selected;
    private const int ItemCount = 5;

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        int vDir = InputHelper.GetMenuVertical();
        if (vDir != 0)
            _selected = (_selected + vDir + ItemCount) % ItemCount;

        if (InputHelper.IsConfirmPressed())
            ActivateItem(_selected, state, manager);

        if (InputHelper.IsCancelPressed())
            manager.QuitRequested = true;
    }

    private static void ActivateItem(int index, GameState state, GameStateManager manager)
    {
        switch (index)
        {
            case 0:
                manager.TransitionTo(GameScreen.CharacterSelect);
                state.Assets.PlaySoundVariant("select", 0.5f);
                break;
            case 1:
                manager.TransitionTo(GameScreen.MetaUpgrades);
                state.Assets.PlaySoundVariant("select", 0.5f);
                break;
            case 2:
                manager.TransitionTo(GameScreen.WeaponGallery);
                state.Assets.PlaySoundVariant("select", 0.5f);
                break;
            case 3:
                manager.OpenSettings();
                state.Assets.PlaySoundVariant("select", 0.5f);
                break;
            case 4:
                manager.QuitRequested = true;
                break;
        }
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(45, 30, 20, 255));

        // Draw desert tile background
        for (int x = 0; x < Constants.LogicalWidth; x += Constants.TileSize)
        {
            for (int y = 0; y < Constants.LogicalHeight; y += Constants.TileSize)
            {
                int tileIdx = ((x / Constants.TileSize) + (y / Constants.TileSize)) % 3;
                state.Assets.Tiles.Draw(tileIdx, x, y, new Color(60, 40, 30, 255));
            }
        }

        // Title
        string title = "CLONETATO";
        int titleW = Raylib.MeasureText(title, 24);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2 + 1, 41, 24, Color.Black);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 40, 24, Color.Gold);

        string subtitle = "Desert Survivor";
        int subW = Raylib.MeasureText(subtitle, 10);
        Raylib.DrawText(subtitle, Constants.LogicalWidth / 2 - subW / 2, 68, 10, Color.Orange);

        // Character sprites as decoration
        for (int i = 0; i < 4; i++)
        {
            state.Assets.Players.DrawScaled(i * 4, Constants.LogicalWidth / 2 - 50 + i * 30, 100, 1.5f, Color.White);
        }

        // Buttons
        int btnW = 80, btnH = 18;
        int btnX = Constants.LogicalWidth / 2 - btnW / 2;
        int startY = 120;
        int spacing = 22;

        string[] labels = { "START RUN", "UPGRADES", "WEAPONS", "SETTINGS", "QUIT" };
        Color[] colors =
        {
            new(60, 100, 60, 255),
            new(70, 50, 90, 255),
            new(80, 70, 50, 255),
            new(60, 70, 100, 255),
            new(100, 60, 60, 255),
        };

        for (int i = 0; i < ItemCount; i++)
        {
            if (UIRenderer.DrawButton(labels[i], btnX, startY + i * spacing, btnW, btnH, colors[i], _selected == i))
                ActivateItem(i, state, manager);
        }

        string hint = InputHelper.GamepadAvailable
            ? "D-Pad navigate, A select"
            : "WASD/Arrows navigate, Enter select";
        UIRenderer.DrawTextSmall(hint,
            Constants.LogicalWidth / 2 - hint.Length * 5 / 2, Constants.LogicalHeight - 15, Color.Gray);
    }
}
