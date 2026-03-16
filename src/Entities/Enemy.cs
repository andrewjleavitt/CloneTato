using System.Numerics;
using CloneTato.Data;

namespace CloneTato.Entities;

public class Enemy : Entity
{
    public int CurrentHP;
    public int MaxHP;
    public float Speed;
    public int ContactDamage;
    public int XPValue;
    public int GoldValue;
    public float KnockbackTimer;
    public Vector2 KnockbackVelocity;
    public EnemyBehavior Behavior;
    public float SineOffset; // for erratic movement

    public void Init(EnemyDef def, Vector2 spawnPos, float scaleFactor = 1f)
    {
        Position = spawnPos;
        SpriteIndex = def.SpriteIndex;
        MaxHP = (int)(def.BaseHP * scaleFactor);
        CurrentHP = MaxHP;
        Speed = def.BaseSpeed * (0.9f + Random.Shared.NextSingle() * 0.2f);
        ContactDamage = (int)(def.BaseDamage * scaleFactor);
        XPValue = def.XPValue;
        GoldValue = def.GoldValue;
        Radius = def.Radius;
        Behavior = def.Behavior;
        SineOffset = Random.Shared.NextSingle() * MathF.PI * 2f;
        KnockbackTimer = 0;
        Active = true;
        FlashTimer = 0;
    }
}

public enum EnemyBehavior
{
    Chase,
    FastChase,
    Tank,
    Erratic
}
