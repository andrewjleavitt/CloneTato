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

    public Rectangle GetFrameRect(int frame)
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
/// Helper to load animation strips from the STRANDED asset pack.
/// </summary>
public static class AnimationLoader
{
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

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Tribe Hunter-idle.png", 32, 37, 6));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Tribe Hunter-Idle Up.png", 32, 37, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Tribe Hunter-Idle Down.png", 32, 37, 6));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Tribe Hunter-walk.png", 32, 37, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Tribe Hunter-Walk Up.png", 32, 37, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Tribe Hunter-Walk Down.png", 32, 37, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Tribe Hunter-death.png", 32, 37, 10, false));

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
}
