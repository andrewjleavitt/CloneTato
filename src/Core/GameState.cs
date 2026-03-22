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

    // Visual-only ground decorations (no collision)
    public List<GroundScatter> GroundScatterProps = new();

    // Explosion VFX pool
    public const int MaxExplosionVFX = 10;
    public ExplosionVFX[] ExplosionEffects = new ExplosionVFX[MaxExplosionVFX];

    // Enemy mines (placed by Planter Bot)
    public const int MaxEnemyMines = 30;
    public List<EnemyMine> EnemyMines = new();

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

    public int CurrentBiome = 1;
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

    // Combat feel — set by systems, consumed by PlayingScreen
    public float PendingHitstop;        // seconds of hitstop to apply
    public float PendingShakeDuration;
    public float PendingShakeIntensity;

    public void RequestHitstop(float duration)
    {
        if (duration > PendingHitstop) PendingHitstop = duration;
    }

    public void RequestScreenShake(float duration, float intensity)
    {
        if (intensity > PendingShakeIntensity)
        {
            PendingShakeDuration = duration;
            PendingShakeIntensity = intensity;
        }
    }

    // Stats tracking
    public int TotalEnemiesKilled;
    public int TotalDamageDealt;
    public float TotalTimeSurvived;

    // Combo kill system
    public int ComboCount;
    public float ComboTimer;
    public const float ComboWindow = 1.5f; // seconds between kills to maintain combo
    public int BestCombo;

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

        EnemyMines.Clear();
        for (int i = 0; i < MaxEnemyMines; i++)
            EnemyMines.Add(new EnemyMine());

        MeleeSwipes.Clear();
        for (int i = 0; i < MaxMeleeSwipes; i++)
            MeleeSwipes.Add(new MeleeSwipe());
    }

    public void StartNewRun(CharacterDef character)
    {
        InitPools();
        Player = new Player();
        Player.Init(character);

        // Set active hero sprite based on character type
        Assets.HeroSprite = character.HeroType switch
        {
            HeroType.BladeDancer => Assets.HeroSwordSprite ?? Assets.HeroGunSprite,
            HeroType.Drifter => Assets.StarterHeroSprite ?? Assets.HeroGunSprite,
            _ => Assets.HeroGunSprite,
        };

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

        CurrentBiome = 1;
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
        ComboCount = 0;
        ComboTimer = 0;
        BestCombo = 0;
    }

    public void StartWave()
    {
        CurrentWave++;
        CurrentWaveConfig = WaveConfig.GetWave(CurrentBiome, CurrentWave);
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

    public EnemyMine? GetInactiveEnemyMine()
    {
        for (int i = 0; i < EnemyMines.Count; i++)
            if (!EnemyMines[i].Active) return EnemyMines[i];
        return null;
    }

    public void SpawnExplosionVFX(Vector2 pos, float radius, int defIndex = -1)
    {
        for (int i = 0; i < ExplosionEffects.Length; i++)
        {
            if (!ExplosionEffects[i].Active)
            {
                ExplosionEffects[i].Init(pos, radius, 0.5f, defIndex);
                return;
            }
        }
    }

    public void HandleEnemyDeath(Enemy enemy)
    {
        enemy.StartDeath();
        TotalEnemiesKilled++;
        EnemiesKilledThisWave++;

        // Combo tracking
        if (ComboTimer > 0)
            ComboCount++;
        else
            ComboCount = 1;
        ComboTimer = ComboWindow;
        if (ComboCount > BestCombo) BestCombo = ComboCount;

        // Combo multiplier: 1.0x at 1 kill, scales up with kills
        // 5 kills = 1.25x, 10 kills = 1.5x, 20 kills = 2.0x
        float comboMult = 1f + Math.Min(ComboCount - 1, 20) * 0.05f;

        // Kamikaze enemies explode on death (smaller than fuse explosion)
        if (enemy.IsKamikaze && enemy.FuseTimer > 0)
        {
            // Death explosion — 60% radius and damage of normal
            float radius = enemy.ExplosionRadius * 0.6f;
            int damage = (int)(enemy.ExplosionDamage * 0.6f);

            // Damage nearby enemies
            for (int j = 0; j < Enemies.Count; j++)
            {
                var other = Enemies[j];
                if (!other.Active || other.IsDying || other == enemy) continue;
                float d = Vector2.Distance(enemy.Position, other.Position);
                if (d < radius)
                {
                    int edm = Math.Max(1, (int)(damage * 0.5f));
                    other.CurrentHP -= edm;
                    other.FlashTimer = 0.1f;
                    if (other.IsKamikaze && other.FuseTimer > 0.2f)
                    {
                        other.FuseArmed = true;
                        other.FuseTimer = 0.2f;
                    }
                    if (other.CurrentHP <= 0 && !other.IsDying)
                        HandleEnemyDeath(other);
                }
            }

            SpawnExplosionVFX(enemy.Position, radius);
            RequestScreenShake(0.12f, 2f);
            Assets.PlaySoundVariant("explosion", 0.3f);
        }

        // Loot enemies burst extra drops in a ring
        if (enemy.IsLootEnemy)
        {
            // Scatter XP orbs in a burst
            int orbCount = 6;
            for (int o = 0; o < orbCount; o++)
            {
                var orb = GetInactiveXPOrb();
                if (orb == null) break;
                float angle = o * (MathF.PI * 2f / orbCount);
                var offset = new Vector2(MathF.Cos(angle), MathF.Sin(angle)) * 12f;
                orb.Init(enemy.Position + offset, Math.Max(1, (int)(enemy.XPValue * comboMult / orbCount)));
            }
            Gold += (int)(enemy.GoldValue * comboMult);

            // Always drop a health pickup
            var hp = GetInactiveHealthPickup();
            hp?.Init(enemy.Position, 15);
            return;
        }

        // XP orbs (with combo multiplier)
        int xpValue = (int)(enemy.XPValue * comboMult);
        int normalOrbCount = 1 + xpValue / 3;
        for (int o = 0; o < normalOrbCount; o++)
        {
            var orb = GetInactiveXPOrb();
            orb?.Init(enemy.Position, Math.Max(1, xpValue / normalOrbCount));
        }
        Gold += (int)(enemy.GoldValue * comboMult);

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

    // Hitbox radii for STRANDED obstacle textures (matching ObstacleTextures load order)
    // Tree1-5(106x72), Big Rock(107x53), Skull(139x89), Skull Grassy(139x92), statue(53x54)
    private static readonly float[] StrandedObstacleRadii = { 20f, 20f, 20f, 20f, 20f, 22f, 25f, 25f, 12f };

    private void GenerateArena()
    {
        Obstacles.Clear();
        Barrels.Clear();
        TerrainZones.Clear();
        GroundScatterProps.Clear();

        var rng = Random.Shared;
        float centerX = Constants.ArenaWidth / 2f;
        float centerY = Constants.ArenaHeight / 2f;
        float safeRadius = 80f; // keep spawn area clear

        bool useStranded = Assets.HasStrandedTerrain;
        int obstacleCount = rng.Next(8, 15);

        for (int i = 0; i < obstacleCount; i++)
        {
            for (int attempt = 0; attempt < 20; attempt++)
            {
                float x = rng.NextSingle() * (Constants.ArenaWidth - 80) + 40;
                float y = rng.NextSingle() * (Constants.ArenaHeight - 80) + 40;

                int texIdx = useStranded ? rng.Next(Assets.ObstacleTextures.Length) : 0;
                float radius = useStranded ? StrandedObstacleRadii[texIdx] : 7f;

                if (Vector2.Distance(new Vector2(x, y), new Vector2(centerX, centerY)) < safeRadius + radius)
                    continue;

                bool overlaps = false;
                foreach (var other in Obstacles)
                {
                    if (Vector2.Distance(new Vector2(x, y), other.Position) < radius + other.Radius + 20f)
                    { overlaps = true; break; }
                }
                if (overlaps) continue;

                if (useStranded)
                {
                    Obstacles.Add(new Obstacle
                    {
                        Position = new Vector2(x, y),
                        Radius = radius,
                        TextureIndex = texIdx,
                        UseStranded = true,
                        Active = true,
                    });
                }
                else
                {
                    int[] treeSprites = { 3 * 18 + 3, 3 * 18 + 8 };
                    Obstacles.Add(new Obstacle
                    {
                        Position = new Vector2(x, y),
                        Radius = radius,
                        SpriteIndex = treeSprites[rng.Next(treeSprites.Length)],
                        Active = true,
                    });
                }
                break;
            }
        }

        // Ground scatter decorations (non-collidable)
        if (useStranded)
        {
            // Small subtle scatter (grass, pebbles, gravel) — dense
            if (Assets.GroundScatterTextures.Length > 0)
            {
                int scatterCount = rng.Next(40, 70);
                for (int i = 0; i < scatterCount; i++)
                {
                    float x = rng.NextSingle() * Constants.ArenaWidth;
                    float y = rng.NextSingle() * Constants.ArenaHeight;
                    GroundScatterProps.Add(new GroundScatter
                    {
                        Position = new Vector2(x, y),
                        TextureIndex = rng.Next(Assets.GroundScatterTextures.Length),
                        FlipH = rng.NextSingle() < 0.5f,
                    });
                }
            }

            // Large accent props (swords, hands, poles) — sparse
            if (Assets.LargeScatterTextures.Length > 0)
            {
                int accentCount = rng.Next(5, 12);
                for (int i = 0; i < accentCount; i++)
                {
                    float x = rng.NextSingle() * (Constants.ArenaWidth - 40) + 20;
                    float y = rng.NextSingle() * (Constants.ArenaHeight - 40) + 20;
                    GroundScatterProps.Add(new GroundScatter
                    {
                        Position = new Vector2(x, y),
                        TextureIndex = rng.Next(Assets.LargeScatterTextures.Length),
                        FlipH = rng.NextSingle() < 0.5f,
                        IsLarge = true,
                    });
                }
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
