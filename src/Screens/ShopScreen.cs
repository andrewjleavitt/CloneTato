using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class ShopScreen
{
    private List<object> _shopItems = new(); // WeaponDef or ItemDef
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
            NextWave(state, manager);
    }

    private void GenerateShopItems(GameState state)
    {
        _shopItems.Clear();
        int maxTier = 1 + state.CurrentWave / 5;

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

        // Refill all clips between waves
        for (int i = 0; i < state.EquippedWeapons.Count; i++)
        {
            state.WeaponClipAmmo[i] = state.EquippedWeapons[i].ClipSize;
            state.WeaponReloadTimers[i] = 0f;
        }

        state.StartWave();
        manager.TransitionTo(GameScreen.Playing);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(35, 25, 20, 255));

        string title = $"SHOP - Wave {state.CurrentWave} Complete!";
        int titleW = Raylib.MeasureText(title, 14);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 6, 14, Color.Gold);

        UIRenderer.DrawTextMedium($"Gold: ${state.Gold}", Constants.LogicalWidth / 2 - 30, 22, Color.Gold);

        var mouse = Raylib.GetMousePosition();
        mouse.X /= Constants.WindowScale;
        mouse.Y /= Constants.WindowScale;

        // === WEAPON UPGRADES (top section) ===
        if (state.EquippedWeapons.Count > 0)
        {
            UIRenderer.DrawTextSmall("UPGRADE WEAPONS", 8, 38, Color.Orange);

            int ugW = 105, ugH = 38;
            int ugY = 48;
            for (int i = 0; i < state.EquippedWeapons.Count; i++)
            {
                var weapon = state.EquippedWeapons[i];
                int ux = 8 + i * (ugW + 4);
                bool hovered = mouse.X >= ux && mouse.X <= ux + ugW && mouse.Y >= ugY && mouse.Y <= ugY + ugH;

                Color bg = hovered ? new Color(70, 55, 35, 255) : new Color(45, 32, 22, 255);
                Raylib.DrawRectangle(ux, ugY, ugW, ugH, bg);
                Color border = weapon.CanUpgrade ? (hovered ? Color.Gold : Color.Orange) : Color.Gray;
                Raylib.DrawRectangleLines(ux, ugY, ugW, ugH, border);

                // Weapon icon
                state.Assets.Weapons.Draw(weapon.Def.SpriteIndex, ux + 2, ugY + 7, Color.White);

                // Name + level
                string lvlText = weapon.UpgradeLevel > 0 ? $" +{weapon.UpgradeLevel}" : "";
                UIRenderer.DrawTextSmall($"{weapon.Def.Name}{lvlText}", ux + 26, ugY + 3, Color.White);

                if (weapon.CanUpgrade)
                {
                    UIRenderer.DrawTextSmall(weapon.UpgradePreview(), ux + 26, ugY + 13, Color.LightGray);
                    UIRenderer.DrawTextSmall($"Upgrade: ${weapon.UpgradeCost}", ux + 26, ugY + 25,
                        state.Gold >= weapon.UpgradeCost ? Color.Green : Color.Red);

                    if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left) && state.Gold >= weapon.UpgradeCost)
                    {
                        state.Gold -= weapon.UpgradeCost;
                        weapon.UpgradeLevel++;
                        state.Assets.PlaySoundVariant("select", 0.5f);
                    }
                }
                else
                {
                    UIRenderer.DrawTextSmall("MAX LEVEL", ux + 26, ugY + 18, Color.Gold);
                }
            }
        }

        // === BUY NEW WEAPONS & ITEMS (bottom section) ===
        UIRenderer.DrawTextSmall("BUY", 8, 94, Color.SkyBlue);

        int cardW = 100, cardH = 65;
        int totalW = _shopItems.Count > 0 ? _shopItems.Count * (cardW + 6) - 6 : 0;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 104;

        for (int i = 0; i < _shopItems.Count; i++)
        {
            int cx = startX + i * (cardW + 6);
            bool hovered = mouse.X >= cx && mouse.X <= cx + cardW && mouse.Y >= cardY && mouse.Y <= cardY + cardH;

            Color bg = hovered ? new Color(70, 50, 35, 255) : new Color(50, 35, 25, 255);
            Raylib.DrawRectangle(cx, cardY, cardW, cardH, bg);
            Raylib.DrawRectangleLines(cx, cardY, cardW, cardH, hovered ? Color.Gold : Color.Gray);

            if (_shopItems[i] is WeaponDef weapon)
            {
                state.Assets.Weapons.Draw(weapon.SpriteIndex, cx + cardW / 2 - 12, cardY + 3, Color.White);
                UIRenderer.DrawTextSmall(weapon.Name, cx + 4, cardY + 28, Color.White);
                UIRenderer.DrawTextSmall($"DMG:{weapon.BaseDamage:F0} SPD:{weapon.FireRate:F1}", cx + 4, cardY + 38, Color.LightGray);
                bool canAfford = state.Gold >= weapon.Cost;
                bool hasSlot = state.EquippedWeapons.Count < Constants.MaxWeaponSlots;
                UIRenderer.DrawTextSmall($"${weapon.Cost}", cx + 4, cardY + 52,
                    canAfford && hasSlot ? Color.Green : Color.Red);
                if (!hasSlot)
                    UIRenderer.DrawTextSmall("FULL", cx + 50, cardY + 52, Color.Red);

                if (hovered && Raylib.IsMouseButtonPressed(MouseButton.Left) && canAfford && hasSlot)
                {
                    state.Gold -= weapon.Cost;
                    var newWeapon = new WeaponInstance(weapon);
                    state.EquippedWeapons.Add(newWeapon);
                    state.WeaponCooldowns.Add(0f);
                    state.WeaponClipAmmo.Add(newWeapon.ClipSize);
                    state.WeaponReloadTimers.Add(0f);
                    state.WeaponOrbitAngles.Add(0f);
                    _shopItems.RemoveAt(i);
                    state.Assets.PlaySoundVariant("select", 0.5f);
                }
            }
            else if (_shopItems[i] is ItemDef item)
            {
                state.Assets.Weapons.Draw(item.SpriteIndex, cx + cardW / 2 - 12, cardY + 3, Color.White);
                UIRenderer.DrawTextSmall(item.Name, cx + 4, cardY + 28, Color.White);
                UIRenderer.DrawTextSmall(item.Description, cx + 4, cardY + 38, Color.LightGray);
                UIRenderer.DrawTextSmall($"${item.Cost}", cx + 4, cardY + 52,
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
            Constants.LogicalWidth / 2 - 50, cardY + cardH + 8, 100, 16, new Color(80, 60, 40, 255)))
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
            Constants.LogicalWidth / 2 - 45, Constants.LogicalHeight - 30, 90, 20, new Color(60, 100, 60, 255)))
        {
            NextWave(state, manager);
        }

        UIRenderer.DrawTextSmall("You heal 10% HP between waves",
            Constants.LogicalWidth / 2 - 70, Constants.LogicalHeight - 8, Color.Gray);
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
