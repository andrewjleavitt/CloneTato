namespace CloneTato.Data;

public class WaveConfig
{
    public float Duration;
    public int TotalEnemies;
    public float SpawnRate; // enemies per second
    public int[] EnemyTypeIndices = [];
    public bool IsBossWave;
    public int GoldReward;

    public static WaveConfig GetWave(int wave)
    {
        var config = new WaveConfig
        {
            Duration = Constants.WaveBaseDuration + wave * 2f,
            TotalEnemies = 12 + wave * 7,
            SpawnRate = 0.8f + wave * 0.18f,
            IsBossWave = wave >= 3 && (wave % 3 == 0 || wave % 5 == 0),
            GoldReward = 20 + wave * 5,
        };

        // Progressively unlock enemy types across waves
        config.EnemyTypeIndices = wave switch
        {
            1 => [0, 1],                                   // Tribe Hunter + Small Bug
            2 => [0, 1, 10],                               // + Rusty Robot
            3 => [0, 1, 2, 10],                            // + Medium Insect
            4 => [0, 1, 2, 4, 10],                         // + Archer
            5 => [0, 1, 2, 8, 10, 13],                     // + Spiny Beetle + Delivery Bot
            6 => [0, 1, 2, 3, 8, 10, 13],                  // + Tribe Warrior
            7 => [0, 1, 2, 3, 7, 8, 10, 13],               // + Big Bug
            8 => [0, 1, 2, 3, 4, 7, 8, 10, 12, 13],        // + Circle Bot
            9 => [0, 1, 2, 3, 7, 8, 10, 12, 13, 15],       // + Bomb Minion
            10 => [0, 1, 2, 3, 7, 8, 10, 11, 12, 13, 14, 15], // + Guard Robot + Hooded Minion
            11 => [0, 1, 2, 3, 5, 7, 8, 10, 11, 12, 13, 14, 15], // + Guard
            12 => [0, 1, 2, 3, 5, 6, 7, 8, 10, 11, 12, 13, 14, 15, 16], // + Warrior + Ranged Minion
            13 => [0, 1, 2, 3, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16], // + Relic Guardian
            _ => [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16], // All enemies
        };

        // Boss waves get more enemies and are longer
        if (config.IsBossWave)
        {
            config.TotalEnemies += 5;
            config.Duration += 10f;
        }

        return config;
    }
}
