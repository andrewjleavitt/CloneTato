namespace CloneTato.Data;

public class WaveConfig
{
    public float Duration;
    public int TotalEnemies;
    public float BaseSpawnRate; // enemies per second (multiplied by phase curve in WaveSystem)
    public int[] EnemyTypeIndices = [];
    public float[]? SpawnWeights; // optional per-enemy-type weights (parallel to EnemyTypeIndices)
    public bool IsBossWave;
    public int GoldReward;

    /// <summary>
    /// Get wave config for a specific biome and wave number.
    /// Biome 1 = The Waste, Biome 2 = Blood Desert, Biome 3 = The Temple.
    /// </summary>
    public static WaveConfig GetWave(int biome, int wave)
    {
        return biome switch
        {
            1 => GetWasteWave(wave),
            2 => GetBloodDesertWave(wave),
            3 => GetTempleWave(wave),
            _ => GetWasteWave(wave), // fallback
        };
    }

    // Biome 1: The Waste
    // Enemies: Small Bug [1], Medium Insect [2], Rusty Robot [10], Delivery Bot [13], Spiny Beetle [8], Big Bug [7]
    // Boss: Dust Warrior
    private static WaveConfig GetWasteWave(int wave)
    {
        return wave switch
        {
            1 => new WaveConfig
            {
                Duration = 40f,
                TotalEnemies = 18,
                BaseSpawnRate = 0.55f,
                EnemyTypeIndices = [1],                    // Small Bug only — swarm intro
                GoldReward = 15,
            },
            2 => new WaveConfig
            {
                Duration = 45f,
                TotalEnemies = 22,
                BaseSpawnRate = 0.60f,
                EnemyTypeIndices = [1, 2],                 // + Medium Insect
                SpawnWeights = [0.65f, 0.35f],
                GoldReward = 18,
            },
            3 => new WaveConfig
            {
                Duration = 50f,
                TotalEnemies = 26,
                BaseSpawnRate = 0.65f,
                EnemyTypeIndices = [1, 2],                 // Same roster, real pressure
                SpawnWeights = [0.50f, 0.50f],
                GoldReward = 22,
            },
            4 => new WaveConfig
            {
                Duration = 55f,
                TotalEnemies = 28,
                BaseSpawnRate = 0.65f,
                EnemyTypeIndices = [1, 2, 10],             // + Rusty Robot
                SpawnWeights = [0.40f, 0.30f, 0.30f],
                GoldReward = 25,
            },
            5 => new WaveConfig
            {
                Duration = 60f,
                TotalEnemies = 32,
                BaseSpawnRate = 0.70f,
                EnemyTypeIndices = [1, 2, 10, 13],         // + Delivery Bot
                SpawnWeights = [0.30f, 0.25f, 0.20f, 0.25f],
                GoldReward = 28,
            },
            6 => new WaveConfig
            {
                Duration = 65f,
                TotalEnemies = 35,
                BaseSpawnRate = 0.72f,
                EnemyTypeIndices = [1, 2, 8, 10, 13],      // + Spiny Beetle
                SpawnWeights = [0.22f, 0.18f, 0.22f, 0.16f, 0.22f],
                GoldReward = 32,
            },
            7 => new WaveConfig
            {
                Duration = 75f,
                TotalEnemies = 38,
                BaseSpawnRate = 0.70f,
                EnemyTypeIndices = [1, 2, 7, 8, 10, 13],   // + Big Bug (full roster)
                SpawnWeights = [0.18f, 0.15f, 0.15f, 0.17f, 0.15f, 0.20f],
                GoldReward = 35,
            },
            8 => new WaveConfig
            {
                Duration = 80f,
                TotalEnemies = 45,
                BaseSpawnRate = 0.78f,
                EnemyTypeIndices = [1, 2, 7, 8, 10, 13],   // All 6 — density ramp
                SpawnWeights = [0.18f, 0.15f, 0.18f, 0.17f, 0.12f, 0.20f],
                GoldReward = 40,
            },
            9 => new WaveConfig
            {
                Duration = 90f,
                TotalEnemies = 55,
                BaseSpawnRate = 0.85f,
                EnemyTypeIndices = [1, 2, 7, 8, 10, 13],   // All 6, heavy — pre-boss gauntlet
                SpawnWeights = [0.16f, 0.14f, 0.22f, 0.18f, 0.12f, 0.18f],
                GoldReward = 45,
            },
            10 => new WaveConfig
            {
                Duration = 90f,
                TotalEnemies = 18,                          // Steady adds during boss fight
                BaseSpawnRate = 0.30f,
                EnemyTypeIndices = [1, 10],                 // Small Bugs + Rusty Robots as adds
                SpawnWeights = [0.70f, 0.30f],
                IsBossWave = true,
                GoldReward = 60,
            },
            _ => GetWasteWave(Math.Clamp(wave, 1, 10)),    // clamp to valid range
        };
    }

    // Biome 2: Blood Desert — placeholder, returns Waste waves as fallback
    // Enemies: Tribe Hunter [0], Archer [4], Guard [5], Tribe Warrior [3], Warrior [6], Relic Guardian [9]
    // Boss: Blowfish
    private static WaveConfig GetBloodDesertWave(int wave)
    {
        // TODO: Implement Blood Desert wave definitions
        return GetWasteWave(wave);
    }

    // Biome 3: The Temple — placeholder, returns Waste waves as fallback
    // Enemies: Hooded Minion [14], Circle Bot [12], Ranged Minion [16], Guard Robot [11], Bomb Minion [15], Planter Bot [17]
    // Boss: Tarnished Widow
    private static WaveConfig GetTempleWave(int wave)
    {
        // TODO: Implement Temple wave definitions
        return GetWasteWave(wave);
    }
}
