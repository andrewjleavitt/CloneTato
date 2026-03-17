using DesertEngine.Config;
using Raylib_cs;

namespace DesertEngine.Assets;

/// <summary>
/// Loads and manages all assets from the Kenney Desert Shooter pack.
/// Both game projects share the same atlas layout.
/// </summary>
public class AssetManager
{
    public SpriteAtlas Players { get; private set; } = null!;
    public SpriteAtlas Enemies { get; private set; } = null!;
    public SpriteAtlas Weapons { get; private set; } = null!;
    public SpriteAtlas Tiles { get; private set; } = null!;
    public SpriteAtlas Interface { get; private set; } = null!;

    private readonly Dictionary<string, Sound> _sounds = new();

    public void LoadAll(string basePath = EngineConstants.AssetBasePath)
    {
        string pngPath = basePath + "/PNG";

        var playersTex = Raylib.LoadTexture(pngPath + "/Players/Tilemap/tilemap_packed.png");
        Players = new SpriteAtlas(playersTex, EngineConstants.SpriteSize, EngineConstants.SpriteSize,
            0, EngineConstants.PlayersCols, EngineConstants.PlayersRows);

        var enemiesTex = Raylib.LoadTexture(pngPath + "/Enemies/Tilemap/tilemap_packed.png");
        Enemies = new SpriteAtlas(enemiesTex, EngineConstants.SpriteSize, EngineConstants.SpriteSize,
            0, EngineConstants.EnemiesCols, EngineConstants.EnemiesRows);

        var weaponsTex = Raylib.LoadTexture(pngPath + "/Weapons/Tilemap/tilemap_packed.png");
        Weapons = new SpriteAtlas(weaponsTex, EngineConstants.SpriteSize, EngineConstants.SpriteSize,
            0, EngineConstants.WeaponsCols, EngineConstants.WeaponsRows);

        var tilesTex = Raylib.LoadTexture(pngPath + "/Tiles/Tilemap/tilemap_packed.png");
        Tiles = new SpriteAtlas(tilesTex, EngineConstants.TileSize, EngineConstants.TileSize,
            0, EngineConstants.TilesCols, EngineConstants.TilesRows);

        var interfaceTex = Raylib.LoadTexture(pngPath + "/Interface/Tilemap/tilemap_packed.png");
        Interface = new SpriteAtlas(interfaceTex, EngineConstants.TileSize, EngineConstants.TileSize,
            0, EngineConstants.InterfaceCols, EngineConstants.InterfaceRows);

        LoadSounds(basePath + "/Sounds");
    }

    private void LoadSounds(string soundPath)
    {
        if (!Directory.Exists(soundPath)) return;
        string[] soundFiles = Directory.GetFiles(soundPath, "*.ogg");
        foreach (var file in soundFiles)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            _sounds[name] = Raylib.LoadSound(file);
        }
    }

    public Sound GetSound(string name)
    {
        return _sounds.TryGetValue(name, out var sound) ? sound : default;
    }

    public void PlaySoundVariant(string baseName, float volumeScale = 1.0f)
    {
        var variants = _sounds.Keys.Where(k => k.StartsWith(baseName + "-")).ToList();
        if (variants.Count == 0) return;
        string pick = variants[Random.Shared.Next(variants.Count)];
        var sound = _sounds[pick];
        Raylib.SetSoundVolume(sound, volumeScale);
        Raylib.PlaySound(sound);
    }

    public void UnloadAll()
    {
        Raylib.UnloadTexture(Players.Texture);
        Raylib.UnloadTexture(Enemies.Texture);
        Raylib.UnloadTexture(Weapons.Texture);
        Raylib.UnloadTexture(Tiles.Texture);
        Raylib.UnloadTexture(Interface.Texture);
        foreach (var sound in _sounds.Values)
            Raylib.UnloadSound(sound);
    }
}
