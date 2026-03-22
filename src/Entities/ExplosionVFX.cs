using System.Numerics;

namespace CloneTato.Entities;

public struct ExplosionVFX
{
    public bool Active;
    public Vector2 Position;
    public float Timer;
    public float Duration;
    public float Radius;
    public int EnemyDefIndex; // for sprite lookup (-1 = generic)

    public void Init(Vector2 pos, float radius, float duration, int defIndex = -1)
    {
        Active = true;
        Position = pos;
        Timer = duration;
        Duration = duration;
        Radius = radius;
        EnemyDefIndex = defIndex;
    }

    public float Progress => 1f - Timer / Duration;
    public float Alpha => Math.Clamp(Timer / Duration, 0f, 1f);
}
