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
    public float AnimTimer;
    public bool IsDying;
    public float DeathTimer;
    private const float DeathDuration = 0.4f;

    // Armed enemy fields — any enemy can get a weapon
    public bool IsArmed;
    public int WeaponSpriteIndex; // index into Weapons atlas
    public float ShootCooldown;
    public float ShootTimer;
    public int ProjectileDamage;
    public float ProjectileSpeed;
    public float PreferredRange;

    // Ranged attack config (innate)
    public int ProjectileCount = 1;
    public float ProjectileSpread;

    // Loot enemy (treasure goblin) — flees, no contact damage, big drops
    public bool IsLootEnemy;
    public float FleeTimer;         // despawn after this many seconds if not killed

    // Kamikaze fields
    public bool IsKamikaze;
    public float FuseTimer;
    public float FuseDuration;
    public float ExplosionRadius;
    public int ExplosionDamage;

    // Boss fields
    public bool IsBoss;
    public float Scale = 1f;
    public int BossSpriteType; // 0=DustWarrior, 1=Blowfish, 2=TarnishedWidow

    // Boss melee attack fields
    public bool HasMeleeAttack;
    public float MeleeAttackCooldown;
    public float MeleeAttackTimer;
    public bool IsAttacking;
    public float AttackAnimTimer;
    public float AttackAnimDuration;
    public float MeleeAttackRange;
    public int MeleeAttackDamage;
    public bool MeleeAttackHit; // prevent multi-hit per swing

    // Index into EnemyDatabase.Enemies (for STRANDED sprite lookup)
    public int DefIndex;

    // Enrage state (Relic Guardian at 50% HP)
    public bool IsEnraged;
    public bool CanEnrage; // set on init for eligible enemies

    // Frontal damage reduction
    public float FrontalDamageReduction;

    // AOE pulse attack
    public bool IsAOEPulse;
    public float PulseVFXTimer; // visual feedback timer

    // Mine-laying
    public bool LaysMines;
    public float MineLayTimer;
    public float MineLayInterval;
    public int MineDamage;
    public float MineExplosionRadius;
    public float MineLifetime;

    // Rush/lunge attack (Warrior anti-kite)
    public bool CanRush;
    public float RushCooldown;
    public float RushCooldownTimer;
    public bool IsRushing;
    public float RushTimer;
    public float RushDuration;
    public float RushSpeed;
    public Vector2 RushDirection;

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
        AnimTimer = Random.Shared.NextSingle() * 2f;
        IsDying = false;
        DeathTimer = 0;
        Active = true;
        FlashTimer = 0;
        IsBoss = false;
        Scale = 1f;
        BossSpriteType = 0;
        IsArmed = false;
        WeaponSpriteIndex = 0;
        ShootCooldown = 0;
        ShootTimer = 0;
        ProjectileDamage = 0;
        ProjectileSpeed = 0;
        PreferredRange = 0;
        ProjectileCount = 1;
        ProjectileSpread = 0;
        IsLootEnemy = false;
        FleeTimer = 0;
        IsKamikaze = false;
        FuseTimer = 0;
        FuseDuration = 0;
        ExplosionRadius = 0;
        ExplosionDamage = 0;
        HasMeleeAttack = false;
        MeleeAttackCooldown = 0;
        MeleeAttackTimer = 0;
        IsAttacking = false;
        AttackAnimTimer = 0;
        AttackAnimDuration = 0;
        MeleeAttackRange = 0;
        MeleeAttackDamage = 0;
        MeleeAttackHit = false;
        IsEnraged = false;
        CanEnrage = def.EnragesAtLowHP;
        FrontalDamageReduction = def.FrontalDamageReduction;
        IsAOEPulse = def.IsAOEPulse;
        PulseVFXTimer = 0;
        LaysMines = def.LaysMines;
        MineLayInterval = def.MineLayInterval;
        MineLayTimer = 1f + Random.Shared.NextSingle() * def.MineLayInterval; // stagger
        MineDamage = (int)(def.BaseDamage * scaleFactor * def.MineDamageMultiplier);
        MineExplosionRadius = def.MineExplosionRadius;
        MineLifetime = def.MineLifetime;
        CanRush = def.CanRush;
        IsRushing = false;
        RushCooldown = def.RushCooldown;
        RushCooldownTimer = 1f + Random.Shared.NextSingle() * def.RushCooldown; // stagger first rush
        RushTimer = 0;
        RushDuration = def.RushDuration;
        RushSpeed = def.BaseSpeed * def.RushSpeedMult;
        RushDirection = Vector2.Zero;

        // Loot enemies (flee behavior) — no contact damage, despawn timer
        if (def.Behavior == EnemyBehavior.Flee)
        {
            IsLootEnemy = true;
            FleeTimer = 12f; // 12 seconds to catch and kill
            ContactDamage = 0;
        }

        // Wire up innate attack patterns from def
        if (def.AttackType == EnemyAttackType.Melee)
        {
            HasMeleeAttack = true;
            MeleeAttackCooldown = def.AttackCooldown;
            MeleeAttackTimer = 0.5f + Random.Shared.NextSingle() * 0.5f; // stagger first attack
            MeleeAttackRange = def.AttackRange;
            MeleeAttackDamage = (int)(def.BaseDamage * scaleFactor * def.AttackDamageMultiplier);
            AttackAnimDuration = def.AttackAnimDuration;
        }
        else if (def.AttackType == EnemyAttackType.Ranged)
        {
            IsArmed = true;
            ShootCooldown = def.AttackCooldown;
            ShootTimer = Random.Shared.NextSingle() * def.AttackCooldown; // stagger
            ProjectileDamage = (int)(def.BaseDamage * scaleFactor * def.AttackDamageMultiplier);
            ProjectileSpeed = def.ProjectileSpeed;
            PreferredRange = def.AttackRange;
            AttackAnimDuration = def.AttackAnimDuration;
            ProjectileCount = def.ProjectileCount;
            ProjectileSpread = def.ProjectileSpread;
        }
        else if (def.AttackType == EnemyAttackType.Kamikaze)
        {
            IsKamikaze = true;
            FuseDuration = def.FuseDuration;
            FuseTimer = def.FuseDuration;
            ExplosionRadius = def.ExplosionRadius;
            ExplosionDamage = (int)(def.BaseDamage * scaleFactor * def.ExplosionDamageMultiplier);
        }
    }

    public void ArmWithWeapon(WeaponDef weapon, float scaleFactor)
    {
        IsArmed = true;
        WeaponSpriteIndex = weapon.SpriteIndex;
        ShootCooldown = 1f / weapon.FireRate;
        ShootTimer = Random.Shared.NextSingle() * ShootCooldown; // stagger
        ProjectileDamage = (int)(weapon.BaseDamage * 0.35f * scaleFactor); // enemies do much less damage than player
        ProjectileSpeed = 160f + Random.Shared.NextSingle() * 40f;
        PreferredRange = weapon.Range * 0.7f;
        Speed *= 0.7f; // armed enemies are slower
        XPValue += 2; // worth more XP
        GoldValue += 1;
    }

    public void InitAsBoss(EnemyDef def, Vector2 spawnPos, float scaleFactor)
    {
        Init(def, spawnPos, scaleFactor);
        IsBoss = true;
        Scale = 2f;
        Radius = def.Radius * Scale;
        XPValue = def.XPValue * 5;
        GoldValue = def.GoldValue * 5;

        // Boss melee attack (sword swing)
        HasMeleeAttack = true;
        MeleeAttackCooldown = 1.8f;
        MeleeAttackTimer = 1f; // initial delay before first attack
        MeleeAttackRange = 45f;
        MeleeAttackDamage = (int)(def.BaseDamage * scaleFactor * 2.5f);
        AttackAnimDuration = 0.5f; // 8 frames at ~16fps
    }

    public void StartDeath()
    {
        IsDying = true;
        DeathTimer = DeathDuration;
        Velocity = Vector2.Zero;
    }

    // SpriteIndex is the base (row start). Walk frames: +0, +1, +2. Death frame: +3.
    public int GetDisplaySprite()
    {
        if (IsDying) return SpriteIndex + 3;
        int frame = (int)(AnimTimer * 6f) % 3; // 6 fps walk cycle
        return SpriteIndex + frame;
    }

    public float DeathAlpha => IsDying ? Math.Clamp(DeathTimer / DeathDuration, 0f, 1f) : 1f;

    /// <summary>
    /// Apply frontal damage reduction for shield-bearing enemies.
    /// damageSourcePos is where the damage is coming from (projectile, player, etc).
    /// Returns the modified damage amount.
    /// </summary>
    public int ApplyFrontalReduction(int damage, Vector2 damageSourcePos)
    {
        if (FrontalDamageReduction <= 0) return damage;

        // "Front" = the direction the enemy is moving (velocity direction)
        if (Velocity.LengthSquared() < 1f) return damage;

        Vector2 facing = Vector2.Normalize(Velocity);
        Vector2 toSource = Vector2.Normalize(damageSourcePos - Position);

        // dot > 0 means the damage source is in front of the enemy
        float dot = Vector2.Dot(facing, toSource);
        if (dot > 0.3f) // ~72° frontal cone
        {
            float reduction = FrontalDamageReduction * Math.Clamp((dot - 0.3f) / 0.7f, 0f, 1f);
            return Math.Max(1, (int)(damage * (1f - reduction)));
        }
        return damage;
    }
}

public enum EnemyBehavior
{
    Chase,
    FastChase,
    Tank,
    Erratic,
    Flee,       // Runs away from player (treasure goblin)
}
