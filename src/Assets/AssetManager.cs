using Raylib_cs;

namespace CloneTato.Assets;

public class AssetManager
{
    public SpriteAtlas Players { get; private set; } = null!;
    public SpriteAtlas Enemies { get; private set; } = null!;
    public SpriteAtlas Weapons { get; private set; } = null!;
    public SpriteAtlas Tiles { get; private set; } = null!;
    public SpriteAtlas Interface { get; private set; } = null!;

    // STRANDED animated sprites — hero variants
    public AnimatedSprite? HeroSprite { get; set; } // active hero (set by character selection)
    public AnimatedSprite? HeroGunSprite { get; private set; }
    public AnimatedSprite? HeroSwordSprite { get; private set; }
    public AnimatedSprite? StarterHeroSprite { get; private set; }
    public AnimatedSprite? CompanionSprite { get; private set; }

    // Enemy animated sprites keyed by EnemyDef index (0=Scorpion→TribeHunter, etc.)
    public AnimatedSprite?[] EnemySprites { get; private set; } = Array.Empty<AnimatedSprite?>();

    // Boss animated sprites
    public AnimatedSprite? BossSprite { get; private set; }
    public AnimatedSprite? BlowfishSprite { get; private set; }
    public AnimatedSprite? TarnishedWidowSprite { get; private set; }

    // STRANDED terrain props (obstacle textures and ground scatter)
    public Texture2D[] ObstacleTextures { get; private set; } = Array.Empty<Texture2D>();
    public Texture2D[] GroundScatterTextures { get; private set; } = Array.Empty<Texture2D>();
    public Texture2D[] LargeScatterTextures { get; private set; } = Array.Empty<Texture2D>(); // larger accent props
    public bool HasStrandedTerrain => ObstacleTextures.Length > 0;

    // STRANDED terrain tileset (Blood Desert floor tiles)
    public Texture2D BloodDesertTileset { get; private set; }
    public bool HasBloodDesertTileset => BloodDesertTileset.Id != 0;

    // STRANDED UI icons
    public Texture2D HealthPickupIcon { get; private set; }
    public Texture2D CoinIcon { get; private set; }
    public Texture2D HPBarSheet { get; private set; }
    public bool HasStrandedUI { get; private set; }

    // Shaders
    public Shader OutlineShader { get; private set; }

    private readonly Dictionary<string, Sound> _sounds = new();

    private static Texture2D LoadTexturePoint(string path)
    {
        var tex = Raylib.LoadTexture(path);
        Raylib.SetTextureFilter(tex, TextureFilter.Point);
        return tex;
    }

    public void LoadAll()
    {
        string basePath = Constants.AssetBasePath + "/PNG";

        var playersTex = LoadTexturePoint(basePath + "/Players/Tilemap/tilemap_packed.png");
        Players = new SpriteAtlas(playersTex, Constants.SpriteSize, Constants.SpriteSize,
            0, Constants.PlayersCols, Constants.PlayersRows);

        var enemiesTex = LoadTexturePoint(basePath + "/Enemies/Tilemap/tilemap_packed.png");
        Enemies = new SpriteAtlas(enemiesTex, Constants.SpriteSize, Constants.SpriteSize,
            0, Constants.EnemiesCols, Constants.EnemiesRows);

        var weaponsTex = LoadTexturePoint(basePath + "/Weapons/Tilemap/tilemap_packed.png");
        Weapons = new SpriteAtlas(weaponsTex, Constants.SpriteSize, Constants.SpriteSize,
            0, Constants.WeaponsCols, Constants.WeaponsRows);

        var tilesTex = LoadTexturePoint(basePath + "/Tiles/Tilemap/tilemap_packed.png");
        Tiles = new SpriteAtlas(tilesTex, Constants.TileSize, Constants.TileSize,
            0, Constants.TilesCols, Constants.TilesRows);

        var interfaceTex = LoadTexturePoint(basePath + "/Interface/Tilemap/tilemap_packed.png");
        Interface = new SpriteAtlas(interfaceTex, Constants.TileSize, Constants.TileSize,
            0, Constants.InterfaceCols, Constants.InterfaceRows);

        LoadSounds();
        LoadStrandedAssets();
        LoadShaders();
    }

    private void LoadStrandedAssets()
    {
        string strandedPath = "assets/stranded";
        if (!Directory.Exists(strandedPath)) return;

        if (Directory.Exists(strandedPath + "/hero"))
        {
            HeroGunSprite = AnimationLoader.LoadHeroGun(strandedPath);
            HeroSwordSprite = AnimationLoader.LoadHeroSword(strandedPath);
            StarterHeroSprite = AnimationLoader.LoadStarterHero(strandedPath);
            CompanionSprite = AnimationLoader.LoadCompanion(strandedPath);
            HeroSprite = HeroGunSprite; // default
        }

        if (Directory.Exists(strandedPath + "/enemies"))
            EnemySprites = AnimationLoader.LoadEnemySprites(strandedPath);

        BossSprite = AnimationLoader.LoadBossSprite(strandedPath);
        BlowfishSprite = AnimationLoader.LoadBlowfish(strandedPath);
        TarnishedWidowSprite = AnimationLoader.LoadTarnishedWidow(strandedPath);

        LoadStrandedTerrain(strandedPath);
        LoadStrandedUI(strandedPath);

        // Blood Desert ground tileset
        string bdTilesetPath = strandedPath + "/terrain/blood_desert/Blood Desert Tileset.png";
        if (File.Exists(bdTilesetPath))
            BloodDesertTileset = LoadTexturePoint(bdTilesetPath);
    }

    private void LoadStrandedUI(string strandedPath)
    {
        string iconsDir = strandedPath + "/ui/pack/16x16 Icons";
        if (!Directory.Exists(iconsDir)) return;

        // Icon1 = heart (health pickup), Icon6 = coin
        string heartPath = $"{iconsDir}/16x16 Icons1.png";
        string coinPath = $"{iconsDir}/16x16 Icons6.png";

        if (File.Exists(heartPath))
            HealthPickupIcon = LoadTexturePoint(heartPath);
        if (File.Exists(coinPath))
            CoinIcon = LoadTexturePoint(coinPath);

        // HP bar spritesheet
        string hpBarPath = strandedPath + "/ui/pack/HP Bars/HP Bar 51x9.png";
        if (File.Exists(hpBarPath))
            HPBarSheet = LoadTexturePoint(hpBarPath);

        HasStrandedUI = HealthPickupIcon.Id != 0;
    }

    private void LoadStrandedTerrain(string strandedPath)
    {
        string propsDir = strandedPath + "/terrain/blood_desert/Separate Sprites";
        if (!Directory.Exists(propsDir)) return;

        // Large obstacle props (collidable) — trees, rocks, skulls, statue
        string[] obstacleFiles =
        {
            "Tree1.png", "Tree2.png", "Tree3.png", "Tree4.png", "Tree5.png",
            "Big Rock.png", "Skull.png", "Skull Grassy.png", "statue.png",
        };
        var obsList = new List<Texture2D>();
        foreach (var f in obstacleFiles)
        {
            string path = $"{propsDir}/{f}";
            if (File.Exists(path))
                obsList.Add(LoadTexturePoint(path));
        }
        ObstacleTextures = obsList.ToArray();

        // Small ground scatter from ground_scatter/ directory (grass, pebbles, gravel — subtle)
        string scatterDir = strandedPath + "/terrain/blood_desert/ground_scatter";
        if (Directory.Exists(scatterDir))
        {
            var scatterFiles = Directory.GetFiles(scatterDir, "*.png");
            Array.Sort(scatterFiles);
            var scatList = new List<Texture2D>();
            foreach (var f in scatterFiles)
                scatList.Add(LoadTexturePoint(f));
            GroundScatterTextures = scatList.ToArray();
        }

        // Larger accent props from Separate Sprites (swords, hands, poles — occasional)
        string[] largeScatterFiles =
        {
            "small rocks.png", "sword 1.png", "swoord 2.png", "sword 3.png",
            "forest pole 1.png", "forest pole 2.png", "forest pole 3.png",
            "Hand.png", "Hand grassy.png",
        };
        var lgList = new List<Texture2D>();
        foreach (var f in largeScatterFiles)
        {
            string path = $"{propsDir}/{f}";
            if (File.Exists(path))
                lgList.Add(LoadTexturePoint(path));
        }
        LargeScatterTextures = lgList.ToArray();
    }

    private void LoadShaders()
    {
        string outlinePath = "assets/shaders/outline.fs";
        if (File.Exists(outlinePath))
            OutlineShader = Raylib.LoadShader(null, outlinePath);
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
        HeroGunSprite?.UnloadAll();
        HeroSwordSprite?.UnloadAll();
        StarterHeroSprite?.UnloadAll();
        CompanionSprite?.UnloadAll();
        BossSprite?.UnloadAll();
        BlowfishSprite?.UnloadAll();
        TarnishedWidowSprite?.UnloadAll();
        foreach (var es in EnemySprites)
            es?.UnloadAll();
        foreach (var tex in ObstacleTextures)
            Raylib.UnloadTexture(tex);
        foreach (var tex in GroundScatterTextures)
            Raylib.UnloadTexture(tex);
        foreach (var tex in LargeScatterTextures)
            Raylib.UnloadTexture(tex);
        if (BloodDesertTileset.Id != 0) Raylib.UnloadTexture(BloodDesertTileset);
        if (HealthPickupIcon.Id != 0) Raylib.UnloadTexture(HealthPickupIcon);
        if (CoinIcon.Id != 0) Raylib.UnloadTexture(CoinIcon);
        if (HPBarSheet.Id != 0) Raylib.UnloadTexture(HPBarSheet);
        if (OutlineShader.Id != 0) Raylib.UnloadShader(OutlineShader);
        foreach (var sound in _sounds.Values)
            Raylib.UnloadSound(sound);
    }
}
