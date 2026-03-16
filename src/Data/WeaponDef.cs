using Raylib_cs;

namespace CloneTato.Data;

public class WeaponDef
{
    public string Name = "";
    public int SpriteIndex;
    public float BaseDamage;
    public float FireRate; // shots per second
    public float ProjectileSpeed;
    public float Range;
    public int PierceCount;
    public float Spread; // radians, for shotgun-type
    public int BurstCount = 1;
    public int ShopTier = 1;
    public int Cost = 50;
    public Color ProjectileColor = Color.Yellow;
}

public static class WeaponDatabase
{
    public static readonly WeaponDef[] Weapons =
    {
        // Tier 1
        new()
        {
            Name = "Pistol", SpriteIndex = 0,
            BaseDamage = 12, FireRate = 2.5f, ProjectileSpeed = 250f,
            Range = 150f, PierceCount = 0, ShopTier = 1, Cost = 40,
            ProjectileColor = Color.Yellow,
        },
        new()
        {
            Name = "SMG", SpriteIndex = 1,
            BaseDamage = 6, FireRate = 6f, ProjectileSpeed = 220f,
            Range = 120f, PierceCount = 0, Spread = 0.15f, ShopTier = 1, Cost = 50,
            ProjectileColor = Color.Orange,
        },
        new()
        {
            Name = "Shotgun", SpriteIndex = 2,
            BaseDamage = 8, FireRate = 1.2f, ProjectileSpeed = 200f,
            Range = 100f, PierceCount = 0, Spread = 0.4f, BurstCount = 5, ShopTier = 1, Cost = 55,
            ProjectileColor = Color.Red,
        },
        new()
        {
            Name = "Crossbow", SpriteIndex = 3,
            BaseDamage = 25, FireRate = 1.0f, ProjectileSpeed = 300f,
            Range = 200f, PierceCount = 2, ShopTier = 1, Cost = 60,
            ProjectileColor = Color.SkyBlue,
        },
        // Tier 2
        new()
        {
            Name = "Rifle", SpriteIndex = 4,
            BaseDamage = 18, FireRate = 2.0f, ProjectileSpeed = 280f,
            Range = 180f, PierceCount = 1, ShopTier = 2, Cost = 90,
            ProjectileColor = Color.Gold,
        },
        new()
        {
            Name = "Dual Pistols", SpriteIndex = 5,
            BaseDamage = 10, FireRate = 4.0f, ProjectileSpeed = 250f,
            Range = 140f, PierceCount = 0, Spread = 0.1f, ShopTier = 2, Cost = 85,
            ProjectileColor = Color.Yellow,
        },
        new()
        {
            Name = "Sniper", SpriteIndex = 6,
            BaseDamage = 45, FireRate = 0.6f, ProjectileSpeed = 400f,
            Range = 250f, PierceCount = 3, ShopTier = 2, Cost = 100,
            ProjectileColor = Color.White,
        },
        // Tier 3
        new()
        {
            Name = "Minigun", SpriteIndex = 7,
            BaseDamage = 5, FireRate = 10f, ProjectileSpeed = 220f,
            Range = 130f, PierceCount = 0, Spread = 0.2f, ShopTier = 3, Cost = 140,
            ProjectileColor = Color.Orange,
        },
        new()
        {
            Name = "Rocket", SpriteIndex = 8,
            BaseDamage = 60, FireRate = 0.4f, ProjectileSpeed = 150f,
            Range = 200f, PierceCount = 5, ShopTier = 3, Cost = 150,
            ProjectileColor = Color.Red,
        },
        new()
        {
            Name = "Laser", SpriteIndex = 9,
            BaseDamage = 3, FireRate = 15f, ProjectileSpeed = 350f,
            Range = 180f, PierceCount = 1, ShopTier = 3, Cost = 130,
            ProjectileColor = Color.Lime,
        },
    };
}
