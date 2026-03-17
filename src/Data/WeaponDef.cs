using Raylib_cs;

namespace CloneTato.Data;

public enum WeaponType
{
    Auto,   // fires automatically when enemies are in range
    Manual, // fires on mouse click (grenades, mines)
    Melee,  // instant arc damage, no projectiles
}

public class WeaponDef
{
    public string Name = "";
    public int SpriteIndex;
    public WeaponType Type = WeaponType.Auto;
    public float BaseDamage;
    public float FireRate; // attacks per second
    public float ProjectileSpeed;
    public float Range;
    public int PierceCount;
    public float Spread; // radians, for shotgun-type
    public int BurstCount = 1;
    public int ShopTier = 1;
    public int Cost = 50;
    public Color ProjectileColor = Color.Yellow;
    public int ClipSize = 0;       // 0 = unlimited (melee, mines)
    public float ReloadTime = 1.0f; // seconds to reload
    public int MaxUpgradeLevel = 5;

    // Melee specific
    public float MeleeArc = MathF.PI * 0.6f; // swing arc width in radians

    // Explosive specific (grenade/rocket)
    public float ExplosionRadius; // 0 = no explosion, >0 = AOE on impact/expiry

    // Mine specific
    public bool IsMine; // placed at feet, detonates on enemy proximity
    public float MineProximity = 25f; // trigger radius

    // Lock-on missile specific
    public bool IsLockOn; // fires homing missiles that lock onto enemies
    public int MissileCount = 10; // total missiles per volley
    public float MissileTurnRate = 6f; // radians/sec steering
}

/// <summary>
/// A mutable weapon instance the player carries. Wraps a WeaponDef and tracks upgrade level.
/// </summary>
public class WeaponInstance
{
    public WeaponDef Def;
    public int UpgradeLevel; // 0 = base, up to Def.MaxUpgradeLevel

    public WeaponInstance(WeaponDef def)
    {
        Def = def;
        UpgradeLevel = 0;
    }

    // Effective stats (scale with upgrade level)
    public float Damage => Def.BaseDamage * (1f + UpgradeLevel * 0.20f);
    public float FireRate => Def.FireRate * (1f + UpgradeLevel * 0.10f);
    public float ProjectileSpeed => Def.ProjectileSpeed * (1f + UpgradeLevel * 0.05f);
    public float Range => Def.Range * (1f + UpgradeLevel * 0.08f);
    public int PierceCount => Def.PierceCount + UpgradeLevel / 2;
    public int BurstCount => Def.BurstCount + (Def.BurstCount > 1 ? UpgradeLevel / 2 : 0);
    public float MeleeArc => Def.MeleeArc * (1f + UpgradeLevel * 0.05f);
    public float ExplosionRadius => Def.ExplosionRadius * (1f + UpgradeLevel * 0.12f);
    public int MissileCount => Def.MissileCount + UpgradeLevel; // +1 missile per upgrade
    public int ClipSize => Def.ClipSize + UpgradeLevel; // +1 per upgrade
    public float ReloadTime => Def.ReloadTime * (1f - UpgradeLevel * 0.08f); // faster reload with upgrades

    public bool CanUpgrade => UpgradeLevel < Def.MaxUpgradeLevel;
    public int UpgradeCost => Def.Cost / 2 + UpgradeLevel * 15;

    public string StatsText()
    {
        string s = $"DMG:{Damage:F0} SPD:{FireRate:F1}";
        if (ClipSize > 0) s += $" CLIP:{ClipSize}";
        if (PierceCount > 0) s += $" PRC:{PierceCount}";
        if (BurstCount > 1) s += $" x{BurstCount}";
        return s;
    }

    public string UpgradePreview()
    {
        if (!CanUpgrade) return "MAX LEVEL";
        int next = UpgradeLevel + 1;
        float nextDmg = Def.BaseDamage * (1f + next * 0.20f);
        float nextRate = Def.FireRate * (1f + next * 0.10f);
        return $"DMG:{Damage:F0}->{nextDmg:F0} SPD:{FireRate:F1}->{nextRate:F1}";
    }
}

public static class WeaponDatabase
{
    public static readonly WeaponDef[] Weapons =
    {
        // === AUTO GUNS (Tier 1) ===
        new()
        {
            Name = "Pistol", SpriteIndex = 0, Type = WeaponType.Auto,
            BaseDamage = 12, FireRate = 2.5f, ProjectileSpeed = 250f,
            Range = 150f, PierceCount = 0, ClipSize = 8, ReloadTime = 1.2f,
            ShopTier = 1, Cost = 40, ProjectileColor = Color.Yellow,
        },
        new()
        {
            Name = "SMG", SpriteIndex = 1, Type = WeaponType.Auto,
            BaseDamage = 6, FireRate = 6f, ProjectileSpeed = 220f,
            Range = 120f, PierceCount = 0, Spread = 0.15f, ClipSize = 20, ReloadTime = 1.0f,
            ShopTier = 1, Cost = 50, ProjectileColor = Color.Orange,
        },
        new()
        {
            Name = "Shotgun", SpriteIndex = 2, Type = WeaponType.Auto,
            BaseDamage = 8, FireRate = 1.2f, ProjectileSpeed = 200f,
            Range = 100f, PierceCount = 0, Spread = 0.4f, BurstCount = 5, ClipSize = 4, ReloadTime = 1.5f,
            ShopTier = 1, Cost = 55, ProjectileColor = Color.Red,
        },
        new()
        {
            Name = "Crossbow", SpriteIndex = 3, Type = WeaponType.Auto,
            BaseDamage = 25, FireRate = 1.0f, ProjectileSpeed = 300f,
            Range = 200f, PierceCount = 2, ClipSize = 1, ReloadTime = 0.8f,
            ShopTier = 1, Cost = 60, ProjectileColor = Color.SkyBlue,
        },

        // === MELEE (Tier 1-2) ===
        new()
        {
            Name = "Knife", SpriteIndex = 10, Type = WeaponType.Melee,
            BaseDamage = 10, FireRate = 3.5f, Range = 28f,
            MeleeArc = MathF.PI * 0.5f, ShopTier = 1, Cost = 35,
        },
        new()
        {
            Name = "Sword", SpriteIndex = 11, Type = WeaponType.Melee,
            BaseDamage = 20, FireRate = 1.8f, Range = 35f,
            MeleeArc = MathF.PI * 0.7f, ShopTier = 1, Cost = 50,
        },
        new()
        {
            Name = "Spear", SpriteIndex = 12, Type = WeaponType.Melee,
            BaseDamage = 18, FireRate = 2.2f, Range = 42f,
            MeleeArc = MathF.PI * 0.3f, ShopTier = 2, Cost = 75,
        },
        new()
        {
            Name = "Hammer", SpriteIndex = 13, Type = WeaponType.Melee,
            BaseDamage = 40, FireRate = 0.8f, Range = 32f,
            MeleeArc = MathF.PI * 0.9f, ShopTier = 2, Cost = 95,
        },

        // === AUTO GUNS (Tier 2) ===
        new()
        {
            Name = "Rifle", SpriteIndex = 4, Type = WeaponType.Auto,
            BaseDamage = 18, FireRate = 2.0f, ProjectileSpeed = 280f,
            Range = 180f, PierceCount = 1, ClipSize = 10, ReloadTime = 1.3f,
            ShopTier = 2, Cost = 90, ProjectileColor = Color.Gold,
        },
        new()
        {
            Name = "Dual Pistols", SpriteIndex = 5, Type = WeaponType.Auto,
            BaseDamage = 10, FireRate = 4.0f, ProjectileSpeed = 250f,
            Range = 140f, PierceCount = 0, Spread = 0.1f, ClipSize = 16, ReloadTime = 1.1f,
            ShopTier = 2, Cost = 85, ProjectileColor = Color.Yellow,
        },
        new()
        {
            Name = "Sniper", SpriteIndex = 6, Type = WeaponType.Auto,
            BaseDamage = 45, FireRate = 0.6f, ProjectileSpeed = 400f,
            Range = 250f, PierceCount = 3, ClipSize = 3, ReloadTime = 1.8f,
            ShopTier = 2, Cost = 100, ProjectileColor = Color.White,
        },

        // === MANUAL SPECIALS (Tier 2-3) ===
        new()
        {
            Name = "Grenade", SpriteIndex = 14, Type = WeaponType.Manual,
            BaseDamage = 35, FireRate = 0.8f, ProjectileSpeed = 180f,
            Range = 160f, ExplosionRadius = 40f, ClipSize = 3, ReloadTime = 2.0f,
            ShopTier = 2, Cost = 80, ProjectileColor = Color.DarkGreen,
        },
        new()
        {
            Name = "Mine Layer", SpriteIndex = 15, Type = WeaponType.Manual,
            BaseDamage = 50, FireRate = 0.5f, Range = 30f,
            ExplosionRadius = 35f, IsMine = true, MineProximity = 22f,
            ShopTier = 2, Cost = 70, ProjectileColor = Color.Gray,
        },
        new()
        {
            Name = "Bomb", SpriteIndex = 16, Type = WeaponType.Manual,
            BaseDamage = 80, FireRate = 0.3f, ProjectileSpeed = 140f,
            Range = 120f, ExplosionRadius = 55f, ClipSize = 1, ReloadTime = 2.5f,
            ShopTier = 3, Cost = 130, ProjectileColor = Color.Orange,
        },

        // === AUTO GUNS (Tier 3) ===
        new()
        {
            Name = "Minigun", SpriteIndex = 7, Type = WeaponType.Auto,
            BaseDamage = 5, FireRate = 10f, ProjectileSpeed = 220f,
            Range = 130f, PierceCount = 0, Spread = 0.2f, ClipSize = 50, ReloadTime = 2.0f,
            ShopTier = 3, Cost = 140, ProjectileColor = Color.Orange,
        },
        new()
        {
            Name = "Rocket", SpriteIndex = 8, Type = WeaponType.Auto,
            BaseDamage = 60, FireRate = 0.4f, ProjectileSpeed = 150f,
            Range = 200f, PierceCount = 0, ExplosionRadius = 45f, ClipSize = 2, ReloadTime = 2.2f,
            ShopTier = 3, Cost = 150, ProjectileColor = Color.Red,
        },
        new()
        {
            Name = "Laser", SpriteIndex = 9, Type = WeaponType.Auto,
            BaseDamage = 3, FireRate = 15f, ProjectileSpeed = 350f,
            Range = 180f, PierceCount = 1, ClipSize = 30, ReloadTime = 1.5f,
            ShopTier = 3, Cost = 130, ProjectileColor = Color.Lime,
        },

        // === LOCK-ON (Tier 3) ===
        new()
        {
            Name = "Missile Launcher", SpriteIndex = 18, Type = WeaponType.Auto,
            BaseDamage = 25, FireRate = 0.25f, ProjectileSpeed = 120f,
            Range = 300f, PierceCount = 0, ExplosionRadius = 30f,
            ClipSize = 10, ReloadTime = 3.0f,
            ShopTier = 3, Cost = 160, ProjectileColor = Color.Red,
            IsLockOn = true, MissileCount = 10, MissileTurnRate = 6f,
        },

        // === MELEE (Tier 3) ===
        new()
        {
            Name = "Cleaver", SpriteIndex = 17, Type = WeaponType.Melee,
            BaseDamage = 55, FireRate = 1.2f, Range = 38f,
            MeleeArc = MathF.PI * 0.8f, ShopTier = 3, Cost = 120,
        },
    };
}
