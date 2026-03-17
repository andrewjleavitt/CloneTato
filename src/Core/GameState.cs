using System.Numerics;
using CloneTato.Assets;
using CloneTato.Data;
using CloneTato.Entities;

namespace CloneTato.Core;

public class GameState
{
    public AssetManager Assets = null!;
    public Player Player = new();
    public List<Enemy> Enemies = new();
    public List<Projectile> Projectiles = new();
    public List<XPOrb> XPOrbs = new();
    public List<DamageNumber> DamageNumbers = new();
    public List<Projectile> EnemyProjectiles = new();
    public List<Mine> Mines = new();
    public List<MeleeSwipe> MeleeSwipes = new();
    public List<HealthPickup> HealthPickups = new();
    public List<Obstacle> Obstacles = new();
    public List<Barrel> Barrels = new();
    public List<TerrainZone> TerrainZones = new();

    public List<WeaponInstance> EquippedWeapons = new();
    public List<float> WeaponCooldowns = new();
    public List<int> WeaponClipAmmo = new();     // current ammo in clip per weapon
    public List<float> WeaponReloadTimers = new(); // >0 means reloading
    public List<float> WeaponOrbitAngles = new(); // visual angle around player
    public List<ItemDef> OwnedItems = new();

    public Vector2 MouseWorldPosition; // mouse cursor in world space
    public bool IsFiring; // true while mouse button is held

    public Stats ItemBonus;
    public Stats LevelBonus;
    public Stats MetaBonus;

    public int CurrentWave;
    public float WaveTimer;
    public bool WaveActive;
    public int EnemiesSpawnedThisWave;
    public int EnemiesKilledThisWave;
    public float SpawnAccumulator;
    public WaveConfig? CurrentWaveConfig;

    public int Gold;
    public int XP;
    public int Level = 1;
    public int XPToNextLevel = 8;
    public bool LevelUpPending;
    public int PendingLevelUps;

    // Stats tracking
    public int TotalEnemiesKilled;
    public int TotalDamageDealt;
    public float TotalTimeSurvived;

    // Entity pools
    public const int MaxEnemies = 300;
    public const int MaxProjectiles = 500;
    public const int MaxXPOrbs = 400;
    public const int MaxDamageNumbers = 100;
    public const int MaxEnemyProjectiles = 200;
    public const int MaxHealthPickups = 20;
    public const int MaxMines = 30;
    public const int MaxMeleeSwipes = 20;

    public void InitPools()
    {
        Enemies.Clear();
        for (int i = 0; i < MaxEnemies; i++)
            Enemies.Add(new Enemy());

        Projectiles.Clear();
        for (int i = 0; i < MaxProjectiles; i++)
            Projectiles.Add(new Projectile());

        EnemyProjectiles.Clear();
        for (int i = 0; i < MaxEnemyProjectiles; i++)
            EnemyProjectiles.Add(new Projectile());

        XPOrbs.Clear();
        for (int i = 0; i < MaxXPOrbs; i++)
            XPOrbs.Add(new XPOrb());

        DamageNumbers.Clear();
        for (int i = 0; i < MaxDamageNumbers; i++)
            DamageNumbers.Add(new DamageNumber());

        HealthPickups.Clear();
        for (int i = 0; i < MaxHealthPickups; i++)
            HealthPickups.Add(new HealthPickup());

        Mines.Clear();
        for (int i = 0; i < MaxMines; i++)
            Mines.Add(new Mine());

        MeleeSwipes.Clear();
        for (int i = 0; i < MaxMeleeSwipes; i++)
            MeleeSwipes.Add(new MeleeSwipe());
    }

    public void StartNewRun(CharacterDef character)
    {
        InitPools();
        Player = new Player();
        Player.Init(character);

        EquippedWeapons.Clear();
        WeaponCooldowns.Clear();
        WeaponClipAmmo.Clear();
        WeaponReloadTimers.Clear();
        WeaponOrbitAngles.Clear();
        OwnedItems.Clear();

        // Give starting weapon
        var startWeapon = new WeaponInstance(WeaponDatabase.Weapons[character.StartingWeaponIndex]);
        EquippedWeapons.Add(startWeapon);
        WeaponCooldowns.Add(0f);
        WeaponClipAmmo.Add(startWeapon.ClipSize);
        WeaponReloadTimers.Add(0f);
        WeaponOrbitAngles.Add(0f);

        GenerateArena();

        ItemBonus = default;
        LevelBonus = default;

        CurrentWave = 0;
        Gold = 0;
        XP = 0;
        Level = 1;
        XPToNextLevel = 8;
        LevelUpPending = false;
        PendingLevelUps = 0;

        TotalEnemiesKilled = 0;
        TotalDamageDealt = 0;
        TotalTimeSurvived = 0;
    }

    public void StartWave()
    {
        CurrentWave++;
        CurrentWaveConfig = WaveConfig.GetWave(CurrentWave);
        WaveTimer = CurrentWaveConfig.Duration;
        WaveActive = true;
        EnemiesSpawnedThisWave = 0;
        EnemiesKilledThisWave = 0;
        SpawnAccumulator = 0;
    }

    public Enemy? GetInactiveEnemy()
    {
        for (int i = 0; i < Enemies.Count; i++)
            if (!Enemies[i].Active) return Enemies[i];
        return null;
    }

    public Projectile? GetInactiveProjectile()
    {
        for (int i = 0; i < Projectiles.Count; i++)
            if (!Projectiles[i].Active) return Projectiles[i];
        return null;
    }

    public Projectile? GetInactiveEnemyProjectile()
    {
        for (int i = 0; i < EnemyProjectiles.Count; i++)
            if (!EnemyProjectiles[i].Active) return EnemyProjectiles[i];
        return null;
    }

    public XPOrb? GetInactiveXPOrb()
    {
        for (int i = 0; i < XPOrbs.Count; i++)
            if (!XPOrbs[i].Active) return XPOrbs[i];
        return null;
    }

    public DamageNumber? GetInactiveDamageNumber()
    {
        for (int i = 0; i < DamageNumbers.Count; i++)
            if (!DamageNumbers[i].Active) return DamageNumbers[i];
        return null;
    }

    public HealthPickup? GetInactiveHealthPickup()
    {
        for (int i = 0; i < HealthPickups.Count; i++)
            if (!HealthPickups[i].Active) return HealthPickups[i];
        return null;
    }

    public Mine? GetInactiveMine()
    {
        for (int i = 0; i < Mines.Count; i++)
            if (!Mines[i].Active) return Mines[i];
        return null;
    }

    public MeleeSwipe? GetInactiveMeleeSwipe()
    {
        for (int i = 0; i < MeleeSwipes.Count; i++)
            if (!MeleeSwipes[i].Active) return MeleeSwipes[i];
        return null;
    }

    public void HandleEnemyDeath(Enemy enemy)
    {
        enemy.StartDeath();
        TotalEnemiesKilled++;
        EnemiesKilledThisWave++;

        // XP orbs
        int orbCount = 1 + enemy.XPValue / 3;
        for (int o = 0; o < orbCount; o++)
        {
            var orb = GetInactiveXPOrb();
            orb?.Init(enemy.Position, Math.Max(1, enemy.XPValue / orbCount));
        }
        Gold += enemy.GoldValue;

        // Armed enemies have a 30% chance to drop a health pickup
        if (enemy.IsArmed && Random.Shared.NextSingle() < 0.30f)
        {
            var pickup = GetInactiveHealthPickup();
            pickup?.Init(enemy.Position, 10);
        }
    }

    public int ActiveEnemyCount()
    {
        int count = 0;
        for (int i = 0; i < Enemies.Count; i++)
            if (Enemies[i].Active) count++;
        return count;
    }

    public void AddXP(int amount)
    {
        XP += (int)(amount * Player.ComputedStats.XPMultiplier);
        while (XP >= XPToNextLevel)
        {
            XP -= XPToNextLevel;
            Level++;
            XPToNextLevel = 5 + Level * 3;
            PendingLevelUps++;
            LevelUpPending = true;
        }
    }

    private void GenerateArena()
    {
        Obstacles.Clear();
        Barrels.Clear();
        TerrainZones.Clear();

        var rng = Random.Shared;
        float centerX = Constants.ArenaWidth / 2f;
        float centerY = Constants.ArenaHeight / 2f;
        float safeRadius = 80f; // keep spawn area clear

        // Place 8-14 obstacles (trees, 2 tiles tall)
        // Tree tops at (3,3) and (8,3) → store top tile index, bottom is +18
        int obstacleCount = rng.Next(8, 15);
        int[] treeSprites = { 3 * 18 + 3, 3 * 18 + 8 }; // top tile indices
        for (int i = 0; i < obstacleCount; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = rng.NextSingle() * (Constants.ArenaWidth - 80) + 40;
                float y = rng.NextSingle() * (Constants.ArenaHeight - 80) + 40;
                float radius = 7f; // ~16px tile, circle slightly smaller

                // Don't place near center
                if (Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY)) < safeRadius + radius)
                    continue;

                // Don't overlap other obstacles
                bool overlaps = false;
                foreach (var other in Obstacles)
                {
                    if (Vector2.Distance(new Vector2(x, y), other.Position) < radius + other.Radius + 12f)
                    { overlaps = true; break; }
                }
                if (overlaps) continue;

                Obstacles.Add(new Obstacle
                {
                    Position = new Vector2(x, y),
                    Radius = radius,
                    SpriteIndex = treeSprites[rng.Next(treeSprites.Length)],
                    Active = true,
                });
                break;
            }
        }

        // Place 4-8 barrels
        int barrelCount = rng.Next(4, 9);
        for (int i = 0; i < barrelCount; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = rng.NextSingle() * (Constants.ArenaWidth - 80) + 40;
                float y = rng.NextSingle() * (Constants.ArenaHeight - 80) + 40;
                var pos = new Vector2(x, y);

                if (Vector2.Distance(pos, new Vector2(centerX, centerY)) < safeRadius + 12f)
                    continue;

                // Don't overlap obstacles or other barrels
                bool overlaps = false;
                foreach (var obs in Obstacles)
                {
                    if (Vector2.Distance(pos, obs.Position) < 12f + obs.Radius + 8f)
                    { overlaps = true; break; }
                }
                if (!overlaps)
                {
                    foreach (var other in Barrels)
                    {
                        if (Vector2.Distance(pos, other.Position) < 24f)
                        { overlaps = true; break; }
                    }
                }
                if (overlaps) continue;

                var type = rng.NextSingle() < 0.55f ? BarrelType.Explosive : BarrelType.Toxic;
                int hp = type == BarrelType.Explosive ? 3 : 5;
                Barrels.Add(new Barrel
                {
                    Position = pos,
                    Radius = 8f,
                    Type = type,
                    CurrentHP = hp,
                    MaxHP = hp,
                    Active = true,
                });
                break;
            }
        }

        // Place 3-5 terrain zones
        int zoneCount = rng.Next(3, 6);
        for (int i = 0; i < zoneCount; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = rng.NextSingle() * (Constants.ArenaWidth - 160) + 80;
                float y = rng.NextSingle() * (Constants.ArenaHeight - 160) + 80;
                float radius = 40f + rng.NextSingle() * 30f;
                var type = rng.NextSingle() < 0.65f ? TerrainType.Sand : TerrainType.Oasis;

                // Don't place center of zone too close to spawn
                if (Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY)) < safeRadius)
                    continue;

                TerrainZones.Add(new TerrainZone
                {
                    Position = new Vector2(x, y),
                    Radius = radius,
                    Type = type,
                    Active = true,
                });
                break;
            }
        }

        // Pick one decorative tile set for this run (0=green grass, 1=purple grass, 2=metallic)
        int decoSet = rng.Next(3);

        // Place 3-6 decorative patches
        int decoCount = rng.Next(3, 7);
        for (int i = 0; i < decoCount; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = rng.NextSingle() * (Constants.ArenaWidth - 160) + 80;
                float y = rng.NextSingle() * (Constants.ArenaHeight - 160) + 80;
                float radius = 30f + rng.NextSingle() * 35f;

                if (Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY)) < safeRadius)
                    continue;

                TerrainZones.Add(new TerrainZone
                {
                    Position = new Vector2(x, y),
                    Radius = radius,
                    Type = TerrainType.Decorative,
                    DecoTileSet = decoSet,
                    Active = true,
                });
                break;
            }
        }
    }

    public void RecomputePlayerStats()
    {
        ItemBonus = default;
        foreach (var item in OwnedItems)
            ItemBonus = ItemBonus + item.StatModifier;
        Player.RecomputeStats(ItemBonus, LevelBonus + MetaBonus);
    }
}
