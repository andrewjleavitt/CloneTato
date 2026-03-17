using System.Numerics;

namespace CloneTato.Entities;

public enum BarrelType
{
    Explosive, // red barrel — AOE explosion on death
    Toxic,     // green barrel — spawns damaging ooze pool on death
}

public class Barrel
{
    public Vector2 Position;
    public float Radius = 10f;
    public int CurrentHP;
    public int MaxHP;
    public BarrelType Type;
    public bool Active;
    public float FlashTimer;
}
