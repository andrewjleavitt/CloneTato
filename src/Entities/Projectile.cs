using System.Numerics;

namespace CloneTato.Entities;

public class Projectile : Entity
{
    public int Damage;
    public float Lifetime;
    public float MaxLifetime;
    public int PierceCount;
    public Raylib_cs.Color ProjectileColor;

    public void Init(Vector2 pos, Vector2 vel, int damage, float lifetime, int pierce, Raylib_cs.Color color)
    {
        Position = pos;
        Velocity = vel;
        Damage = damage;
        Lifetime = lifetime;
        MaxLifetime = lifetime;
        PierceCount = pierce;
        ProjectileColor = color;
        Radius = 3f;
        Active = true;
        FlashTimer = 0;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        Lifetime -= dt;
        if (Lifetime <= 0) Active = false;
    }
}
