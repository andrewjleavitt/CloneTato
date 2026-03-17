using System.Numerics;

namespace CloneTato.Entities;

public class Mine : Entity
{
    public int Damage;
    public float ExplosionRadius;
    public float ProximityRadius;
    public float ArmTimer; // small delay before mine activates
    public int WeaponSpriteIndex;

    public void Init(Vector2 pos, int damage, float explosionRadius, float proximity, int spriteIndex)
    {
        Position = pos;
        Damage = damage;
        ExplosionRadius = explosionRadius;
        ProximityRadius = proximity;
        WeaponSpriteIndex = spriteIndex;
        Radius = 5f;
        ArmTimer = 0.4f;
        Active = true;
        Velocity = Vector2.Zero;
    }

    public override void Update(float dt)
    {
        if (ArmTimer > 0) ArmTimer -= dt;
    }
}
