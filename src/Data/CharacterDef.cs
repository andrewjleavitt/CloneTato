namespace CloneTato.Data;

public class CharacterDef
{
    public string Name = "";
    public string Description = "";
    public int SpriteIndex; // base sprite index in Players atlas (char occupies 4 consecutive tiles)
    public Stats BaseStats;
    public int StartingWeaponIndex;
}

public static class CharacterDatabase
{
    public static readonly CharacterDef[] Characters =
    {
        new()
        {
            Name = "Desert Cat",
            Description = "Balanced fighter",
            SpriteIndex = 0,
            BaseStats = Stats.Default(),
            StartingWeaponIndex = 0, // Pistol
        },
        new()
        {
            Name = "Sand Mouse",
            Description = "Fast but fragile",
            SpriteIndex = 4,
            BaseStats = new Stats
            {
                MaxHP = 70, MoveSpeed = 110f, DamageMultiplier = 0.9f,
                AttackSpeedMultiplier = 1.2f, CritChance = 0.08f, CritDamage = 1.5f,
                Armor = 0, DodgeChance = 0.1f, PickupRange = 60f, XPMultiplier = 1.0f,
                ReloadSpeedMultiplier = 1.0f,
            },
            StartingWeaponIndex = 1, // SMG
        },
        new()
        {
            Name = "Dune Fox",
            Description = "Lucky and evasive",
            SpriteIndex = 8,
            BaseStats = new Stats
            {
                MaxHP = 85, MoveSpeed = 90f, DamageMultiplier = 1.1f,
                AttackSpeedMultiplier = 1.0f, CritChance = 0.15f, CritDamage = 1.8f,
                Armor = 0, DodgeChance = 0.15f, PickupRange = 50f, XPMultiplier = 1.1f,
                ReloadSpeedMultiplier = 1.0f,
            },
            StartingWeaponIndex = 2, // Shotgun
        },
        new()
        {
            Name = "Rock Turtle",
            Description = "Tanky but slow",
            SpriteIndex = 12,
            BaseStats = new Stats
            {
                MaxHP = 150, MoveSpeed = 55f, DamageMultiplier = 1.0f,
                AttackSpeedMultiplier = 0.8f, CritChance = 0.03f, CritDamage = 1.5f,
                Armor = 3, DodgeChance = 0f, PickupRange = 40f, XPMultiplier = 1.0f,
                ReloadSpeedMultiplier = 1.0f,
            },
            StartingWeaponIndex = 3, // Crossbow
        },
    };
}
