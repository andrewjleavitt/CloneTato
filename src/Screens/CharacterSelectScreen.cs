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

        int hDir = InputHelper.GetMenuHorizontal();
        if (hDir != 0)
            _selectedIndex = Math.Clamp(_selectedIndex + hDir, 0, CharacterDatabase.Characters.Length - 1);
        if (InputHelper.IsConfirmPressed())
            StartRun(state, manager);
        if (InputHelper.IsCancelPressed())
            manager.TransitionTo(GameScreen.MainMenu);
    }

    private void StartRun(GameState state, GameStateManager manager)
    {
        if (!manager.Meta.CharacterUnlocked[_selectedIndex]) return;
        var character = CharacterDatabase.Characters[_selectedIndex];
        state.StartNewRun(character);
        state.MetaBonus = manager.Meta.GetMetaBonus();
        state.RecomputePlayerStats();
        state.StartWave();
        manager.TransitionTo(GameScreen.Playing);
        state.Assets.PlaySoundVariant("select", 0.5f);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(35, 25, 20, 255));

        string title = "SELECT CHARACTER";
        int titleW = Raylib.MeasureText(title, 18);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 20, 18, Color.Gold);

        // Character cards
        int cardW = 110, cardH = 140;
        int totalW = CharacterDatabase.Characters.Length * (cardW + 10) - 10;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 50;

        var mouse = Display.ScreenToLogical(Raylib.GetMousePosition());

        for (int i = 0; i < CharacterDatabase.Characters.Length; i++)
        {
            var ch = CharacterDatabase.Characters[i];
            int cx = startX + i * (cardW + 10);
            bool selected = i == _selectedIndex;
            bool unlocked = manager.Meta.CharacterUnlocked.Length > i && manager.Meta.CharacterUnlocked[i];

            Color cardBg = !unlocked ? new Color(30, 25, 20, 255) :
                selected ? new Color(80, 60, 40, 255) : new Color(50, 35, 25, 255);
            Raylib.DrawRectangle(cx, cardY, cardW, cardH, cardBg);
            Color borderColor = !unlocked ? new Color(60, 50, 40, 255) :
                selected ? Color.Gold : Color.Gray;
            Raylib.DrawRectangleLines(cx, cardY, cardW, cardH, borderColor);

            if (unlocked)
            {
                state.Assets.Players.DrawScaled(ch.SpriteIndex, cx + cardW / 2, cardY + 32, 2f, Color.White);
                UIRenderer.DrawTextSmall(ch.Name, cx + 4, cardY + 56, Color.White);
                UIRenderer.DrawTextSmall(ch.Description, cx + 4, cardY + 68, Color.LightGray);

                var s = ch.BaseStats;
                UIRenderer.DrawTextSmall($"HP: {s.MaxHP}", cx + 4, cardY + 82, Color.Green);
                UIRenderer.DrawTextSmall($"SPD: {s.MoveSpeed:F0}", cx + 4, cardY + 94, Color.SkyBlue);
                UIRenderer.DrawTextSmall($"ARM: {s.Armor}", cx + 4, cardY + 106, Color.Orange);
                UIRenderer.DrawTextSmall($"DDG: {s.DodgeChance * 100:F0}%", cx + 4, cardY + 118, Color.Yellow);
            }
            else
            {
                state.Assets.Players.DrawScaled(ch.SpriteIndex, cx + cardW / 2, cardY + 32, 2f,
                    new Color((byte)30, (byte)30, (byte)30, (byte)255));
                UIRenderer.DrawTextSmall("LOCKED", cx + cardW / 2 - 14, cardY + 60, Color.Gray);
                string hint = i switch
                {
                    1 => "Reach wave 5",
                    2 => "Kill 200 enemies",
                    3 => "Reach wave 10",
                    _ => "???",
                };
                UIRenderer.DrawTextSmall(hint, cx + 4, cardY + 80, new Color(120, 100, 80, 255));
            }

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

        int btnW = 100, btnH = 22;
        if (UIRenderer.DrawButton("START", Constants.LogicalWidth / 2 - btnW / 2,
            Constants.LogicalHeight - 45, btnW, btnH, new Color(60, 100, 60, 255)))
        {
            StartRun(state, manager);
        }

        UIRenderer.DrawTextSmall("Arrow keys to select, Enter to start",
            Constants.LogicalWidth / 2 - 90, Constants.LogicalHeight - 12, Color.Gray);
    }
}
