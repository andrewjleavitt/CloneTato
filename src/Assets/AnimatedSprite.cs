using System.Numerics;
using Raylib_cs;

namespace CloneTato.Assets;

/// <summary>
/// A single animation loaded from a horizontal sprite strip.
/// The strip is one row of equal-width frames.
/// </summary>
public class SpriteAnimation
{
    public Texture2D Texture { get; }
    public int FrameWidth { get; }
    public int FrameHeight { get; }
    public int FrameCount { get; }
    public float FrameDuration { get; }
    public bool Loop { get; set; } = true;

    public SpriteAnimation(Texture2D texture, int frameWidth, int frameHeight, int frameCount, float fps)
    {
        Texture = texture;
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        FrameCount = frameCount;
        FrameDuration = 1f / fps;
    }

    public virtual Rectangle GetFrameRect(int frame)
    {
        int f = Math.Clamp(frame, 0, FrameCount - 1);
        return new Rectangle(f * FrameWidth, 0, FrameWidth, FrameHeight);
    }
}

/// <summary>
/// Loads and manages a set of named animations for an entity.
/// Handles playback state (current frame, timer, direction).
/// </summary>
public class AnimatedSprite
{
    private readonly Dictionary<string, SpriteAnimation> _animations = new();
    private SpriteAnimation? _current;
    private string _currentName = "";
    private int _frame;
    private float _timer;
    private bool _finished;

    public bool FlipH { get; set; }

    /// <summary>
    /// Horizontal offset of the character's visual center from the frame center.
    /// Positive = character is right of frame center. Applied before flip.
    /// When FlipH is true, this offset is mirrored automatically.
    /// </summary>
    public float PivotOffsetX { get; set; }
    public float PivotOffsetY { get; set; }

    public int FrameWidth => _current?.FrameWidth ?? 0;
    public int FrameHeight => _current?.FrameHeight ?? 0;
    public bool IsFinished => _finished;
    public string CurrentAnimation => _currentName;

    public void AddAnimation(string name, SpriteAnimation anim)
    {
        _animations[name] = anim;
    }

    public void Play(string name, bool restart = false)
    {
        if (_currentName == name && !restart) return;
        if (!_animations.TryGetValue(name, out var anim)) return;

        _current = anim;
        _currentName = name;
        _frame = 0;
        _timer = 0f;
        _finished = false;
    }

    public bool HasAnimation(string name) => _animations.ContainsKey(name);

    public void Update(float dt)
    {
        if (_current == null || _finished) return;

        _timer += dt;
        if (_timer >= _current.FrameDuration)
        {
            _timer -= _current.FrameDuration;
            _frame++;

            if (_frame >= _current.FrameCount)
            {
                if (_current.Loop)
                    _frame = 0;
                else
                {
                    _frame = _current.FrameCount - 1;
                    _finished = true;
                }
            }
        }
    }

    /// <summary>Draw centered at (x, y) with optional tint and scale.</summary>
    public void DrawCentered(float x, float y, Color tint, float scale = 1f)
    {
        if (_current == null) return;

        var src = _current.GetFrameRect(_frame);
        if (FlipH) src.Width = -src.Width;

        // Apply pivot offset so the character's visual center aligns with (x, y).
        // When flipped, mirror the X offset.
        float ox = FlipH ? -PivotOffsetX : PivotOffsetX;
        float oy = PivotOffsetY;

        float w = _current.FrameWidth * scale;
        float h = _current.FrameHeight * scale;
        var dest = new Rectangle(x + ox * scale, y + oy * scale, w, h);
        var origin = new Vector2(w / 2f, h / 2f);
        Raylib.DrawTexturePro(_current.Texture, src, dest, origin, 0f, tint);
    }

    /// <summary>Draw at top-left (x, y) with optional tint and scale.</summary>
    public void Draw(float x, float y, Color tint, float scale = 1f)
    {
        if (_current == null) return;

        var src = _current.GetFrameRect(_frame);
        if (FlipH) src.Width = -src.Width;

        float w = _current.FrameWidth * scale;
        float h = _current.FrameHeight * scale;
        var dest = new Rectangle(x, y, w, h);
        Raylib.DrawTexturePro(_current.Texture, src, dest, Vector2.Zero, 0f, tint);
    }

    /// <summary>
    /// Get the source rect for the current frame (for custom drawing with DrawTexturePro).
    /// </summary>
    public Rectangle GetCurrentSourceRect()
    {
        if (_current == null) return default;
        var src = _current.GetFrameRect(_frame);
        if (FlipH) src.Width = -src.Width;
        return src;
    }

    public Texture2D GetCurrentTexture()
    {
        return _current?.Texture ?? default;
    }

    /// <summary>
    /// Draw a specific animation at a specific frame, without affecting internal playback state.
    /// Used for pooled entities that share one AnimatedSprite definition.
    /// </summary>
    public void DrawAnimationFrame(string animName, int frame, bool flipH,
        float x, float y, Color tint, float scale = 1f)
    {
        if (!_animations.TryGetValue(animName, out var anim)) return;

        var src = anim.GetFrameRect(frame);
        if (flipH) src.Width = -src.Width;

        float ox = flipH ? -PivotOffsetX : PivotOffsetX;
        float oy = PivotOffsetY;

        float w = anim.FrameWidth * scale;
        float h = anim.FrameHeight * scale;
        var dest = new Rectangle(x + ox * scale, y + oy * scale, w, h);
        var origin = new Vector2(w / 2f, h / 2f);
        Raylib.DrawTexturePro(anim.Texture, src, dest, origin, 0f, tint);
    }

    /// <summary>Get frame count for a named animation.</summary>
    public int GetFrameCount(string animName)
    {
        return _animations.TryGetValue(animName, out var anim) ? anim.FrameCount : 0;
    }

    /// <summary>Get frame duration for a named animation.</summary>
    public float GetFrameDuration(string animName)
    {
        return _animations.TryGetValue(animName, out var anim) ? anim.FrameDuration : 0.1f;
    }

    /// <summary>Get frame width for a named animation.</summary>
    public int GetFrameWidth(string animName)
    {
        return _animations.TryGetValue(animName, out var anim) ? anim.FrameWidth : 0;
    }

    /// <summary>Get frame height for a named animation.</summary>
    public int GetFrameHeight(string animName)
    {
        return _animations.TryGetValue(animName, out var anim) ? anim.FrameHeight : 0;
    }

    public void UnloadAll()
    {
        var textures = new HashSet<uint>();
        foreach (var anim in _animations.Values)
        {
            if (textures.Add(anim.Texture.Id))
                Raylib.UnloadTexture(anim.Texture);
        }
        _animations.Clear();
    }
}

/// <summary>
/// Animation from a grid-based sprite sheet where frames are in rows/columns.
/// </summary>
public class GridSpriteAnimation : SpriteAnimation
{
    private readonly int _cols;
    private readonly int _startFrame;

    public GridSpriteAnimation(Texture2D texture, int cellW, int cellH, int cols,
        int startFrame, int frameCount, float fps)
        : base(texture, cellW, cellH, frameCount, fps)
    {
        _cols = cols;
        _startFrame = startFrame;
    }

    public override Rectangle GetFrameRect(int frame)
    {
        int f = Math.Clamp(frame, 0, FrameCount - 1) + _startFrame;
        int col = f % _cols;
        int row = f / _cols;
        return new Rectangle(col * FrameWidth, row * FrameHeight, FrameWidth, FrameHeight);
    }
}

/// <summary>
/// Helper to load animation strips from the STRANDED asset pack.
/// </summary>
public static class AnimationLoader
{
    /// <summary>
    /// Create an animation from a grid-based sprite sheet.
    /// Frames are laid out left-to-right, top-to-bottom in a grid.
    /// </summary>
    public static SpriteAnimation FromGrid(Texture2D texture, int cellW, int cellH, int cols,
        int startFrame, int frameCount, float fps, bool loop = true)
    {
        return new GridSpriteAnimation(texture, cellW, cellH, cols, startFrame, frameCount, fps) { Loop = loop };
    }

    /// <summary>
    /// Load a horizontal strip sprite sheet where frame width is known
    /// and frame count is derived from image width / frameWidth.
    /// </summary>
    public static SpriteAnimation LoadStrip(string path, int frameWidth, int frameHeight, float fps, bool loop = true)
    {
        var tex = Raylib.LoadTexture(path);
        int frameCount = tex.Width / frameWidth;
        return new SpriteAnimation(tex, frameWidth, frameHeight, frameCount, fps) { Loop = loop };
    }

    /// <summary>
    /// Load a horizontal strip and derive frame count from image width / frameWidth,
    /// using explicit frameHeight. Overload uses image height when frameHeight matches.
    /// </summary>
    public static SpriteAnimation LoadStripAutoCount(string path, int frameWidth, float fps, bool loop = true)
    {
        var tex = Raylib.LoadTexture(path);
        int frameCount = tex.Width / frameWidth;
        return new SpriteAnimation(tex, frameWidth, tex.Height, frameCount, fps) { Loop = loop };
    }

    /// <summary>
    /// Build an AnimatedSprite for the hero (gun variant) with all directional animations.
    /// Uses first frame of idle_right as static fallback.
    /// </summary>
    public static AnimatedSprite LoadHeroGun(string basePath)
    {
        var sprite = new AnimatedSprite();
        string gun = basePath + "/hero/Without Sword (for gun)";

        // Idle
        sprite.AddAnimation("idle_right", LoadStrip($"{gun}/left right/R.png", 64, 65, 8));
        sprite.AddAnimation("idle_up", LoadStrip($"{gun}/Up/04 Stranded - Pack 4 back up-Idle Up.png", 64, 65, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{gun}/Down/04 Stranded - Pack 4 back up-Idle Down.png", 64, 65, 8));

        // Run
        sprite.AddAnimation("run_right", LoadStrip($"{gun}/left right/R Run.png", 64, 65, 10));
        sprite.AddAnimation("run_up", LoadStrip($"{gun}/Up/04 Stranded - Pack 4 back up-Run UP.png", 64, 65, 10));
        sprite.AddAnimation("run_down", LoadStrip($"{gun}/Down/04 Stranded - Pack 4 back up-Run Down.png", 64, 65, 10));

        // Roll (dash)
        sprite.AddAnimation("roll_right", LoadStrip($"{gun}/left right/R Roll.png", 64, 65, 12, false));
        sprite.AddAnimation("roll_up", LoadStrip($"{gun}/Up/04 Stranded - Pack 4 back up-Roll Up.png", 64, 65, 12, false));
        sprite.AddAnimation("roll_down", LoadStrip($"{gun}/Down/04 Stranded - Pack 4 back up-Roll Down.png", 64, 65, 12, false));

        // Death
        sprite.AddAnimation("death", LoadStrip($"{gun}/04 Stranded - Pack 4 back up-Death.png", 64, 65, 10, false));

        // Character art is ~14.5px left of frame center in the right-facing sprites.
        // Shift draw position right so character visual center = entity position.
        sprite.PivotOffsetX = 14.5f;

        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for the hero (sword variant) with all directional animations.
    /// </summary>
    public static AnimatedSprite LoadHeroSword(string basePath)
    {
        var sprite = new AnimatedSprite();
        string sword = basePath + "/hero/With Sword";

        // Idle
        sprite.AddAnimation("idle_right", LoadStrip($"{sword}/Right Left/Idle left right.png", 64, 65, 8));
        sprite.AddAnimation("idle_up", LoadStrip($"{sword}/Up/04 Stranded - Pack 4 back up-Idle Up.png", 64, 65, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{sword}/Down/04 Stranded - Pack 4 back up-Idle Down.png", 64, 65, 8));

        // Run
        sprite.AddAnimation("run_right", LoadStrip($"{sword}/Right Left/R Run.png", 64, 65, 10));
        sprite.AddAnimation("run_up", LoadStrip($"{sword}/Up/04 Stranded - Pack 4 back up-Run UP.png", 64, 65, 10));
        sprite.AddAnimation("run_down", LoadStrip($"{sword}/Down/04 Stranded - Pack 4 back up-Run Down.png", 64, 65, 10));

        // Roll (dash)
        sprite.AddAnimation("roll_right", LoadStrip($"{sword}/Right Left/R Roll.png", 64, 65, 12, false));
        sprite.AddAnimation("roll_up", LoadStrip($"{sword}/Up/04 Stranded - Pack 4 back up-Roll Up.png", 64, 65, 12, false));
        sprite.AddAnimation("roll_down", LoadStrip($"{sword}/Down/04 Stranded - Pack 4 back up-Roll Down.png", 64, 65, 12, false));

        // Slash attack
        sprite.AddAnimation("slash_right", LoadStrip($"{sword}/Right Left/R Slash.png", 64, 65, 12, false));
        sprite.AddAnimation("slash_up", LoadStrip($"{sword}/Up/attack slash up.png", 64, 65, 12, false));
        sprite.AddAnimation("slash_down", LoadStrip($"{sword}/Down/04 Stranded - Pack 4 back up-Slash Down.png", 64, 65, 12, false));

        // Death
        sprite.AddAnimation("death", LoadStrip($"{sword}/04 Stranded - Pack 4 back up-Death.png", 64, 65, 10, false));

        // Character art is ~14.5px left of frame center in the right-facing sprites.
        // Shift draw position right so character visual center = entity position.
        sprite.PivotOffsetX = 14.5f;

        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for a tribe hunter enemy.
    /// </summary>
    public static AnimatedSprite LoadTribeHunter(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/enemies/tribe/Tribe Hunter";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Tribe Hunter-idle.png", 34, 37, 6));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Tribe Hunter-Idle Up.png", 34, 37, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Tribe Hunter-Idle Down.png", 34, 37, 6));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Tribe Hunter-walk.png", 34, 37, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Tribe Hunter-Walk Up.png", 34, 37, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Tribe Hunter-Walk Down.png", 34, 37, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Tribe Hunter-death.png", 34, 37, 10, false));

        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for a tribe warrior enemy.
    /// </summary>
    public static AnimatedSprite LoadTribeWarrior(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/enemies/tribe/Tribe Warrior";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Tribe Warrior-Idle.png", 62, 69, 6));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Tribe Warrior-idle up.png", 62, 69, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Tribe Warrior-idle down.png", 62, 69, 6));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Tribe Warrior-Walk.png", 62, 69, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Tribe Warrior-Walk up.png", 62, 69, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Tribe Warrior-Walk Down.png", 62, 69, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Tribe Warrior-death.png", 62, 69, 10, false));

        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite LoadSmallBug(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/enemies/insects/Small Bug";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Small Insect-Death.png", 20, 28, 10, false));

        // Bugs don't have directional variants — reuse same animation
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));

        sprite.Play("idle_right");
        return sprite;
    }

    public static AnimatedSprite LoadMediumInsect(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/enemies/insects/Medium Bug";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 6));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Medium Insect-Death.png", 34, 37, 10, false));

        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 6));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 8));

        sprite.Play("idle_right");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for a starter pack Archer enemy (grid sheet 32x32, 8 cols).
    /// </summary>
    public static AnimatedSprite? LoadStarterArcher(string basePath)
    {
        string path = basePath + "/enemies/starter_archer/archer.png";
        if (!File.Exists(path)) return null;
        var tex = Raylib.LoadTexture(path);
        var sprite = new AnimatedSprite();
        // Archer: idle 1-5(5f), run 11-14(4f), shoot 19-25(7f), hit 33-34(2f), dead 35-42(8f)
        int cols = 8;
        sprite.AddAnimation("idle_right", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        // death: 35-42 but grid only has 40 cells (8x5), cap at frame 39
        sprite.AddAnimation("death", FromGrid(tex, 32, 32, cols, 35, 5, 8, false));
        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for a starter pack Guard enemy (grid sheet 32x32, 16 cols).
    /// </summary>
    public static AnimatedSprite? LoadStarterGuard(string basePath)
    {
        string path = basePath + "/enemies/starter_guard/guard.png";
        if (!File.Exists(path)) return null;
        var tex = Raylib.LoadTexture(path);
        var sprite = new AnimatedSprite();
        // Guard: idle 1-5(5f), run 11-14(4f), attack 19-23(5f), dead 29-44(16f)
        int cols = 16;
        sprite.AddAnimation("idle_right", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("death", FromGrid(tex, 32, 32, cols, 29, 16, 10, false));
        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for a starter pack Warrior enemy (grid sheet 32x32, 8 cols).
    /// </summary>
    public static AnimatedSprite? LoadStarterWarrior(string basePath)
    {
        string path = basePath + "/enemies/starter_warrior/warrior.png";
        if (!File.Exists(path)) return null;
        var tex = Raylib.LoadTexture(path);
        var sprite = new AnimatedSprite();
        // Warrior: idle 1-5(5f), run 11-14(4f), attack 19-23(5f), hit 29-30(2f), dead 33-40(8f)
        int cols = 8;
        sprite.AddAnimation("idle_right", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("death", FromGrid(tex, 32, 32, cols, 33, 8, 10, false));
        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Load all enemy animated sprites, indexed to match EnemyDatabase order.
    /// 0=TribeHunter, 1=SmallBug, 2=MediumInsect, 3=TribeWarrior, 4=Archer, 5=Guard, 6=Warrior
    /// </summary>
    public static AnimatedSprite?[] LoadEnemySprites(string basePath)
    {
        var sprites = new AnimatedSprite?[7];
        sprites[0] = LoadTribeHunter(basePath);
        sprites[1] = LoadSmallBug(basePath);
        sprites[2] = LoadMediumInsect(basePath);
        sprites[3] = LoadTribeWarrior(basePath);
        sprites[4] = LoadStarterArcher(basePath);
        sprites[5] = LoadStarterGuard(basePath);
        sprites[6] = LoadStarterWarrior(basePath);
        return sprites;
    }

    public static AnimatedSprite LoadDustWarrior(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/bosses/dust_warrior";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Warrior-Idle.png", 67, 45, 8));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Warrior-Idle.png", 67, 45, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Warrior-Idle.png", 67, 45, 8));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Warrior-Run .png", 67, 45, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Warrior-Run .png", 67, 45, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Warrior-Run .png", 67, 45, 8));
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Warrior-Attack.png", 67, 45, 10, false));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Warrior-Death.png", 67, 45, 8, false));

        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadBossSprite(string basePath)
    {
        string bossPath = basePath + "/bosses";
        if (!Directory.Exists(bossPath)) return null;

        // Load Dust Warrior as the default boss sprite
        if (Directory.Exists(bossPath + "/dust_warrior"))
            return LoadDustWarrior(basePath);

        return null;
    }
}
