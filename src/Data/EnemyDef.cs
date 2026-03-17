namespace CloneTato.Data;

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

}

public static class EnemyDatabase
{
    public static readonly EnemyDef[] Enemies =
    {
        new()
        {
            Name = "Scorpion", SpriteIndex = 0,
            BaseHP = 25, BaseSpeed = 45f, BaseDamage = 8,
            XPValue = 2, GoldValue = 1, Radius = 10f,
            Behavior = Entities.EnemyBehavior.Chase,
        },
        new()
        {
            Name = "Snake", SpriteIndex = 4,
            BaseHP = 15, BaseSpeed = 70f, BaseDamage = 5,
            XPValue = 2, GoldValue = 1, Radius = 8f,
            Behavior = Entities.EnemyBehavior.FastChase,
        },
        new()
        {
            Name = "Bat", SpriteIndex = 8,
            BaseHP = 18, BaseSpeed = 55f, BaseDamage = 6,
            XPValue = 3, GoldValue = 2, Radius = 9f,
            Behavior = Entities.EnemyBehavior.Erratic,
        },
        new()
        {
            Name = "Beetle", SpriteIndex = 12,
            BaseHP = 60, BaseSpeed = 30f, BaseDamage = 15,
            XPValue = 5, GoldValue = 3, Radius = 12f,
            Behavior = Entities.EnemyBehavior.Tank,
        },
    };
}
