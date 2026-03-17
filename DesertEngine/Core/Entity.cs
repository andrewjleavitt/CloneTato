using System.Numerics;

namespace DesertEngine.Core;

public class Entity
{
    public Vector2 Position;
    public Vector2 Velocity;
    public float Radius = 8f;
    public bool Active;
    public int SpriteIndex;
    public float Rotation;
    public float FlashTimer;

    public virtual void Update(float dt)
    {
        Position += Velocity * dt;
        if (FlashTimer > 0) FlashTimer -= dt;
    }
}
