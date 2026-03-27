using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class GameOverScreen
{
    private bool _tokensAwarded;
    private int _tokensEarned;
    private int _selected; // 0=retry, 1=menu

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        if (!_tokensAwarded)
        {
            _tokensEarned = manager.Meta.CalculateRunTokens(state.CurrentWave, state.TotalEnemiesKilled, false);
            manager.Meta.Tokens += _tokensEarned;
            manager.Meta.TotalRuns++;
            manager.Meta.TotalKills += state.TotalEnemiesKilled;
            if (state.CurrentWave > manager.Meta.BestWave)
                manager.Meta.BestWave = state.CurrentWave;
            manager.Meta.CheckUnlocks();
            manager.Meta.Save();
            _tokensAwarded = true;
            _selected = 0;
        }

        int vDir = InputHelper.GetMenuVertical();
        if (vDir != 0)
            _selected = (_selected + vDir + 2) % 2;

        if (InputHelper.IsConfirmPressed())
        {
            if (_selected == 0)
            {
                _tokensAwarded = false;
                manager.TransitionTo(GameScreen.CharacterSelect);
            }
            else
            {
                _tokensAwarded = false;
                manager.TransitionTo(GameScreen.MainMenu);
            }
        }

        if (InputHelper.IsCancelPressed())
        {
            _tokensAwarded = false;
            manager.TransitionTo(GameScreen.MainMenu);
        }
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(30, 15, 15, 255));

        string title = "GAME OVER";
        int titleW = Raylib.MeasureText(title, 28);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 40, 28, Color.Red);

        var stats = state.Player.ComputedStats;
        int leftX = 60;
        int rightX = Constants.LogicalWidth / 2 + 40;

        // Left column: run summary
        int y = 80;
        UIRenderer.DrawTextSmall("-- RUN --", leftX, y, Color.Gold);
        y += 12;
        UIRenderer.DrawTextSmall($"Wave: {state.CurrentWave}/{Constants.WavesPerBiome}", leftX, y, Color.White); y += 10;
        UIRenderer.DrawTextSmall($"Level: {state.Level}", leftX, y, Color.SkyBlue); y += 10;
        UIRenderer.DrawTextSmall($"Kills: {state.TotalEnemiesKilled}", leftX, y, Color.White); y += 10;
        UIRenderer.DrawTextSmall($"Damage: {state.TotalDamageDealt}", leftX, y, Color.White); y += 10;
        UIRenderer.DrawTextSmall($"Time: {state.TotalTimeSurvived:F1}s", leftX, y, Color.White); y += 10;
        UIRenderer.DrawTextSmall($"Gold: {state.Gold}", leftX, y, Color.Gold); y += 10;
        UIRenderer.DrawTextSmall($"Tokens: +{_tokensEarned}", leftX, y, Color.Gold); y += 14;

        // Weapons
        UIRenderer.DrawTextSmall("-- WEAPONS --", leftX, y, Color.Gold);
        y += 12;
        for (int i = 0; i < state.EquippedWeapons.Count; i++)
        {
            var w = state.EquippedWeapons[i];
            string lvl = w.UpgradeLevel > 0 ? $" +{w.UpgradeLevel}" : "";
            UIRenderer.DrawTextSmall($"{w.Def.Name}{lvl}", leftX, y, Color.White);
            y += 10;
        }

        // Right column: all stats
        y = 80;
        UIRenderer.DrawTextSmall("-- STATS --", rightX, y, Color.Gold);
        y += 12;

        DrawStat("Max HP", $"{stats.MaxHP}", rightX, ref y, Color.Green);
        DrawStat("Move Spd", $"{stats.MoveSpeed:F0}", rightX, ref y, Color.White);
        DrawStat("Damage", $"{stats.DamageMultiplier:F2}x", rightX, ref y, Color.Orange);
        DrawStat("Atk Spd", $"{stats.AttackSpeedMultiplier:F2}x", rightX, ref y, Color.Orange);
        DrawStat("Crit", $"{stats.CritChance * 100:F0}% ({stats.CritDamage:F1}x)", rightX, ref y, Color.Yellow);
        DrawStat("Armor", $"{stats.Armor}", rightX, ref y, Color.LightGray);
        DrawStat("Dodge", $"{stats.DodgeChance * 100:F0}%", rightX, ref y, Color.SkyBlue);
        DrawStat("Pickup", $"{stats.PickupRange:F0}", rightX, ref y, Color.Lime);
        DrawStat("XP Mult", $"{stats.XPMultiplier:F2}x", rightX, ref y, Color.SkyBlue);
        // Dash stats (only show if non-default)
        if (stats.DashCooldownReduction > 0)
            DrawStat("Dash CD", $"-{stats.DashCooldownReduction:F2}s", rightX, ref y, Color.Purple);
        if (stats.PostDashAttackSpeed > 0)
            DrawStat("Post-Dash AS", $"+{stats.PostDashAttackSpeed * 100:F0}%", rightX, ref y, Color.Gold);
        if (stats.PostDashMoveSpeed > 0)
            DrawStat("Post-Dash MS", $"+{stats.PostDashMoveSpeed * 100:F0}%", rightX, ref y, Color.Gold);
        if (stats.PostDashInvuln > 0)
            DrawStat("Post-Dash Inv", $"{stats.PostDashInvuln:F1}s", rightX, ref y, Color.Gold);

        // Buttons
        int btnW = 100, btnH = 22;
        if (UIRenderer.DrawButton("RETRY", Constants.LogicalWidth / 2 - btnW / 2, 310, btnW, btnH,
            new Color(60, 100, 60, 255), _selected == 0))
        {
            _tokensAwarded = false;
            manager.TransitionTo(GameScreen.CharacterSelect);
        }

        if (UIRenderer.DrawButton("MENU", Constants.LogicalWidth / 2 - btnW / 2, 336, btnW, btnH,
            new Color(100, 60, 60, 255), _selected == 1))
        {
            _tokensAwarded = false;
            manager.TransitionTo(GameScreen.MainMenu);
        }
    }

    private static void DrawStat(string label, string value, int x, ref int y, Color color)
    {
        UIRenderer.DrawTextSmall(label, x, y, Color.Gray);
        UIRenderer.DrawTextSmall(value, x + 75, y, color);
        y += 10;
    }
}
