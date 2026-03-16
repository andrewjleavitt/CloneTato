using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class LevelUpScreen
{
    private readonly StatUpgrade[] _choices = new StatUpgrade[3];
    private bool _initialized;

    private record StatUpgrade(string Name, string Description, Stats Bonus);

    private static readonly StatUpgrade[] AllUpgrades =
    {
        new("Max HP +15", "+15 Maximum HP", new Stats { MaxHP = 15 }),
        new("Max HP +25", "+25 Maximum HP", new Stats { MaxHP = 25 }),
        new("Speed +10", "+10 Move Speed", new Stats { MoveSpeed = 10f }),
        new("Speed +18", "+18 Move Speed", new Stats { MoveSpeed = 18f }),
        new("Damage +8%", "+8% Damage", new Stats { DamageMultiplier = 0.08f }),
        new("Damage +15%", "+15% Damage", new Stats { DamageMultiplier = 0.15f }),
        new("Atk Speed +10%", "+10% Attack Speed", new Stats { AttackSpeedMultiplier = 0.10f }),
        new("Atk Speed +18%", "+18% Attack Speed", new Stats { AttackSpeedMultiplier = 0.18f }),
        new("Armor +2", "+2 Armor", new Stats { Armor = 2 }),
        new("Armor +4", "+4 Armor", new Stats { Armor = 4 }),
        new("Dodge +5%", "+5% Dodge Chance", new Stats { DodgeChance = 0.05f }),
        new("Crit +5%", "+5% Crit Chance", new Stats { CritChance = 0.05f }),
        new("Pickup +15", "+15 Pickup Range", new Stats { PickupRange = 15f }),
        new("XP +10%", "+10% XP Gain", new Stats { XPMultiplier = 0.10f }),
    };

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        if (!_initialized)
        {
            GenerateChoices();
            _initialized = true;
        }
    }

    private void GenerateChoices()
    {
        var indices = Enumerable.Range(0, AllUpgrades.Length).ToList();
        for (int i = 0; i < 3; i++)
        {
            int pick = Random.Shared.Next(indices.Count);
            _choices[i] = AllUpgrades[indices[pick]];
            indices.RemoveAt(pick);
        }
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        // Semi-transparent overlay
        Raylib.DrawRectangle(0, 0, Constants.LogicalWidth, Constants.LogicalHeight, new Color(0, 0, 0, 180));

        string title = $"LEVEL UP! (Lv {state.Level})";
        int titleW = Raylib.MeasureText(title, 14);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 50, 14, Color.Gold);

        UIRenderer.DrawTextSmall("Choose an upgrade:", Constants.LogicalWidth / 2 - 40, 70, Color.White);

        int cardW = 120, cardH = 40;
        int totalW = 3 * (cardW + 10) - 10;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 90;

        var mouse = Raylib.GetMousePosition();
        mouse.X /= Constants.WindowScale;
        mouse.Y /= Constants.WindowScale;

        for (int i = 0; i < 3; i++)
        {
            int cx = startX + i * (cardW + 10);
            bool hovered = mouse.X >= cx && mouse.X <= cx + cardW && mouse.Y >= cardY && mouse.Y <= cardY + cardH;

            Color bg = hovered ? new Color(80, 60, 40, 255) : new Color(50, 35, 25, 255);
            Raylib.DrawRectangle(cx, cardY, cardW, cardH, bg);
            Raylib.DrawRectangleLines(cx, cardY, cardW, cardH, hovered ? Color.Gold : Color.Gray);

            UIRenderer.DrawTextSmall(_choices[i].Name, cx + 6, cardY + 8, Color.White);
            UIRenderer.DrawTextSmall(_choices[i].Description, cx + 6, cardY + 22, Color.LightGray);

            if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                state.LevelBonus = state.LevelBonus + _choices[i].Bonus;
                state.RecomputePlayerStats();
                state.PendingLevelUps--;
                if (state.PendingLevelUps <= 0)
                    state.LevelUpPending = false;
                _initialized = false;

                // Heal a bit on level up
                state.Player.CurrentHP = Math.Min(
                    state.Player.CurrentHP + 5,
                    state.Player.ComputedStats.MaxHP);

                state.Assets.PlaySoundVariant("select", 0.5f);

                // If no more pending level ups, go back to playing
                if (!state.LevelUpPending)
                {
                    manager.TransitionTo(GameScreen.Playing);
                }
            }
        }
    }
}
