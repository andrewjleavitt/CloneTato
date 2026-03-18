namespace CloneTato;

public static class Constants
{
    public const int LogicalWidth = 640;
    public const int LogicalHeight = 360;
    public const int WindowScale = 3;
    public const int WindowWidth = LogicalWidth * WindowScale;
    public const int WindowHeight = LogicalHeight * WindowScale;

    public const int ArenaWidth = 1600;
    public const int ArenaHeight = 1200;

    public const int TileSize = 16;
    public const int SpriteSize = 24;
    public const int TileSpacing = 1;

    public const int MaxWeaponSlots = 6;
    public const int MaxWaves = 20;
    public const float WaveBaseDuration = 25.0f;

    public const float PlayerInvincibilityTime = 0.5f;
    public const float XPAttractRadius = 50f;
    public const float XPCollectRadius = 10f;
    public const float DashSpeed = 350f;
    public const float DashDuration = 0.15f;
    public const float DashCooldown = 0.6f;

    public const float KnockbackDuration = 0.15f;
    public const float KnockbackForce = 200f;
    public const float DamageNumberDuration = 0.8f;

    // Tilemap info
    public const int PlayersCols = 4;
    public const int PlayersRows = 4;
    public const int EnemiesCols = 4;
    public const int EnemiesRows = 4;
    public const int WeaponsCols = 10;
    public const int WeaponsRows = 4;
    public const int TilesCols = 18;
    public const int TilesRows = 13;
    public const int InterfaceCols = 18;
    public const int InterfaceRows = 11;

    public const string AssetBasePath = "kenney_desert-shooter-pack_1";
}
