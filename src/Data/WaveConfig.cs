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

        // Progressively unlock enemy types
        config.EnemyTypeIndices = wave switch
        {
            <= 3 => [0],                  // Tribe Hunter only
            <= 5 => [0, 1],              // + Small Bug
            <= 8 => [0, 1, 2],           // + Medium Insect
            <= 11 => [0, 1, 2, 3],       // + Tribe Warrior
            <= 14 => [0, 1, 2, 3, 4],    // + Archer
            <= 17 => [0, 1, 2, 3, 4, 5], // + Guard
            _ => [0, 1, 2, 3, 4, 5, 6],  // + Warrior
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
