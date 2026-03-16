using System.Numerics;

namespace CloneTato.Entities;

public class XPOrb : Entity
{
    public int XPValue;
    public float SpawnTimer;

    public void Init(Vector2 pos, int value)
    {
        Position = pos;
        XPValue = value;
        Radius = 4f;
        Active = true;
        SpawnTimer = 0.3f;
        // scatter slightly
        float angle = Random.Shared.NextSingle() * MathF.PI * 2f;
        float force = 30f + Random.Shared.NextSingle() * 40f;
        Velocity = new Vector2(MathF.Cos(angle) * force, MathF.Sin(angle) * force);
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        Velocity *= 0.92f; // friction
        if (SpawnTimer > 0) SpawnTimer -= dt;
    }
}
