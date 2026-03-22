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

    // Biome 2: Blood Desert
    // Core: Tribe Warrior [3], Archer [4], Guard [5], Warrior [6], Relic Guardian [9]
    // Filler: Small Bug [1] (fast melee), Spiny Beetle [8] (ranged crossover)
    // Boss: Blowfish
    private static WaveConfig GetBloodDesertWave(int wave)
    {
        return wave switch
        {
            1 => new WaveConfig
            {
                Duration = 40f,
                TotalEnemies = 16,
                BaseSpawnRate = 0.50f,
                EnemyTypeIndices = [4],                    // Archer only — ranged intro
                GoldReward = 18,
            },
            2 => new WaveConfig
            {
                Duration = 45f,
                TotalEnemies = 20,
                BaseSpawnRate = 0.55f,
                EnemyTypeIndices = [4, 6],                 // + Warrior (melee pressure)
                SpawnWeights = [0.55f, 0.45f],
                GoldReward = 22,
            },
            3 => new WaveConfig
            {
                Duration = 50f,
                TotalEnemies = 24,
                BaseSpawnRate = 0.60f,
                EnemyTypeIndices = [4, 5, 6],              // + Guard (tank)
                SpawnWeights = [0.35f, 0.30f, 0.35f],
                GoldReward = 25,
            },
            4 => new WaveConfig
            {
                Duration = 55f,
                TotalEnemies = 28,
                BaseSpawnRate = 0.65f,
                EnemyTypeIndices = [3, 4, 5, 6],           // + Tribe Warrior (melee)
                SpawnWeights = [0.25f, 0.25f, 0.25f, 0.25f],
                GoldReward = 28,
            },
            5 => new WaveConfig
            {
                Duration = 60f,
                TotalEnemies = 32,
                BaseSpawnRate = 0.68f,
                EnemyTypeIndices = [1, 3, 4, 5, 6],        // + Small Bug (fast filler)
                SpawnWeights = [0.20f, 0.20f, 0.20f, 0.20f, 0.20f],
                GoldReward = 32,
            },
            6 => new WaveConfig
            {
                Duration = 65f,
                TotalEnemies = 36,
                BaseSpawnRate = 0.72f,
                EnemyTypeIndices = [1, 3, 4, 5, 6, 8],     // + Spiny Beetle (ranged crossover)
                SpawnWeights = [0.14f, 0.18f, 0.18f, 0.18f, 0.18f, 0.14f],
                GoldReward = 35,
            },
            7 => new WaveConfig
            {
                Duration = 75f,
                TotalEnemies = 40,
                BaseSpawnRate = 0.72f,
                EnemyTypeIndices = [1, 3, 4, 5, 6, 8, 9],  // + Relic Guardian (full roster)
                SpawnWeights = [0.10f, 0.14f, 0.16f, 0.14f, 0.16f, 0.14f, 0.16f],
                GoldReward = 40,
            },
            8 => new WaveConfig
            {
                Duration = 80f,
                TotalEnemies = 48,
                BaseSpawnRate = 0.80f,
                EnemyTypeIndices = [3, 4, 5, 6, 8, 9],     // Core roster, dense
                SpawnWeights = [0.16f, 0.18f, 0.16f, 0.18f, 0.16f, 0.16f],
                GoldReward = 45,
            },
            9 => new WaveConfig
            {
                Duration = 90f,
                TotalEnemies = 58,
                BaseSpawnRate = 0.88f,
                EnemyTypeIndices = [3, 4, 5, 6, 8, 9],     // All core, heavy — pre-boss gauntlet
                SpawnWeights = [0.14f, 0.16f, 0.14f, 0.16f, 0.16f, 0.24f],  // heavy Relic Guardian
                GoldReward = 50,
            },
            10 => new WaveConfig
            {
                Duration = 100f,
                TotalEnemies = 20,
                BaseSpawnRate = 0.32f,
                EnemyTypeIndices = [4, 6],                  // Archers + Warriors as adds
                SpawnWeights = [0.55f, 0.45f],
                IsBossWave = true,
                GoldReward = 70,
            },
            _ => GetBloodDesertWave(Math.Clamp(wave, 1, 10)),
        };
    }

    // Biome 3: The Temple
    // Core: Hooded Minion [14], Circle Bot [12], Ranged Minion [16], Guard Robot [11], Bomb Minion [15], Planter Bot [17]
    // Filler: Small Bug [1] (fast filler)
    // Boss: Tarnished Widow
    private static WaveConfig GetTempleWave(int wave)
    {
        return wave switch
        {
            1 => new WaveConfig
            {
                Duration = 40f,
                TotalEnemies = 16,
                BaseSpawnRate = 0.50f,
                EnemyTypeIndices = [14],                   // Hooded Minion only — assassin intro
                GoldReward = 20,
            },
            2 => new WaveConfig
            {
                Duration = 45f,
                TotalEnemies = 20,
                BaseSpawnRate = 0.55f,
                EnemyTypeIndices = [14, 16],               // + Ranged Minion (suppressor)
                SpawnWeights = [0.55f, 0.45f],
                GoldReward = 24,
            },
            3 => new WaveConfig
            {
                Duration = 50f,
                TotalEnemies = 24,
                BaseSpawnRate = 0.60f,
                EnemyTypeIndices = [12, 14, 16],           // + Circle Bot (AOE threat)
                SpawnWeights = [0.30f, 0.35f, 0.35f],
                GoldReward = 28,
            },
            4 => new WaveConfig
            {
                Duration = 55f,
                TotalEnemies = 28,
                BaseSpawnRate = 0.65f,
                EnemyTypeIndices = [11, 12, 14, 16],       // + Guard Robot (tank)
                SpawnWeights = [0.25f, 0.20f, 0.30f, 0.25f],
                GoldReward = 32,
            },
            5 => new WaveConfig
            {
                Duration = 60f,
                TotalEnemies = 32,
                BaseSpawnRate = 0.68f,
                EnemyTypeIndices = [11, 12, 14, 15, 16],   // + Bomb Minion (kamikaze)
                SpawnWeights = [0.18f, 0.18f, 0.24f, 0.22f, 0.18f],
                GoldReward = 35,
            },
            6 => new WaveConfig
            {
                Duration = 65f,
                TotalEnemies = 36,
                BaseSpawnRate = 0.72f,
                EnemyTypeIndices = [11, 12, 14, 15, 16, 17], // + Planter Bot (full roster)
                SpawnWeights = [0.16f, 0.16f, 0.20f, 0.16f, 0.16f, 0.16f],
                GoldReward = 38,
            },
            7 => new WaveConfig
            {
                Duration = 75f,
                TotalEnemies = 42,
                BaseSpawnRate = 0.75f,
                EnemyTypeIndices = [1, 11, 12, 14, 15, 16, 17], // + Snake filler
                SpawnWeights = [0.10f, 0.14f, 0.14f, 0.18f, 0.14f, 0.16f, 0.14f],
                GoldReward = 42,
            },
            8 => new WaveConfig
            {
                Duration = 80f,
                TotalEnemies = 50,
                BaseSpawnRate = 0.82f,
                EnemyTypeIndices = [11, 12, 14, 15, 16, 17], // Core roster, dense
                SpawnWeights = [0.16f, 0.16f, 0.20f, 0.16f, 0.16f, 0.16f],
                GoldReward = 48,
            },
            9 => new WaveConfig
            {
                Duration = 90f,
                TotalEnemies = 60,
                BaseSpawnRate = 0.90f,
                EnemyTypeIndices = [11, 12, 14, 15, 16, 17], // All core, heavy — pre-boss gauntlet
                SpawnWeights = [0.18f, 0.18f, 0.16f, 0.18f, 0.14f, 0.16f],  // heavy robots
                GoldReward = 55,
            },
            10 => new WaveConfig
            {
                Duration = 110f,
                TotalEnemies = 22,
                BaseSpawnRate = 0.32f,
                EnemyTypeIndices = [14, 15],               // Hooded + Bomb Minions as adds
                SpawnWeights = [0.60f, 0.40f],
                IsBossWave = true,
                GoldReward = 80,
            },
            _ => GetTempleWave(Math.Clamp(wave, 1, 10)),
        };
    }
}
