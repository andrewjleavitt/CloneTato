using CloneTato.Core;

namespace CloneTato.Systems;

public static class WaveSystem
{
    public static void Update(float dt, GameState state)
    {
        if (!state.WaveActive) return;

        var config = state.CurrentWaveConfig;
        if (config == null) return;

        state.WaveTimer -= dt;

        // Spawn enemies
        if (state.EnemiesSpawnedThisWave < config.TotalEnemies)
        {
            state.SpawnAccumulator += config.SpawnRate * dt;
            while (state.SpawnAccumulator >= 1f && state.EnemiesSpawnedThisWave < config.TotalEnemies)
            {
                EnemySystem.SpawnEnemy(state, state.CurrentWave);
                state.SpawnAccumulator -= 1f;
            }
        }

        // Wave complete: timer expired AND all enemies dead
        bool allSpawned = state.EnemiesSpawnedThisWave >= config.TotalEnemies;
        bool allDead = state.ActiveEnemyCount() == 0;
        bool timerDone = state.WaveTimer <= 0;

        if (allSpawned && allDead && timerDone)
        {
            state.WaveActive = false;
            state.Gold += config.GoldReward;
        }
        else if (timerDone && !allSpawned)
        {
            // Timer ran out but not all spawned: force-spawn remaining faster
            state.SpawnAccumulator += config.SpawnRate * 3f * dt;
        }
    }
}
