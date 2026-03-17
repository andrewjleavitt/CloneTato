using System.Numerics;

namespace CloneTato.Entities;

public class HealthPickup : Entity
{
    public int HealAmount;

    public void Init(Vector2 pos, int heal)
    {
        Position = pos;
        HealAmount = heal;
        Active = true;
        Radius = 6f;
    }
}
