namespace CloneTato.Data;

public class ItemDef
{
    public string Name = "";
    public string Description = "";
    public int SpriteIndex; // weapons atlas index used as icon
    public Stats StatModifier;
    public int Cost;
    public int ShopTier = 1;
}

public static class ItemDatabase
{
    public static readonly ItemDef[] Items =
    {
        // Tier 1
        new()
        {
            Name = "Sturdy Boots", Description = "+12 Speed",
            SpriteIndex = 30, Cost = 30, ShopTier = 1,
            StatModifier = new Stats { MoveSpeed = 12f },
        },
        new()
        {
            Name = "Iron Plate", Description = "+2 Armor",
            SpriteIndex = 31, Cost = 35, ShopTier = 1,
            StatModifier = new Stats { Armor = 2 },
        },
        new()
        {
            Name = "Lucky Charm", Description = "+5% Dodge",
            SpriteIndex = 32, Cost = 40, ShopTier = 1,
            StatModifier = new Stats { DodgeChance = 0.05f },
        },
        new()
        {
            Name = "Bandage", Description = "+15 Max HP",
            SpriteIndex = 33, Cost = 25, ShopTier = 1,
            StatModifier = new Stats { MaxHP = 15 },
        },
        new()
        {
            Name = "Magnet", Description = "+20 Pickup Range",
            SpriteIndex = 34, Cost = 20, ShopTier = 1,
            StatModifier = new Stats { PickupRange = 20f },
        },
        // Tier 2
        new()
        {
            Name = "Adrenaline", Description = "+12% Atk Speed",
            SpriteIndex = 35, Cost = 65, ShopTier = 2,
            StatModifier = new Stats { AttackSpeedMultiplier = 0.12f },
        },
        new()
        {
            Name = "Scope", Description = "+10% Damage",
            SpriteIndex = 36, Cost = 70, ShopTier = 2,
            StatModifier = new Stats { DamageMultiplier = 0.10f },
        },
        new()
        {
            Name = "Experience Tome", Description = "+15% XP",
            SpriteIndex = 37, Cost = 60, ShopTier = 2,
            StatModifier = new Stats { XPMultiplier = 0.15f },
        },
        new()
        {
            Name = "Shield", Description = "+4 Armor, +20 HP",
            SpriteIndex = 38, Cost = 80, ShopTier = 2,
            StatModifier = new Stats { Armor = 4, MaxHP = 20 },
        },
        // Tier 3
        new()
        {
            Name = "Crit Goggles", Description = "+10% Crit",
            SpriteIndex = 39, Cost = 100, ShopTier = 3,
            StatModifier = new Stats { CritChance = 0.10f },
        },
        new()
        {
            Name = "Power Gauntlet", Description = "+20% Dmg, +15% Atk Spd",
            SpriteIndex = 20, Cost = 120, ShopTier = 3,
            StatModifier = new Stats { DamageMultiplier = 0.20f, AttackSpeedMultiplier = 0.15f },
        },
        new()
        {
            Name = "Fortress", Description = "+50 HP, +5 Armor",
            SpriteIndex = 21, Cost = 110, ShopTier = 3,
            StatModifier = new Stats { MaxHP = 50, Armor = 5 },
        },
    };
}
