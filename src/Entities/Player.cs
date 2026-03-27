using System.Numerics;
using CloneTato.Data;

namespace CloneTato.Entities;

public class Player : Entity
{
    public Stats BaseStats;
    public Stats ComputedStats;
    public int CurrentHP;
    public float InvincibilityTimer;
    public int CharacterIndex;
    public HeroType HeroType;
    public bool FacingLeft;
    public float AnimTimer;

    // Dash
    public bool IsDashing;
    public float DashTimer;
    public float DashCooldownTimer;
    public Vector2 DashDirection;

    // Post-dash buff
    public float DashBuffTimer; // time remaining on post-dash buff
    public const float DashBuffDuration = 1.2f; // seconds of buff after dash

    // Hit knockback
    public float KnockbackTimer;
    public Vector2 KnockbackVelocity;

    // Melee attack animation
    public float MeleeAnimTimer; // > 0 means currently showing attack anim
    public int MeleeAttackCount;  // alternates slash/chop for BladeDancer

    // BladeDancer: aim comes from movement direction, not right stick
    public Vector2 LastMoveDirection = new(1, 0); // last non-zero move input

    // Footstep sound timing
    public float FootstepTimer;

    public Player()
    {
        Radius = 10f;
        Active = true;
    }

    public void Init(CharacterDef character)
    {
        CharacterIndex = character.SpriteIndex;
        HeroType = character.HeroType;
        BaseStats = character.BaseStats;
        ComputedStats = BaseStats;
        CurrentHP = ComputedStats.MaxHP;
        Position = new Vector2(Constants.ArenaWidth / 2f, Constants.ArenaHeight / 2f);
        SpriteIndex = character.SpriteIndex;
    }

    public void RecomputeStats(Stats itemBonus, Stats levelBonus)
    {
        ComputedStats = BaseStats + itemBonus + levelBonus;
    }

    public int GetDisplaySprite()
    {
        // Each character has 4 sprites in a row: 3 walk frames + 1 death frame
        int baseSprite = CharacterIndex;
        if (CurrentHP <= 0) return baseSprite + 3; // death frame
        int frame = (int)(AnimTimer * 6f) % 3; // cycle through frames 0, 1, 2
        return baseSprite + frame;
    }
}
