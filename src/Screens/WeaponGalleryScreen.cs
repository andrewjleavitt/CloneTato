using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class WeaponGalleryScreen
{
    private int _scrollOffset;
    private const int CardsPerRow = 3;
    private const int CardW = 130;
    private const int CardH = 52;
    private const int CardPadding = 6;

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        if (InputHelper.IsCancelPressed())
            manager.TransitionTo(GameScreen.MainMenu);

        // Scroll with mouse wheel or gamepad vertical nav
        int wheel = (int)Raylib.GetMouseWheelMove();
        _scrollOffset -= wheel * 20;

        int vDir = InputHelper.GetMenuVertical();
        _scrollOffset += vDir * 20;

        int totalRows = (WeaponDatabase.Weapons.Length + CardsPerRow - 1) / CardsPerRow;
        int contentHeight = totalRows * (CardH + CardPadding) + 40;
        int maxScroll = Math.Max(0, contentHeight - Constants.LogicalHeight + 30);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, maxScroll);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(35, 25, 18, 255));

        // Title
        string title = "WEAPON GALLERY";
        int titleW = Raylib.MeasureText(title, 14);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 6, 14, Color.Gold);

        int startY = 24 - _scrollOffset;
        int totalW = CardsPerRow * (CardW + CardPadding) - CardPadding;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;

        for (int i = 0; i < WeaponDatabase.Weapons.Length; i++)
        {
            var weapon = WeaponDatabase.Weapons[i];
            int row = i / CardsPerRow;
            int col = i % CardsPerRow;

            int cx = startX + col * (CardW + CardPadding);
            int cy = startY + row * (CardH + CardPadding);

            // Skip if off screen
            if (cy + CardH < 0 || cy > Constants.LogicalHeight) continue;

            // Card background
            Color tierColor = weapon.ShopTier switch
            {
                1 => new Color(50, 50, 40, 255),
                2 => new Color(50, 45, 30, 255),
                3 => new Color(55, 35, 30, 255),
                _ => new Color(50, 50, 50, 255),
            };
            Raylib.DrawRectangle(cx, cy, CardW, CardH, tierColor);

            Color borderColor = weapon.ShopTier switch
            {
                1 => Color.Gray,
                2 => Color.SkyBlue,
                3 => Color.Gold,
                _ => Color.Gray,
            };
            Raylib.DrawRectangleLines(cx, cy, CardW, CardH, borderColor);

            // Weapon sprite
            state.Assets.Weapons.Draw(weapon.SpriteIndex, cx + 4, cy + 4, Color.White);

            // Name + tier
            string tierLabel = weapon.ShopTier switch { 1 => "I", 2 => "II", 3 => "III", _ => "?" };
            UIRenderer.DrawTextSmall(weapon.Name, cx + 28, cy + 4, Color.White);
            UIRenderer.DrawTextSmall($"T{tierLabel}", cx + CardW - 18, cy + 4, borderColor);

            // Type
            string typeStr = weapon.Type switch
            {
                WeaponType.Auto when weapon.IsLockOn => "Lock-On",
                WeaponType.Auto => "Auto",
                WeaponType.Manual when weapon.IsMine => "Mine",
                WeaponType.Manual => "Manual",
                WeaponType.Melee => "Melee",
                _ => "?"
            };
            UIRenderer.DrawTextSmall(typeStr, cx + 28, cy + 14, Color.LightGray);

            // Stats line 1: damage, fire rate
            string stats1 = $"DMG:{weapon.BaseDamage:F0} RoF:{weapon.FireRate:F1}/s";
            UIRenderer.DrawTextSmall(stats1, cx + 4, cy + 26, new Color(200, 200, 180, 255));

            // Stats line 2: varies by type
            string stats2 = "";
            if (weapon.Type == WeaponType.Melee)
            {
                stats2 = $"RNG:{weapon.Range:F0} ARC:{(weapon.MeleeArc * 180f / MathF.PI):F0}°";
            }
            else if (weapon.IsLockOn)
            {
                stats2 = $"x{weapon.MissileCount} missiles AOE:{weapon.ExplosionRadius:F0}";
            }
            else
            {
                stats2 = $"RNG:{weapon.Range:F0} SPD:{weapon.ProjectileSpeed:F0}";
                if (weapon.PierceCount > 0) stats2 += $" PRC:{weapon.PierceCount}";
                if (weapon.BurstCount > 1) stats2 += $" x{weapon.BurstCount}";
            }
            UIRenderer.DrawTextSmall(stats2, cx + 4, cy + 36, new Color(180, 180, 160, 255));

            // Clip info (bottom-right of card)
            if (weapon.ClipSize > 0)
            {
                string clipStr = $"{weapon.ClipSize}/{weapon.ReloadTime:F1}s";
                UIRenderer.DrawTextSmall(clipStr, cx + CardW - clipStr.Length * 5 - 4, cy + 36, Color.SkyBlue);
            }

            // Cost
            string costStr = $"${weapon.Cost}";
            UIRenderer.DrawTextSmall(costStr, cx + CardW - costStr.Length * 5 - 4, cy + 14, Color.Gold);
        }

        // Back hint
        UIRenderer.DrawTextSmall("ESC to go back", 4, Constants.LogicalHeight - 12, Color.Gray);
    }
}
