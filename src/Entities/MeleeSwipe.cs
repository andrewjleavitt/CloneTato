using System.Numerics;

namespace CloneTato.Entities;

/// <summary>
/// Visual-only entity for melee attack arc rendering. Damage is applied instantly
/// when the swing is created; this just shows the visual.
/// </summary>
public class MeleeSwipe : Entity
{
    public float ArcAngle;      // center angle of the swipe
    public float ArcWidth;      // total arc width in radians
    public float SwipeRadius;   // how far the swipe reaches
    public float Lifetime;
    public float MaxLifetime;
    public Raylib_cs.Color SwipeColor;

    public void Init(Vector2 origin, float angle, float arcWidth, float radius, Raylib_cs.Color color)
    {
        Position = origin;
        ArcAngle = angle;
        ArcWidth = arcWidth;
        SwipeRadius = radius;
        MaxLifetime = 0.2f;
        Lifetime = MaxLifetime;
        SwipeColor = color;
        Active = true;
    }

    public override void Update(float dt)
    {
        Lifetime -= dt;
        if (Lifetime <= 0) Active = false;
    }

    public float Alpha => Math.Clamp(Lifetime / MaxLifetime, 0f, 1f);
}
