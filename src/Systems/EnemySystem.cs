using System.Numerics;
using CloneTato.Core;
using CloneTato.Data;
using CloneTato.Entities;

namespace CloneTato.Systems;

public static class EnemySystem
{
    public static void Update(float dt, GameState state)
    {
        var playerPos = state.Player.Position;
        float time = (float)Raylib_cs.Raylib.GetTime();

        for (int i = 0; i < state.Enemies.Count; i++)
        {
            var enemy = state.Enemies[i];
            if (!enemy.Active) continue;

            // Knockback
            if (enemy.KnockbackTimer > 0)
            {
                enemy.KnockbackTimer -= dt;
                enemy.Position += enemy.KnockbackVelocity * dt;
                enemy.KnockbackVelocity *= 0.85f;
                continue;
            }

            // Movement toward player
            Vector2 dir = playerPos - enemy.Position;
            float dist = dir.Length();
            if (dist > 1f)
            {
                dir /= dist; // normalize

                // Erratic enemies add sine wave perpendicular movement
                if (enemy.Behavior == EnemyBehavior.Erratic)
                {
                    float sine = MathF.Sin(time * 4f + enemy.SineOffset) * 0.6f;
                    Vector2 perp = new(-dir.Y, dir.X);
                    dir += perp * sine;
                    dir = Vector2.Normalize(dir);
                }

                enemy.Velocity = dir * enemy.Speed;
            }

            enemy.Position += enemy.Velocity * dt;

            // Clamp to arena (enemies can be slightly outside to spawn)
            enemy.Position.X = Math.Clamp(enemy.Position.X, -20f, Constants.ArenaWidth + 20f);
            enemy.Position.Y = Math.Clamp(enemy.Position.Y, -20f, Constants.ArenaHeight + 20f);

            if (enemy.FlashTimer > 0) enemy.FlashTimer -= dt;
        }
    }

    public static void SpawnEnemy(GameState state, int waveNumber)
    {
        var config = state.CurrentWaveConfig;
        if (config == null) return;

        var enemy = state.GetInactiveEnemy();
        if (enemy == null) return;

        // Pick random enemy type from this wave's allowed types
        int typeIndex = config.EnemyTypeIndices[Random.Shared.Next(config.EnemyTypeIndices.Length)];
        var def = EnemyDatabase.Enemies[typeIndex];

        // Scale stats based on wave
        float scaleFactor = 1f + (waveNumber - 1) * 0.12f;

        // Boss spawn on boss waves (last enemy of the wave)
        if (config.IsBossWave && state.EnemiesSpawnedThisWave == config.TotalEnemies - 1)
        {
            scaleFactor *= 4f;
        }

        // Spawn from random arena edge
        Vector2 spawnPos = GetEdgeSpawnPosition(state.Player.Position);
        enemy.Init(def, spawnPos, scaleFactor);

        state.EnemiesSpawnedThisWave++;
    }

    private static Vector2 GetEdgeSpawnPosition(Vector2 playerPos)
    {
        int edge = Random.Shared.Next(4);
        float margin = 30f;
        return edge switch
        {
            0 => new Vector2(Random.Shared.NextSingle() * Constants.ArenaWidth, -margin), // top
            1 => new Vector2(Random.Shared.NextSingle() * Constants.ArenaWidth, Constants.ArenaHeight + margin), // bottom
            2 => new Vector2(-margin, Random.Shared.NextSingle() * Constants.ArenaHeight), // left
            _ => new Vector2(Constants.ArenaWidth + margin, Random.Shared.NextSingle() * Constants.ArenaHeight), // right
        };
    }
}
