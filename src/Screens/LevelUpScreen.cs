using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class LevelUpScreen
{
    private readonly StatUpgrade[] _choices = new StatUpgrade[3];
    private bool _initialized;
    private int _selected;

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
        new("Reload +15%", "+15% Reload Speed", new Stats { ReloadSpeedMultiplier = 0.15f }),
        new("Reload +25%", "+25% Reload Speed", new Stats { ReloadSpeedMultiplier = 0.25f }),
        new("Dash Speed", "+80 Dash Speed", new Stats { DashSpeedBonus = 80f }),
        new("Dash Cooldown", "-0.12s Dash Cooldown", new Stats { DashCooldownReduction = 0.12f }),
        new("Long Dash", "+0.06s Dash Duration", new Stats { DashDurationBonus = 0.06f }),
        new("Dash: Atk Speed", "30% atk speed after dash", new Stats { PostDashAttackSpeed = 0.3f }),
        new("Dash: Move", "25% move speed after dash", new Stats { PostDashMoveSpeed = 0.25f }),
        new("Dash: Invuln", "0.5s invuln after dash", new Stats { PostDashInvuln = 0.5f }),
    };

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        if (!_initialized)
        {
            GenerateChoices();
            _initialized = true;
            _selected = 0;
        }

        int hDir = InputHelper.GetMenuHorizontal();
        if (hDir != 0)
            _selected = (_selected + hDir + 3) % 3;

        if (InputHelper.IsConfirmPressed())
            ChooseUpgrade(state, manager, _selected);
    }

    private void ChooseUpgrade(GameState state, GameStateManager manager, int index)
    {
        state.LevelBonus = state.LevelBonus + _choices[index].Bonus;
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

        if (!state.LevelUpPending)
            manager.TransitionTo(GameScreen.Playing);
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
        int titleW = Raylib.MeasureText(title, 16);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 80, 16, Color.Gold);

        UIRenderer.DrawTextSmall("Choose an upgrade:", Constants.LogicalWidth / 2 - 40, 105, Color.White);

        int cardW = 150, cardH = 50;
        int totalW = 3 * (cardW + 12) - 12;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 120;

        var mouse = Display.ScreenToLogical(Raylib.GetMousePosition());

        for (int i = 0; i < 3; i++)
        {
            int cx = startX + i * (cardW + 12);
            bool hovered = mouse.X >= cx && mouse.X <= cx + cardW && mouse.Y >= cardY && mouse.Y <= cardY + cardH;
            bool selected = _selected == i;

            if (hovered) _selected = i;

            Color bg = (hovered || selected) ? new Color(80, 60, 40, 255) : new Color(50, 35, 25, 255);
            Raylib.DrawRectangle(cx, cardY, cardW, cardH, bg);
            Raylib.DrawRectangleLines(cx, cardY, cardW, cardH, (hovered || selected) ? Color.Gold : Color.Gray);

            UIRenderer.DrawTextSmall(_choices[i].Name, cx + 6, cardY + 8, Color.White);
            UIRenderer.DrawTextSmall(_choices[i].Description, cx + 6, cardY + 22, Color.LightGray);

            if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
                ChooseUpgrade(state, manager, i);
        }

        string hint = InputHelper.GamepadAvailable
            ? "D-Pad select, A confirm"
            : "Left/Right select, Enter confirm";
        int hintW = hint.Length * 5;
        UIRenderer.DrawTextSmall(hint, Constants.LogicalWidth / 2 - hintW / 2, cardY + cardH + 10, Color.Gray);
    }
}
