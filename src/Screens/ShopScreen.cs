using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class ShopScreen
{
    private List<object> _shopItems = new(); // mix of WeaponDef and ItemDef
    private bool _initialized;
    private int _rerollCost = 5;

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        if (!_initialized)
        {
            GenerateShopItems(state);
            _initialized = true;
            _rerollCost = 5 + state.CurrentWave;
        }

        if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space))
        {
            NextWave(state, manager);
        }
    }

    private void GenerateShopItems(GameState state)
    {
        _shopItems.Clear();
        int maxTier = 1 + state.CurrentWave / 5;

        // 2 weapons + 2 items
        var availableWeapons = WeaponDatabase.Weapons.Where(w => w.ShopTier <= maxTier).ToList();
        var availableItems = ItemDatabase.Items.Where(i => i.ShopTier <= maxTier).ToList();

        Shuffle(availableWeapons);
        Shuffle(availableItems);

        for (int i = 0; i < Math.Min(2, availableWeapons.Count); i++)
            _shopItems.Add(availableWeapons[i]);
        for (int i = 0; i < Math.Min(2, availableItems.Count); i++)
            _shopItems.Add(availableItems[i]);
    }

    private void NextWave(GameState state, GameStateManager manager)
    {
        _initialized = false;
        state.RecomputePlayerStats();
        // Heal 10% between waves
        state.Player.CurrentHP = Math.Min(
            state.Player.CurrentHP + state.Player.ComputedStats.MaxHP / 10,
            state.Player.ComputedStats.MaxHP);
        state.StartWave();
        manager.TransitionTo(GameScreen.Playing);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(35, 25, 20, 255));

        string title = $"SHOP - Wave {state.CurrentWave} Complete!";
        int titleW = Raylib.MeasureText(title, 14);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 10, 14, Color.Gold);

        // Gold display
        UIRenderer.DrawTextMedium($"Gold: ${state.Gold}", Constants.LogicalWidth / 2 - 30, 28, Color.Gold);

        // Shop items
        int cardW = 100, cardH = 70;
        int totalW = _shopItems.Count * (cardW + 8) - 8;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 50;

        var mouse = Raylib.GetMousePosition();
        mouse.X /= Constants.WindowScale;
        mouse.Y /= Constants.WindowScale;

        for (int i = 0; i < _shopItems.Count; i++)
        {
            int cx = startX + i * (cardW + 8);
            bool hovered = mouse.X >= cx && mouse.X <= cx + cardW && mouse.Y >= cardY && mouse.Y <= cardY + cardH;

            Color bg = hovered ? new Color(70, 50, 35, 255) : new Color(50, 35, 25, 255);
            Raylib.DrawRectangle(cx, cardY, cardW, cardH, bg);
            Raylib.DrawRectangleLines(cx, cardY, cardW, cardH, hovered ? Color.Gold : Color.Gray);

            if (_shopItems[i] is WeaponDef weapon)
            {
                state.Assets.Weapons.Draw(weapon.SpriteIndex, cx + cardW / 2 - 12, cardY + 4, Color.White);
                UIRenderer.DrawTextSmall(weapon.Name, cx + 4, cardY + 30, Color.White);
                UIRenderer.DrawTextSmall($"DMG:{weapon.BaseDamage:F0} SPD:{weapon.FireRate:F1}", cx + 4, cardY + 40, Color.LightGray);
                UIRenderer.DrawTextSmall($"${weapon.Cost}", cx + 4, cardY + 55,
                    state.Gold >= weapon.Cost ? Color.Green : Color.Red);

                if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left) && state.Gold >= weapon.Cost)
                {
                    if (state.EquippedWeapons.Count < Constants.MaxWeaponSlots)
                    {
                        state.Gold -= weapon.Cost;
                        state.EquippedWeapons.Add(weapon);
                        state.WeaponCooldowns.Add(0f);
                        _shopItems.RemoveAt(i);
                        state.Assets.PlaySoundVariant("select", 0.5f);
                    }
                }
            }
            else if (_shopItems[i] is ItemDef item)
            {
                state.Assets.Weapons.Draw(item.SpriteIndex, cx + cardW / 2 - 12, cardY + 4, Color.White);
                UIRenderer.DrawTextSmall(item.Name, cx + 4, cardY + 30, Color.White);
                UIRenderer.DrawTextSmall(item.Description, cx + 4, cardY + 40, Color.LightGray);
                UIRenderer.DrawTextSmall($"${item.Cost}", cx + 4, cardY + 55,
                    state.Gold >= item.Cost ? Color.Green : Color.Red);

                if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left) && state.Gold >= item.Cost)
                {
                    state.Gold -= item.Cost;
                    state.OwnedItems.Add(item);
                    _shopItems.RemoveAt(i);
                    state.RecomputePlayerStats();
                    state.Assets.PlaySoundVariant("coin", 0.5f);
                }
            }
        }

        // Reroll button
        if (UIRenderer.DrawButton($"REROLL (${_rerollCost})",
            Constants.LogicalWidth / 2 - 50, cardY + cardH + 12, 100, 18, new Color(80, 60, 40, 255)))
        {
            if (state.Gold >= _rerollCost)
            {
                state.Gold -= _rerollCost;
                _rerollCost += 3;
                GenerateShopItems(state);
                state.Assets.PlaySoundVariant("select", 0.5f);
            }
        }

        // Next wave button
        if (UIRenderer.DrawButton("NEXT WAVE",
            Constants.LogicalWidth / 2 - 45, Constants.LogicalHeight - 35, 90, 22, new Color(60, 100, 60, 255)))
        {
            NextWave(state, manager);
        }

        // Heal info
        UIRenderer.DrawTextSmall("You heal 10% HP between waves",
            Constants.LogicalWidth / 2 - 70, Constants.LogicalHeight - 10, Color.Gray);
    }

    private static void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Shared.Next(i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
