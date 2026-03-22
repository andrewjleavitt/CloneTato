using System.Numerics;

namespace CloneTato.Entities;

public class EnemyMine : Entity
{
    public int Damage;
    public float ExplosionRadius;
    public float ProximityRadius;
    public float ArmTimer;
    public float Lifetime; // despawn after this many seconds

    public void Init(Vector2 pos, int damage, float explosionRadius, float proximity, float lifetime)
    {
        Position = pos;
        Damage = damage;
        ExplosionRadius = explosionRadius;
        ProximityRadius = proximity;
        Lifetime = lifetime;
        Radius = 5f;
        ArmTimer = 0.5f;
        Active = true;
        Velocity = Vector2.Zero;
    }

    public override void Update(float dt)
    {
        if (ArmTimer > 0) ArmTimer -= dt;
        Lifetime -= dt;
        if (Lifetime <= 0) Active = false;
    }

    public bool IsArmed => ArmTimer <= 0;
}
