using Raylib_cs;

namespace CloneTato.Assets;

public class AssetManager
{
    public SpriteAtlas Players { get; private set; } = null!;
    public SpriteAtlas Enemies { get; private set; } = null!;
    public SpriteAtlas Weapons { get; private set; } = null!;
    public SpriteAtlas Tiles { get; private set; } = null!;
    public SpriteAtlas Interface { get; private set; } = null!;

    private readonly Dictionary<string, Sound> _sounds = new();

    public void LoadAll()
    {
        string basePath = Constants.AssetBasePath + "/PNG";

        var playersTex = Raylib.LoadTexture(basePath + "/Players/Tilemap/tilemap_packed.png");
        Players = new SpriteAtlas(playersTex, Constants.SpriteSize, Constants.SpriteSize,
            0, Constants.PlayersCols, Constants.PlayersRows);

        var enemiesTex = Raylib.LoadTexture(basePath + "/Enemies/Tilemap/tilemap_packed.png");
        Enemies = new SpriteAtlas(enemiesTex, Constants.SpriteSize, Constants.SpriteSize,
            0, Constants.EnemiesCols, Constants.EnemiesRows);

        var weaponsTex = Raylib.LoadTexture(basePath + "/Weapons/Tilemap/tilemap_packed.png");
        Weapons = new SpriteAtlas(weaponsTex, Constants.SpriteSize, Constants.SpriteSize,
            0, Constants.WeaponsCols, Constants.WeaponsRows);

        var tilesTex = Raylib.LoadTexture(basePath + "/Tiles/Tilemap/tilemap_packed.png");
        Tiles = new SpriteAtlas(tilesTex, Constants.TileSize, Constants.TileSize,
            0, Constants.TilesCols, Constants.TilesRows);

        var interfaceTex = Raylib.LoadTexture(basePath + "/Interface/Tilemap/tilemap_packed.png");
        Interface = new SpriteAtlas(interfaceTex, Constants.TileSize, Constants.TileSize,
            0, Constants.InterfaceCols, Constants.InterfaceRows);

        LoadSounds();
    }

    private void LoadSounds()
    {
        string soundPath = Constants.AssetBasePath + "/Sounds";
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
        Raylib.SetSoundVolume(sound, volumeScale * Core.GameSettings.Current.SFXVolume);
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
