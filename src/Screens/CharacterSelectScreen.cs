using CloneTato.Assets;
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
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 15, 18, Color.Gold);

        // Character cards
        int cardW = 160, cardH = 200;
        int totalW = CharacterDatabase.Characters.Length * (cardW + 12) - 12;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 42;

        var mouse = Display.ScreenToLogical(Raylib.GetMousePosition());

        for (int i = 0; i < CharacterDatabase.Characters.Length; i++)
        {
            var ch = CharacterDatabase.Characters[i];
            int cx = startX + i * (cardW + 12);
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
                // Draw animated hero sprite preview
                var heroSprite = GetHeroPreviewSprite(ch.HeroType, state.Assets);
                float previewX = cx + cardW / 2f;
                float previewY = cardY + 60;

                if (heroSprite != null)
                {
                    int animFrame = ((int)(Raylib.GetTime() * 6)) % heroSprite.GetFrameCount("idle_down");
                    heroSprite.DrawAnimationFrame("idle_down", animFrame, false,
                        previewX, previewY, Color.White, 2f);
                }
                else
                {
                    state.Assets.Players.DrawScaled(ch.SpriteIndex, (int)previewX, (int)previewY, 2f, Color.White);
                }

                // Draw companion for Drifter
                if (ch.HeroType == HeroType.Drifter && state.Assets.CompanionSprite != null)
                {
                    int compFrame = ((int)(Raylib.GetTime() * 5)) % state.Assets.CompanionSprite.GetFrameCount("idle");
                    state.Assets.CompanionSprite.DrawAnimationFrame("idle", compFrame, false,
                        previewX + 24, previewY - 16, Color.White, 1.5f);
                }

                // Name
                int nameW = Raylib.MeasureText(ch.Name, 12);
                Raylib.DrawText(ch.Name, cx + cardW / 2 - nameW / 2, cardY + 96, 12, Color.White);

                // Description
                UIRenderer.DrawTextSmall(ch.Description, cx + 6, cardY + 114, Color.LightGray);

                // Stats
                var s = ch.BaseStats;
                UIRenderer.DrawTextSmall($"HP: {s.MaxHP}", cx + 6, cardY + 130, Color.Green);
                UIRenderer.DrawTextSmall($"SPD: {s.MoveSpeed:F0}", cx + 6, cardY + 142, Color.SkyBlue);
                UIRenderer.DrawTextSmall($"DMG: {s.DamageMultiplier:P0}", cx + 6, cardY + 154, Color.Orange);

                // Special trait
                string trait = ch.HeroType switch
                {
                    HeroType.BladeDancer => "+40% melee, -30% ranged",
                    HeroType.Drifter => "+50% damage, -40% HP, companion",
                    _ => "Balanced all-rounder",
                };
                UIRenderer.DrawTextSmall(trait, cx + 6, cardY + 170, Color.Gold);
            }
            else
            {
                // Locked character — show silhouette
                var heroSprite = GetHeroPreviewSprite(ch.HeroType, state.Assets);
                if (heroSprite != null)
                {
                    int animFrame = ((int)(Raylib.GetTime() * 4)) % heroSprite.GetFrameCount("idle_down");
                    heroSprite.DrawAnimationFrame("idle_down", animFrame, false,
                        cx + cardW / 2f, cardY + 60, new Color((byte)20, (byte)20, (byte)20, (byte)255), 2f);
                }

                int lockW = Raylib.MeasureText("LOCKED", 12);
                Raylib.DrawText("LOCKED", cx + cardW / 2 - lockW / 2, cardY + 100, 12, Color.Gray);

                string hint = i switch
                {
                    1 => "Beat wave 10",
                    2 => "Beat wave 20",
                    _ => "???",
                };
                UIRenderer.DrawTextSmall(hint, cx + 6, cardY + 120, new Color(120, 100, 80, 255));
            }

            // Click to select or confirm
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
            Constants.LogicalHeight - 40, btnW, btnH, new Color(60, 100, 60, 255)))
        {
            StartRun(state, manager);
        }

        string controlHint = InputHelper.GamepadAvailable
            ? "D-Pad select, A start, B back"
            : "Arrow keys select, Enter start, Esc back";
        UIRenderer.DrawTextSmall(controlHint,
            Constants.LogicalWidth / 2 - controlHint.Length * 5 / 2, Constants.LogicalHeight - 12, Color.Gray);
    }

    private static AnimatedSprite? GetHeroPreviewSprite(HeroType type, AssetManager assets)
    {
        return type switch
        {
            HeroType.BladeDancer => assets.HeroSwordSprite,
            HeroType.Drifter => assets.StarterHeroSprite,
            _ => assets.HeroGunSprite,
        };
    }
}
