using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class MetaUpgradeScreen
{
    private int _selectedIndex;

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        int vDir = InputHelper.GetMenuVertical();
        if (vDir != 0)
            _selectedIndex = Math.Clamp(_selectedIndex + vDir, 0, MetaProgression.Upgrades.Length - 1);

        if (InputHelper.IsConfirmPressed())
        {
            var meta = manager.Meta;
            int level = meta.GetLevel(_selectedIndex);
            int cost = meta.GetUpgradeCost(_selectedIndex);
            if (level < MetaProgression.MaxUpgradeLevel && meta.Tokens >= cost)
            {
                meta.Tokens -= cost;
                meta.AddLevel(_selectedIndex);
                meta.Save();
                state.Assets.PlaySoundVariant("select", 0.5f);
            }
        }

        if (InputHelper.IsCancelPressed())
            manager.TransitionTo(GameScreen.MainMenu);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(25, 20, 30, 255));

        string title = "UPGRADES";
        int titleW = Raylib.MeasureText(title, 18);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 12, 18, Color.Gold);

        // Token display
        string tokenStr = $"Tokens: {manager.Meta.Tokens}";
        UIRenderer.DrawTextSmall(tokenStr, Constants.LogicalWidth - tokenStr.Length * 5 - 12, 14, Color.Gold);

        int startY = 40;
        int rowH = 30;
        int leftX = 60;

        var mouse = Display.ScreenToLogical(Raylib.GetMousePosition());

        for (int i = 0; i < MetaProgression.Upgrades.Length; i++)
        {
            var upgrade = MetaProgression.Upgrades[i];
            int level = manager.Meta.GetLevel(i);
            int cost = manager.Meta.GetUpgradeCost(i);
            bool maxed = level >= MetaProgression.MaxUpgradeLevel;
            int y = startY + i * rowH;

            bool hovered = mouse.Y >= y && mouse.Y < y + rowH && mouse.X >= leftX && mouse.X < Constants.LogicalWidth - 60;
            if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _selectedIndex = i;
                var meta = manager.Meta;
                if (!maxed && meta.Tokens >= cost)
                {
                    meta.Tokens -= cost;
                    meta.AddLevel(i);
                    meta.Save();
                    state.Assets.PlaySoundVariant("select", 0.5f);
                }
            }

            bool selected = i == _selectedIndex;
            Color bg = selected ? new Color(50, 40, 55, 255) :
                hovered ? new Color(40, 35, 45, 255) : new Color(30, 25, 35, 255);
            Raylib.DrawRectangle(leftX - 4, y, Constants.LogicalWidth - 112, rowH - 2, bg);

            if (selected)
                Raylib.DrawRectangleLines(leftX - 4, y, Constants.LogicalWidth - 112, rowH - 2, Color.Gold);

            // Name + stat
            UIRenderer.DrawTextSmall(upgrade.Name, leftX, y + 3, Color.White);
            UIRenderer.DrawTextSmall(upgrade.Description, leftX, y + 13, Color.LightGray);

            // Level bar
            int barX = leftX + 180;
            int barW = 100;
            int barH = 6;
            int barY = y + 5;
            Raylib.DrawRectangle(barX, barY, barW, barH, new Color(20, 15, 25, 255));
            int filled = (int)(barW * (float)level / MetaProgression.MaxUpgradeLevel);
            Color barColor = maxed ? Color.Gold : Color.Purple;
            Raylib.DrawRectangle(barX, barY, filled, barH, barColor);
            Raylib.DrawRectangleLines(barX, barY, barW, barH, new Color(80, 70, 90, 255));

            // Level text
            string lvlStr = maxed ? "MAX" : $"Lv {level}";
            UIRenderer.DrawTextSmall(lvlStr, barX + barW + 6, y + 3, maxed ? Color.Gold : Color.White);

            // Cost
            if (!maxed)
            {
                bool canAfford = manager.Meta.Tokens >= cost;
                string costStr = $"{cost} tokens";
                UIRenderer.DrawTextSmall(costStr, barX + barW + 6, y + 13,
                    canAfford ? Color.Green : Color.Red);
            }
        }

        UIRenderer.DrawTextSmall("ESC = back, Enter = buy", leftX, Constants.LogicalHeight - 12, Color.Gray);
    }
}
