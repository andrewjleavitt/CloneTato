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
        // Starter Pack enemies (index 4-6)
        new()
        {
            Name = "Archer", SpriteIndex = 0,
            BaseHP = 20, BaseSpeed = 40f, BaseDamage = 10,
            XPValue = 3, GoldValue = 2, Radius = 10f,
            Behavior = Entities.EnemyBehavior.Erratic, // kites at range
        },
        new()
        {
            Name = "Guard", SpriteIndex = 0,
            BaseHP = 45, BaseSpeed = 35f, BaseDamage = 12,
            XPValue = 4, GoldValue = 2, Radius = 12f,
            Behavior = Entities.EnemyBehavior.Chase,
        },
        new()
        {
            Name = "Warrior", SpriteIndex = 0,
            BaseHP = 55, BaseSpeed = 38f, BaseDamage = 14,
            XPValue = 4, GoldValue = 3, Radius = 11f,
            Behavior = Entities.EnemyBehavior.Chase,
        },
        // Insects (index 7-8)
        new()
        {
            Name = "Big Bug", SpriteIndex = 0,
            BaseHP = 80, BaseSpeed = 25f, BaseDamage = 18,
            XPValue = 6, GoldValue = 4, Radius = 14f,
            Behavior = Entities.EnemyBehavior.Tank,
        },
        new()
        {
            Name = "Spiny Beetle", SpriteIndex = 0,
            BaseHP = 30, BaseSpeed = 60f, BaseDamage = 10,
            XPValue = 3, GoldValue = 2, Radius = 11f,
            Behavior = Entities.EnemyBehavior.Erratic,
        },
        // Beast (index 9) — Relic Guardian
        new()
        {
            Name = "Relic Guardian", SpriteIndex = 0,
            BaseHP = 70, BaseSpeed = 32f, BaseDamage = 16,
            XPValue = 7, GoldValue = 4, Radius = 14f,
            Behavior = Entities.EnemyBehavior.Tank,
        },
        // Robots (index 10-13)
        new()
        {
            Name = "Rusty Robot", SpriteIndex = 0,
            BaseHP = 20, BaseSpeed = 65f, BaseDamage = 7,
            XPValue = 2, GoldValue = 1, Radius = 8f,
            Behavior = Entities.EnemyBehavior.FastChase,
        },
        new()
        {
            Name = "Guard Robot", SpriteIndex = 0,
            BaseHP = 50, BaseSpeed = 30f, BaseDamage = 14,
            XPValue = 5, GoldValue = 3, Radius = 11f,
            Behavior = Entities.EnemyBehavior.Tank,
        },
        new()
        {
            Name = "Circle Bot", SpriteIndex = 0,
            BaseHP = 35, BaseSpeed = 45f, BaseDamage = 10,
            XPValue = 3, GoldValue = 2, Radius = 10f,
            Behavior = Entities.EnemyBehavior.Erratic,
        },
        new()
        {
            Name = "Delivery Bot", SpriteIndex = 0,
            BaseHP = 12, BaseSpeed = 80f, BaseDamage = 4,
            XPValue = 1, GoldValue = 1, Radius = 7f,
            Behavior = Entities.EnemyBehavior.FastChase,
        },
        // Minions (index 14-16)
        new()
        {
            Name = "Hooded Minion", SpriteIndex = 0,
            BaseHP = 30, BaseSpeed = 42f, BaseDamage = 11,
            XPValue = 3, GoldValue = 2, Radius = 10f,
            Behavior = Entities.EnemyBehavior.Chase,
        },
        new()
        {
            Name = "Bomb Minion", SpriteIndex = 0,
            BaseHP = 10, BaseSpeed = 75f, BaseDamage = 3,
            XPValue = 2, GoldValue = 1, Radius = 5f,
            Behavior = Entities.EnemyBehavior.FastChase,
        },
        new()
        {
            Name = "Ranged Minion", SpriteIndex = 0,
            BaseHP = 22, BaseSpeed = 38f, BaseDamage = 8,
            XPValue = 3, GoldValue = 2, Radius = 8f,
            Behavior = Entities.EnemyBehavior.Erratic,
        },
    };
}
