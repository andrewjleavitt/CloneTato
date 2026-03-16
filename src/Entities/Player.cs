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
    public bool FacingLeft;
    public float AnimTimer;

    public Player()
    {
        Radius = 10f;
        Active = true;
    }

    public void Init(CharacterDef character)
    {
        CharacterIndex = character.SpriteIndex;
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
        // Row 0: idle frames (0,1), Row 1: walk frames (2,3) based on the 4x4 player grid
        // Each character has 4 sprites: 2 idle variants + 2 walk variants
        int baseSprite = CharacterIndex;
        bool moving = Velocity.LengthSquared() > 1f;
        int frame = (int)(AnimTimer * 4f) % 2;
        return moving ? baseSprite + 2 + frame : baseSprite + frame;
    }
}
