using System.Numerics;

namespace CloneTato.Entities;

public class DamageNumber : Entity
{
    public string Text = "";
    public float Lifetime;
    public float Alpha;
    public Raylib_cs.Color BaseColor;

    public void Init(Vector2 pos, string text, Raylib_cs.Color color)
    {
        Position = pos + new Vector2(
            Random.Shared.NextSingle() * 10f - 5f,
            Random.Shared.NextSingle() * 5f - 10f);
        Text = text;
        Lifetime = Constants.DamageNumberDuration;
        Alpha = 1f;
        BaseColor = color;
        Velocity = new Vector2(0, -30f);
        Active = true;
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        Lifetime -= dt;
        Alpha = Math.Clamp(Lifetime / Constants.DamageNumberDuration, 0f, 1f);
        if (Lifetime <= 0) Active = false;
    }
}
