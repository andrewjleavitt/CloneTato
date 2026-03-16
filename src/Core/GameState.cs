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

    public List<WeaponDef> EquippedWeapons = new();
    public List<float> WeaponCooldowns = new();
    public List<ItemDef> OwnedItems = new();

    public Stats ItemBonus;
    public Stats LevelBonus;

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

    public void InitPools()
    {
        Enemies.Clear();
        for (int i = 0; i < MaxEnemies; i++)
            Enemies.Add(new Enemy());

        Projectiles.Clear();
        for (int i = 0; i < MaxProjectiles; i++)
            Projectiles.Add(new Projectile());

        XPOrbs.Clear();
        for (int i = 0; i < MaxXPOrbs; i++)
            XPOrbs.Add(new XPOrb());

        DamageNumbers.Clear();
        for (int i = 0; i < MaxDamageNumbers; i++)
            DamageNumbers.Add(new DamageNumber());
    }

    public void StartNewRun(CharacterDef character)
    {
        InitPools();
        Player = new Player();
        Player.Init(character);

        EquippedWeapons.Clear();
        WeaponCooldowns.Clear();
        OwnedItems.Clear();

        // Give starting weapon
        var startWeapon = WeaponDatabase.Weapons[character.StartingWeaponIndex];
        EquippedWeapons.Add(startWeapon);
        WeaponCooldowns.Add(0f);

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

    public void RecomputePlayerStats()
    {
        ItemBonus = default;
        foreach (var item in OwnedItems)
            ItemBonus = ItemBonus + item.StatModifier;
        Player.RecomputeStats(ItemBonus, LevelBonus);
    }
}
