using System.Numerics;

namespace DesertEngine.Core;

public class Projectile : Entity
{
    public int Damage;
    public float Lifetime;
    public float MaxLifetime;
    public int PierceCount;
    public Raylib_cs.Color ProjectileColor;
    public float ExplosionRadius;

    public void Init(Vector2 pos, Vector2 vel, int damage, float lifetime, int pierce,
        Raylib_cs.Color color, float explosionRadius = 0f)
    {
        Position = pos;
        Velocity = vel;
        Damage = damage;
        Lifetime = lifetime;
        MaxLifetime = lifetime;
        PierceCount = pierce;
        ProjectileColor = color;
        ExplosionRadius = explosionRadius;
        Radius = explosionRadius > 0 ? 4f : 3f;
        Active = true;
        FlashTimer = 0;
        SpriteIndex = 0;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        Lifetime -= dt;
        if (Lifetime <= 0) Active = false;
    }

    public bool IsExplosive => ExplosionRadius > 0;
}
