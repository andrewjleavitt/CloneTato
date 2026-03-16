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
            IsBossWave = wave % 5 == 0,
            GoldReward = 20 + wave * 5,
        };

        // Progressively unlock enemy types
        config.EnemyTypeIndices = wave switch
        {
            <= 3 => [0],                  // Scorpion only
            <= 7 => [0, 1],              // + Snake
            <= 12 => [0, 1, 2],          // + Bat
            _ => [0, 1, 2, 3],           // + Beetle
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
