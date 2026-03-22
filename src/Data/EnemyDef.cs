namespace CloneTato.Data;

public enum EnemyAttackType
{
    Bump,       // Contact damage only (default)
    Melee,      // Initiates melee attack at range with cooldown
    Ranged,     // Fires innate projectiles, kites at preferred range
    Kamikaze,   // Fuse timer → explosion on contact or timeout
}

public class EnemyDef
{
    public string Name = "";
    public int SpriteIndex;
    public int BaseHP;
    public float BaseSpeed;
    public int BaseDamage;
    public int XPValue;
    public int GoldValue;
    public float Radius = 10f;
    public Entities.EnemyBehavior Behavior;

    // Attack pattern config
    public EnemyAttackType AttackType = EnemyAttackType.Bump;
    public float AttackCooldown;        // seconds between attacks
    public float AttackRange;           // melee reach or ranged preferred range
    public float AttackAnimDuration;    // how long the attack animation plays
    public float AttackDamageMultiplier = 1f; // multiplier on BaseDamage for attack hits
    public float ProjectileSpeed;       // ranged only: projectile velocity
    public int ProjectileCount = 1;     // ranged only: projectiles per shot (e.g., 3 for spread)
    public float ProjectileSpread;      // ranged only: spread angle in radians

    // Enrage (speed/damage boost at low HP)
    public bool EnragesAtLowHP;         // triggers enrage at 50% HP
    public float EnrageSpeedMult = 1.5f;
    public float EnrageDamageMult = 1.3f;

    // Frontal damage reduction (shield-bearers)
    public float FrontalDamageReduction; // 0 = none, 0.5 = 50% reduction from front

    // AOE pulse attack (Circle Bot style)
    public bool IsAOEPulse;             // melee attack hits in all directions (pulse)

    // Mine-laying (Planter Bot)
    public bool LaysMines;              // drops mines while moving
    public float MineLayInterval = 3f;  // seconds between mine drops
    public float MineDamageMultiplier = 1f;
    public float MineExplosionRadius = 30f;
    public float MineLifetime = 15f;

    // Rush/lunge (anti-kite melee)
    public bool CanRush;                // enables rush behavior
    public float RushCooldown = 3f;     // seconds between rushes
    public float RushDuration = 0.3f;   // how long the rush lasts
    public float RushSpeedMult = 3f;    // speed multiplier during rush

    // Kamikaze params
    public float FuseDuration;          // seconds until self-destruct
    public float ExplosionRadius;       // blast radius
    public float ExplosionDamageMultiplier = 1f; // multiplier on BaseDamage for explosion
}

public static class EnemyDatabase
{
    public static readonly EnemyDef[] Enemies =
    {
        new()
        {
            Name = "Scorpion", SpriteIndex = 0,
            BaseHP = 25, BaseSpeed = 45f, BaseDamage = 8,
            XPValue = 2, GoldValue = 1, Radius = 12f,
            Behavior = Entities.EnemyBehavior.Chase,
            AttackType = EnemyAttackType.Ranged,
            AttackCooldown = 1.8f, AttackRange = 110f,
            AttackAnimDuration = 0.35f, AttackDamageMultiplier = 0.7f,
            ProjectileSpeed = 170f,
        },
        new()
        {
            Name = "Snake", SpriteIndex = 4,
            BaseHP = 15, BaseSpeed = 70f, BaseDamage = 5,
            XPValue = 2, GoldValue = 1, Radius = 8f,
            Behavior = Entities.EnemyBehavior.FastChase,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 0.8f, AttackRange = 22f,
            AttackAnimDuration = 0.3f, AttackDamageMultiplier = 1.2f,
        },
        new()
        {
            Name = "Bat", SpriteIndex = 8,
            BaseHP = 18, BaseSpeed = 55f, BaseDamage = 6,
            XPValue = 3, GoldValue = 2, Radius = 11f,
            Behavior = Entities.EnemyBehavior.Erratic,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 1.2f, AttackRange = 25f,
            AttackAnimDuration = 0.4f, AttackDamageMultiplier = 1f,
        },
        new()
        {
            Name = "Beetle", SpriteIndex = 12,
            BaseHP = 60, BaseSpeed = 30f, BaseDamage = 15,
            XPValue = 5, GoldValue = 3, Radius = 14f,
            Behavior = Entities.EnemyBehavior.Tank,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 1.6f, AttackRange = 30f,
            AttackAnimDuration = 0.55f, AttackDamageMultiplier = 1.4f,
        },
        // Starter Pack enemies (index 4-6)
        new()
        {
            Name = "Archer", SpriteIndex = 0,
            BaseHP = 20, BaseSpeed = 40f, BaseDamage = 10,
            XPValue = 3, GoldValue = 2, Radius = 10f,
            Behavior = Entities.EnemyBehavior.Erratic,
            AttackType = EnemyAttackType.Ranged,
            AttackCooldown = 1.2f, AttackRange = 130f,
            AttackAnimDuration = 0.35f, AttackDamageMultiplier = 0.9f,
            ProjectileSpeed = 210f,
        },
        new()
        {
            Name = "Guard", SpriteIndex = 0,
            BaseHP = 45, BaseSpeed = 35f, BaseDamage = 12,
            XPValue = 4, GoldValue = 2, Radius = 12f,
            Behavior = Entities.EnemyBehavior.Tank,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 1.4f, AttackRange = 28f,
            AttackAnimDuration = 0.4f, AttackDamageMultiplier = 1.3f,
            FrontalDamageReduction = 0.5f,
        },
        new()
        {
            Name = "Warrior", SpriteIndex = 0,
            BaseHP = 55, BaseSpeed = 38f, BaseDamage = 14,
            XPValue = 4, GoldValue = 3, Radius = 11f,
            Behavior = Entities.EnemyBehavior.Chase,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 1.0f, AttackRange = 26f,
            AttackAnimDuration = 0.35f, AttackDamageMultiplier = 1.3f,
            CanRush = true, RushCooldown = 3.5f, RushDuration = 0.25f, RushSpeedMult = 3.5f,
        },
        // Insects (index 7-8)
        new()
        {
            Name = "Big Bug", SpriteIndex = 0,
            BaseHP = 80, BaseSpeed = 25f, BaseDamage = 18,
            XPValue = 6, GoldValue = 4, Radius = 16f,
            Behavior = Entities.EnemyBehavior.Tank,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 2.0f, AttackRange = 35f,
            AttackAnimDuration = 0.5f, AttackDamageMultiplier = 1.5f,
        },
        new()
        {
            Name = "Spiny Beetle", SpriteIndex = 0,
            BaseHP = 30, BaseSpeed = 60f, BaseDamage = 10,
            XPValue = 3, GoldValue = 2, Radius = 14f,
            Behavior = Entities.EnemyBehavior.Erratic,
            AttackType = EnemyAttackType.Ranged,
            AttackCooldown = 1.5f, AttackRange = 120f,
            AttackAnimDuration = 0.4f, AttackDamageMultiplier = 0.8f,
            ProjectileSpeed = 180f, ProjectileCount = 3, ProjectileSpread = 0.35f,
        },
        // Beast (index 9) — Relic Guardian
        new()
        {
            Name = "Relic Guardian", SpriteIndex = 0,
            BaseHP = 70, BaseSpeed = 32f, BaseDamage = 16,
            XPValue = 7, GoldValue = 4, Radius = 16f,
            Behavior = Entities.EnemyBehavior.Tank,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 2.2f, AttackRange = 38f,
            AttackAnimDuration = 0.6f, AttackDamageMultiplier = 1.6f,
            EnragesAtLowHP = true,
        },
        // Robots (index 10-13)
        new()
        {
            Name = "Rusty Robot", SpriteIndex = 0,
            BaseHP = 20, BaseSpeed = 65f, BaseDamage = 7,
            XPValue = 2, GoldValue = 1, Radius = 8f,
            Behavior = Entities.EnemyBehavior.FastChase,
            AttackType = EnemyAttackType.Kamikaze,
            FuseDuration = 5f, ExplosionRadius = 40f, ExplosionDamageMultiplier = 3f,
        },
        new()
        {
            Name = "Guard Robot", SpriteIndex = 0,
            BaseHP = 50, BaseSpeed = 30f, BaseDamage = 14,
            XPValue = 5, GoldValue = 3, Radius = 12f,
            Behavior = Entities.EnemyBehavior.Tank,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 1.5f, AttackRange = 30f,
            AttackAnimDuration = 0.45f, AttackDamageMultiplier = 1.4f,
            FrontalDamageReduction = 0.4f,
        },
        new()
        {
            Name = "Circle Bot", SpriteIndex = 0,
            BaseHP = 35, BaseSpeed = 45f, BaseDamage = 10,
            XPValue = 3, GoldValue = 2, Radius = 12f,
            Behavior = Entities.EnemyBehavior.Erratic,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 2.5f, AttackRange = 45f,
            AttackAnimDuration = 0.5f, AttackDamageMultiplier = 1.0f,
            IsAOEPulse = true,
        },
        new()
        {
            Name = "Delivery Bot", SpriteIndex = 0,
            BaseHP = 25, BaseSpeed = 85f, BaseDamage = 0,
            XPValue = 8, GoldValue = 12, Radius = 7f,
            Behavior = Entities.EnemyBehavior.Flee,
        },
        // Minions (index 14-16)
        new()
        {
            Name = "Hooded Minion", SpriteIndex = 0,
            BaseHP = 30, BaseSpeed = 42f, BaseDamage = 11,
            XPValue = 3, GoldValue = 2, Radius = 10f,
            Behavior = Entities.EnemyBehavior.Chase,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 0.9f, AttackRange = 24f,
            AttackAnimDuration = 0.3f, AttackDamageMultiplier = 1.3f,
        },
        new()
        {
            Name = "Bomb Minion", SpriteIndex = 0,
            BaseHP = 10, BaseSpeed = 75f, BaseDamage = 3,
            XPValue = 2, GoldValue = 1, Radius = 5f,
            Behavior = Entities.EnemyBehavior.FastChase,
            AttackType = EnemyAttackType.Kamikaze,
            FuseDuration = 4f, ExplosionRadius = 55f, ExplosionDamageMultiplier = 4f,
        },
        new()
        {
            Name = "Ranged Minion", SpriteIndex = 0,
            BaseHP = 22, BaseSpeed = 38f, BaseDamage = 8,
            XPValue = 3, GoldValue = 2, Radius = 8f,
            Behavior = Entities.EnemyBehavior.Erratic,
            AttackType = EnemyAttackType.Ranged,
            AttackCooldown = 1.3f, AttackRange = 115f,
            AttackAnimDuration = 0.4f, AttackDamageMultiplier = 0.7f,
            ProjectileSpeed = 190f, ProjectileCount = 3, ProjectileSpread = 0.25f,
        },
        // Planter Bot (index 17)
        new()
        {
            Name = "Planter Bot", SpriteIndex = 0,
            BaseHP = 28, BaseSpeed = 35f, BaseDamage = 6,
            XPValue = 3, GoldValue = 2, Radius = 11f,
            Behavior = Entities.EnemyBehavior.Chase,
            AttackType = EnemyAttackType.Melee,
            AttackCooldown = 2.0f, AttackRange = 32f,
            AttackAnimDuration = 0.5f, AttackDamageMultiplier = 1.2f,
            LaysMines = true, MineLayInterval = 2.5f, MineDamageMultiplier = 1.5f,
            MineExplosionRadius = 35f, MineLifetime = 12f,
        },
    };
}
