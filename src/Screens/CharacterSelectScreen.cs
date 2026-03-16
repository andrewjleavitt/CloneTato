using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class CharacterSelectScreen
{
    private int _selectedIndex;
    private GameStateManager? _manager;

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        _manager = manager;

        if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A))
            _selectedIndex = Math.Max(0, _selectedIndex - 1);
        if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D))
            _selectedIndex = Math.Min(CharacterDatabase.Characters.Length - 1, _selectedIndex + 1);
        if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space))
            StartRun(state, manager);
        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
            manager.TransitionTo(GameScreen.MainMenu);
    }

    private void StartRun(GameState state, GameStateManager manager)
    {
        var character = CharacterDatabase.Characters[_selectedIndex];
        state.StartNewRun(character);
        state.StartWave();
        manager.TransitionTo(GameScreen.Playing);
        state.Assets.PlaySoundVariant("select", 0.5f);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(35, 25, 20, 255));

        string title = "SELECT CHARACTER";
        int titleW = Raylib.MeasureText(title, 14);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 15, 14, Color.Gold);

        // Character cards
        int cardW = 90, cardH = 110;
        int totalW = CharacterDatabase.Characters.Length * (cardW + 8) - 8;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 40;

        var mouse = Raylib.GetMousePosition();
        mouse.X /= Constants.WindowScale;
        mouse.Y /= Constants.WindowScale;

        for (int i = 0; i < CharacterDatabase.Characters.Length; i++)
        {
            var ch = CharacterDatabase.Characters[i];
            int cx = startX + i * (cardW + 8);
            bool selected = i == _selectedIndex;

            Color cardBg = selected ? new Color(80, 60, 40, 255) : new Color(50, 35, 25, 255);
            Raylib.DrawRectangle(cx, cardY, cardW, cardH, cardBg);
            Color borderColor = selected ? Color.Gold : Color.Gray;
            Raylib.DrawRectangleLines(cx, cardY, cardW, cardH, borderColor);

            state.Assets.Players.DrawScaled(ch.SpriteIndex, cx + cardW / 2, cardY + 28, 2f, Color.White);

            UIRenderer.DrawTextSmall(ch.Name, cx + 4, cardY + 48, Color.White);
            UIRenderer.DrawTextSmall(ch.Description, cx + 4, cardY + 58, Color.LightGray);

            var s = ch.BaseStats;
            UIRenderer.DrawTextSmall($"HP: {s.MaxHP}", cx + 4, cardY + 70, Color.Green);
            UIRenderer.DrawTextSmall($"SPD: {s.MoveSpeed:F0}", cx + 4, cardY + 80, Color.SkyBlue);
            UIRenderer.DrawTextSmall($"ARM: {s.Armor}", cx + 4, cardY + 90, Color.Orange);
            UIRenderer.DrawTextSmall($"DDG: {s.DodgeChance * 100:F0}%", cx + 4, cardY + 100, Color.Yellow);

            // Click to select or double-click to start
            if (mouse.X >= cx && mouse.X <= cx + cardW && mouse.Y >= cardY && mouse.Y <= cardY + cardH)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    if (_selectedIndex == i)
                        StartRun(state, manager);
                    else
                        _selectedIndex = i;
                }
            }
        }

        int btnW = 80, btnH = 18;
        if (UIRenderer.DrawButton("START", Constants.LogicalWidth / 2 - btnW / 2,
            Constants.LogicalHeight - 35, btnW, btnH, new Color(60, 100, 60, 255)))
        {
            StartRun(state, manager);
        }

        UIRenderer.DrawTextSmall("Arrow keys to select, Enter to start",
            Constants.LogicalWidth / 2 - 90, Constants.LogicalHeight - 12, Color.Gray);
    }
}
