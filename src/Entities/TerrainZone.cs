using System.Numerics;

namespace CloneTato.Entities;

public enum TerrainType
{
    Sand,       // slows movement
    Oasis,      // heals over time
    Ooze,       // damages anything standing in it (temporary)
    Decorative, // visual only (grass, metallic)
}

public class TerrainZone
{
    public Vector2 Position;
    public float Radius;
    public TerrainType Type;
    public bool Active;

    // Temporary zones (ooze)
    public float Duration; // <=0 means permanent
    public float DamagePerSecond; // for ooze

    // Decorative tile set index (0=green grass, 1=purple grass, 2=metallic)
    public int DecoTileSet;

    public float SpeedMultiplier => Type switch
    {
        TerrainType.Sand => 0.6f,
        TerrainType.Ooze => 0.7f,
        _ => 1f,
    };

    public float HealPerSecond => Type switch
    {
        TerrainType.Oasis => 3f,
        _ => 0f,
    };
}
