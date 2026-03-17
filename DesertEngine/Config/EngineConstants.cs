namespace DesertEngine.Config;

public static class EngineConstants
{
    public const int LogicalWidth = 480;
    public const int LogicalHeight = 270;
    public const int WindowScale = 3;
    public const int WindowWidth = LogicalWidth * WindowScale;
    public const int WindowHeight = LogicalHeight * WindowScale;

    public const int TileSize = 16;
    public const int SpriteSize = 24;

    // Kenney Desert Shooter Pack tilemap grid dimensions
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
