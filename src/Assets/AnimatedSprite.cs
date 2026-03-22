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
        // Snap to integer pixels to prevent subpixel blur on pixel art
        var dest = new Rectangle(MathF.Round(x + ox * scale), MathF.Round(y + oy * scale), w, h);
        var origin = new Vector2(MathF.Round(w / 2f), MathF.Round(h / 2f));
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
        var dest = new Rectangle(MathF.Round(x), MathF.Round(y), w, h);
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
        // Snap to integer pixels to prevent subpixel blur on pixel art
        var dest = new Rectangle(MathF.Round(x + ox * scale), MathF.Round(y + oy * scale), w, h);
        var origin = new Vector2(MathF.Round(w / 2f), MathF.Round(h / 2f));
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
    /// <summary>Load a texture with point filtering for crisp pixel art.</summary>
    private static Texture2D LoadTexturePoint(string path)
    {
        var tex = Raylib.LoadTexture(path);
        Raylib.SetTextureFilter(tex, TextureFilter.Point);
        return tex;
    }

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
        var tex = LoadTexturePoint(path);
        int frameCount = tex.Width / frameWidth;
        return new SpriteAnimation(tex, frameWidth, frameHeight, frameCount, fps) { Loop = loop };
    }

    /// <summary>
    /// Load a horizontal strip and derive frame count from image width / frameWidth,
    /// using explicit frameHeight. Overload uses image height when frameHeight matches.
    /// </summary>
    public static SpriteAnimation LoadStripAutoCount(string path, int frameWidth, float fps, bool loop = true)
    {
        var tex = LoadTexturePoint(path);
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
        // Y offset shifts sprite up so entity position aligns with character's feet/shadow.
        sprite.PivotOffsetX = 15f;
        sprite.PivotOffsetY = -8f;

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

        sprite.PivotOffsetX = 15f;
        sprite.PivotOffsetY = -8f;

        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for the Starter Pack hero (grid sheet 32x32, 10 cols × 6 rows).
    /// 320x192 image from Starter Pack v1.
    /// </summary>
    public static AnimatedSprite? LoadStarterHero(string basePath)
    {
        string path = basePath + "/hero/starter_hero/hero.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 10; // 320 / 32 = 10

        // Row 0: idle (5 frames, front-facing standing/bob)
        sprite.AddAnimation("idle_down", FromGrid(tex, 32, 32, cols, 0, 5, 8));
        // Row 1: walk down (4 frames, front-facing walk cycle)
        sprite.AddAnimation("run_down", FromGrid(tex, 32, 32, cols, 10, 4, 10));
        // Row 3: walk right (4 frames from 8-frame repeating cycle, side-facing)
        sprite.AddAnimation("idle_right", FromGrid(tex, 32, 32, cols, 30, 4, 8));
        sprite.AddAnimation("run_right", FromGrid(tex, 32, 32, cols, 30, 4, 10));
        // Row 2: walk up (8 frames, back-facing walk cycle)
        sprite.AddAnimation("idle_up", FromGrid(tex, 32, 32, cols, 20, 8, 8));
        sprite.AddAnimation("run_up", FromGrid(tex, 32, 32, cols, 20, 8, 10));
        // Row 4: hit reaction (2 frames, used for dodge roll)
        sprite.AddAnimation("roll_down", FromGrid(tex, 32, 32, cols, 40, 2, 10, false));
        sprite.AddAnimation("roll_right", FromGrid(tex, 32, 32, cols, 40, 2, 10, false));
        sprite.AddAnimation("roll_up", FromGrid(tex, 32, 32, cols, 40, 2, 10, false));
        // Row 5: death sparkle (10 frames, fading particle effect)
        sprite.AddAnimation("death", FromGrid(tex, 32, 32, cols, 50, 10, 10, false));

        sprite.PivotOffsetY = -4f;
        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Build an AnimatedSprite for the Starter Pack companion (grid sheet 32x32, 10 cols × 4 rows).
    /// 320x128 image. Companion follows the Drifter hero.
    /// </summary>
    public static AnimatedSprite? LoadCompanion(string basePath)
    {
        string path = basePath + "/hero/starter_hero/companion.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 10; // 320 / 32 = 10

        // Row 0: idle bob (4 frames)
        sprite.AddAnimation("idle", FromGrid(tex, 32, 32, cols, 0, 4, 6));
        // Row 1: move (4 frames)
        sprite.AddAnimation("move", FromGrid(tex, 32, 32, cols, 10, 4, 8));
        // Row 2: pulse/gather (4 frames)
        sprite.AddAnimation("gather", FromGrid(tex, 32, 32, cols, 20, 4, 8));
        // Row 3: attack/activate (longer animation)
        sprite.AddAnimation("attack", FromGrid(tex, 32, 32, cols, 30, 10, 10));

        sprite.Play("idle");
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

        // Attack (shoot) — 7 frames per direction
        sprite.AddAnimation("attack_right", LoadStrip($"{dir}/Tribe Hunter-Shoot.png", 34, 37, 12, false));
        sprite.AddAnimation("attack_up", LoadStrip($"{dir}/Tribe Hunter-Shoot Up.png", 34, 37, 12, false));
        sprite.AddAnimation("attack_down", LoadStrip($"{dir}/Tribe Hunter-Shoot down.png", 34, 37, 12, false));

        // Hunter body sits high in 34x37 frame; cloak extends below
        sprite.PivotOffsetY = -6f;

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

        // Attack — 11 frames per direction
        sprite.AddAnimation("attack_right", LoadStrip($"{dir}/Tribe Warrior-attack.png", 62, 69, 12, false));
        sprite.AddAnimation("attack_up", LoadStrip($"{dir}/Tribe Warrior-Attack Up.png", 62, 69, 12, false));
        sprite.AddAnimation("attack_down", LoadStrip($"{dir}/Tribe Warrior-attack down.png", 62, 69, 12, false));

        // Large frame (62x69) — warrior body sits high, weapon extends left
        sprite.PivotOffsetY = -12f;

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
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Small Insect-Attack.png", 20, 28, 12, false));

        // Bugs don't have directional variants — reuse same animation
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Small insect - idle move.png", 20, 28, 6));

        // Small bug (20x28) — body near top of frame, legs extend down
        sprite.PivotOffsetY = -3f;

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
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Medium Insect-Attack.png", 34, 37, 12, false));

        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 6));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Medium Insect-idleMove.png", 34, 37, 8));

        // Medium bug (34x37) — body slightly above center
        sprite.PivotOffsetY = -4f;

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
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        // Archer: idle 1-5(5f), run 11-14(4f), shoot 19-25(7f), hit 33-34(2f), dead 35-42(8f)
        int cols = 8;
        sprite.AddAnimation("idle_right", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        // shoot: frames 19-25 (7 frames)
        sprite.AddAnimation("attack", FromGrid(tex, 32, 32, cols, 19, 7, 12, false));
        // death: 35-42 but grid only has 40 cells (8x5), cap at frame 39
        sprite.AddAnimation("death", FromGrid(tex, 32, 32, cols, 35, 5, 8, false));
        // Grid sheet (32x32) — character feet near bottom
        sprite.PivotOffsetY = -4f;
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
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        // Guard: idle 1-5(5f), run 11-14(4f), attack 19-23(5f), dead 29-44(16f)
        int cols = 16;
        sprite.AddAnimation("idle_right", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        // attack: frames 19-23 (5 frames)
        sprite.AddAnimation("attack", FromGrid(tex, 32, 32, cols, 19, 5, 12, false));
        sprite.AddAnimation("death", FromGrid(tex, 32, 32, cols, 29, 16, 10, false));
        // Stocky guard — slightly above center in 32x32 frame
        sprite.PivotOffsetY = -3f;
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
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        // Warrior: idle 1-5(5f), run 11-14(4f), attack 19-23(5f), hit 29-30(2f), dead 33-40(8f)
        int cols = 8;
        sprite.AddAnimation("idle_right", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 32, 32, cols, 1, 5, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 32, 32, cols, 11, 4, 8));
        // attack: frames 19-23 (5 frames)
        sprite.AddAnimation("attack", FromGrid(tex, 32, 32, cols, 19, 5, 12, false));
        sprite.AddAnimation("death", FromGrid(tex, 32, 32, cols, 32, 8, 10, false));
        // Warrior — feet near bottom of 32x32 frame
        sprite.PivotOffsetY = -4f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite LoadBigBug(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/enemies/insects/Big Bug";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Big Insect-moveidle.png", 72, 44, 8));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Big Insect-moveidle.png", 72, 44, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Big Insect-Death.png", 72, 44, 13, false));
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Big Insect-Attack.png", 72, 44, 12, false));

        // Reuse for all directions (non-directional sprite)
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Big Insect-moveidle.png", 72, 44, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Big Insect-moveidle.png", 72, 44, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Big Insect-moveidle.png", 72, 44, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Big Insect-moveidle.png", 72, 44, 8));

        // Big bug (72x44) — body mass sits above center
        sprite.PivotOffsetY = -6f;
        sprite.Play("idle_right");
        return sprite;
    }

    public static AnimatedSprite LoadSpinyBeetle(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/enemies/insects/Medium bug 2";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Medium2 bug-Idle Move.png", 88, 37, 8));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Medium2 bug-Idle Move.png", 88, 37, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Medium2 bug-Death.png", 88, 37, 11, false));
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Medium2 bug-Attack.png", 88, 37, 12, false));

        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Medium2 bug-Idle Move.png", 88, 37, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Medium2 bug-Idle Move.png", 88, 37, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Medium2 bug-Idle Move.png", 88, 37, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Medium2 bug-Idle Move.png", 88, 37, 8));

        sprite.PivotOffsetY = -4f;
        sprite.Play("idle_right");
        return sprite;
    }

    public static AnimatedSprite LoadTamedBeast(string basePath)
    {
        var sprite = new AnimatedSprite();
        string dir = basePath + "/enemies/tribe/Tribe Tamed Beast";

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Tribe Tamed Beast-Idle.png", 76, 67, 6));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Tribe Tamed Beast-Up Idle.png", 76, 67, 6));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Tribe Tamed Beast-Down Idle.png", 76, 67, 6));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Tribe Tamed Beast-move lr.png", 76, 67, 6));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Tribe Tamed Beast-Move Up.png", 76, 67, 6));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Tribe Tamed Beast-Move Down.png", 76, 67, 6));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Tribe Tamed Beast-death.png", 76, 67, 17, false));

        // Attack — 16 frames per direction
        sprite.AddAnimation("attack_right", LoadStrip($"{dir}/Tribe Tamed Beast - Attack Left Right.png", 76, 67, 12, false));
        sprite.AddAnimation("attack_up", LoadStrip($"{dir}/Tribe Tamed Beast-Attack Up.png", 76, 67, 12, false));
        sprite.AddAnimation("attack_down", LoadStrip($"{dir}/Tribe Tamed Beast-Attack Down.png", 76, 67, 12, false));

        // Large frame (76x67) — beast body sits high, shadow at bottom
        sprite.PivotOffsetY = -12f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadRustyRobot(string basePath)
    {
        string path = basePath + "/enemies/robots/Rusty Robot/Rusty Robot 20x29 without Shadow.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 8;
        sprite.AddAnimation("idle_right", FromGrid(tex, 20, 29, cols, 0, 8, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 20, 29, cols, 0, 8, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 20, 29, cols, 0, 8, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 20, 29, cols, 8, 8, 10));
        sprite.AddAnimation("walk_up", FromGrid(tex, 20, 29, cols, 8, 8, 10));
        sprite.AddAnimation("walk_down", FromGrid(tex, 20, 29, cols, 8, 8, 10));
        sprite.AddAnimation("death", FromGrid(tex, 20, 29, cols, 8, 8, 10, false));
        sprite.PivotOffsetY = -3f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadGuardRobot(string basePath)
    {
        string path = basePath + "/enemies/robots/Guard Robot/Robot 1 - Blue 26x34 without shadows.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 10;
        sprite.AddAnimation("idle_right", FromGrid(tex, 26, 34, cols, 10, 8, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 26, 34, cols, 10, 8, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 26, 34, cols, 10, 8, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 26, 34, cols, 20, 8, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 26, 34, cols, 20, 8, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 26, 34, cols, 20, 8, 8));
        // Row 3: attack (arms extend) — 10 frames
        sprite.AddAnimation("attack", FromGrid(tex, 26, 34, cols, 30, 10, 12, false));
        sprite.AddAnimation("death", FromGrid(tex, 26, 34, cols, 40, 10, 10, false));
        // Guard robot (26x34) — body above center, legs at bottom
        sprite.PivotOffsetY = -5f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadCircleBot(string basePath)
    {
        string path = basePath + "/enemies/robots/Circle Bot/Circle Bot blue 29x35 without shadow.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 8;
        sprite.AddAnimation("idle_right", FromGrid(tex, 29, 35, cols, 0, 8, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 29, 35, cols, 0, 8, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 29, 35, cols, 0, 8, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 29, 35, cols, 8, 8, 10));
        sprite.AddAnimation("walk_up", FromGrid(tex, 29, 35, cols, 8, 8, 10));
        sprite.AddAnimation("walk_down", FromGrid(tex, 29, 35, cols, 8, 8, 10));
        // Row 2: attack (blue energy pulse) — 8 frames
        sprite.AddAnimation("attack", FromGrid(tex, 29, 35, cols, 16, 8, 12, false));
        sprite.AddAnimation("death", FromGrid(tex, 29, 35, cols, 24, 8, 10, false));
        // Circle bot (29x35) — round body sits above center
        sprite.PivotOffsetY = -5f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadDeliveryBot(string basePath)
    {
        string path = basePath + "/enemies/robots/Delivery Bot/Delivery Bot yellow without shadow 23x21.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 6;
        sprite.AddAnimation("idle_right", FromGrid(tex, 23, 21, cols, 0, 6, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 23, 21, cols, 0, 6, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 23, 21, cols, 0, 6, 8));
        sprite.AddAnimation("walk_right", FromGrid(tex, 23, 21, cols, 6, 6, 10));
        sprite.AddAnimation("walk_up", FromGrid(tex, 23, 21, cols, 6, 6, 10));
        sprite.AddAnimation("walk_down", FromGrid(tex, 23, 21, cols, 6, 6, 10));
        // Row 2: attack/shoot (6 frames)
        sprite.AddAnimation("attack", FromGrid(tex, 23, 21, cols, 12, 6, 12, false));
        sprite.AddAnimation("death", FromGrid(tex, 23, 21, cols, 18, 6, 10, false));
        sprite.PivotOffsetY = -2f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadHoodedMinion(string basePath)
    {
        string dir = basePath + "/enemies/minions/Minion 1/Sprites Without Shadows";
        if (!Directory.Exists(dir)) return null;
        var sprite = new AnimatedSprite();

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Minion 1-idle.png", 33, 36, 8));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Minion 1-idle.png", 33, 36, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Minion 1-idle.png", 33, 36, 8));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Minion 1-Run.png", 33, 36, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Minion 1-Run.png", 33, 36, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Minion 1-Run.png", 33, 36, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Minion 1-Death.png", 33, 36, 8, false));
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Minion 1-Attack.png", 33, 36, 12, false));

        sprite.PivotOffsetY = -4f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadBombMinion(string basePath)
    {
        string dir = basePath + "/enemies/minions/Minion 2/Sprites Without Shadows";
        if (!Directory.Exists(dir)) return null;
        var sprite = new AnimatedSprite();

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Minion 2-Idle.png", 13, 15, 8));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Minion 2-Idle.png", 13, 15, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Minion 2-Idle.png", 13, 15, 8));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Minion 2-Run.png", 13, 15, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Minion 2-Run.png", 13, 15, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Minion 2-Run.png", 13, 15, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Minion 2-Prep Explode.png", 13, 15, 4, false));

        // Explosion VFX — large separate strip (124x77, 15 frames)
        string explPath = basePath + "/enemies/minions/Minion 2/Minion 2 Explosion - 124x71.png";
        if (File.Exists(explPath))
            sprite.AddAnimation("explode", LoadStrip(explPath, 124, 77, 10, false));

        sprite.PivotOffsetY = -1f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadPlanterBot(string basePath)
    {
        string path = basePath + "/enemies/robots/Planter Robot/Planter Bot Blue no shadow.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 24; // 696 / 29 = 24
        // Row 0: idle (8 frames)
        sprite.AddAnimation("idle_right", FromGrid(tex, 29, 37, cols, 0, 8, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 29, 37, cols, 0, 8, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 29, 37, cols, 0, 8, 8));
        // Row 1: walk (8 frames)
        sprite.AddAnimation("walk_right", FromGrid(tex, 29, 37, cols, 24, 8, 10));
        sprite.AddAnimation("walk_up", FromGrid(tex, 29, 37, cols, 24, 8, 10));
        sprite.AddAnimation("walk_down", FromGrid(tex, 29, 37, cols, 24, 8, 10));
        // Row 2: plant/attack (24 frames)
        sprite.AddAnimation("attack", FromGrid(tex, 29, 37, cols, 48, 24, 12, false));
        // Use end of attack as death
        sprite.AddAnimation("death", FromGrid(tex, 29, 37, cols, 60, 12, 10, false));
        // Planter bot (29x37) — body above center, treads at bottom
        sprite.PivotOffsetY = -5f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadRangedMinion(string basePath)
    {
        string dir = basePath + "/enemies/minions/Minion 3/Sprites without Shadow";
        if (!Directory.Exists(dir)) return null;
        var sprite = new AnimatedSprite();

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Minion 3-Idle.png", 25, 15, 8));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Minion 3-Idle.png", 25, 15, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Minion 3-Idle.png", 25, 15, 8));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/Minion 3-Run.png", 25, 15, 8));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Minion 3-Run.png", 25, 15, 8));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Minion 3-Run.png", 25, 15, 8));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Minion 3-Death.png", 25, 15, 7, false));
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Minion 3-Range Attack.png", 25, 15, 12, false));

        sprite.PivotOffsetY = -1f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadBlowfish(string basePath)
    {
        string dir = basePath + "/enemies/blowfish";
        if (!Directory.Exists(dir)) return null;
        var sprite = new AnimatedSprite();

        sprite.AddAnimation("idle_right", LoadStrip($"{dir}/Blowfish-Big Idle.png", 94, 47, 8));
        sprite.AddAnimation("idle_up", LoadStrip($"{dir}/Blowfish-Big Idle.png", 94, 47, 8));
        sprite.AddAnimation("idle_down", LoadStrip($"{dir}/Blowfish-Big Idle.png", 94, 47, 8));
        sprite.AddAnimation("walk_right", LoadStrip($"{dir}/move Left & Right.png", 94, 47, 12));
        sprite.AddAnimation("walk_up", LoadStrip($"{dir}/Move Up.png", 94, 47, 18));
        sprite.AddAnimation("walk_down", LoadStrip($"{dir}/Move Down.png", 94, 47, 18));
        sprite.AddAnimation("attack", LoadStrip($"{dir}/Blowfish-Attack out of Ground Down.png", 94, 47, 13, false));
        sprite.AddAnimation("death", LoadStrip($"{dir}/Blowfish-Death.png", 94, 47, 14, false));

        sprite.PivotOffsetY = -6f;
        sprite.Play("idle_down");
        return sprite;
    }

    public static AnimatedSprite? LoadTarnishedWidow(string basePath)
    {
        string path = basePath + "/bosses/tarnished_widow/The Tarnished Widow 188x90.png";
        if (!File.Exists(path)) return null;
        var tex = LoadTexturePoint(path);
        var sprite = new AnimatedSprite();
        int cols = 18; // 3384 / 188 = 18 cols, 720 / 90 = 8 rows

        // Row 1: Idle (8 frames)
        sprite.AddAnimation("idle_right", FromGrid(tex, 188, 90, cols, 0, 8, 8));
        sprite.AddAnimation("idle_up", FromGrid(tex, 188, 90, cols, 0, 8, 8));
        sprite.AddAnimation("idle_down", FromGrid(tex, 188, 90, cols, 0, 8, 8));
        // Row 2: Walk (10 frames)
        sprite.AddAnimation("walk_right", FromGrid(tex, 188, 90, cols, 18, 10, 8));
        sprite.AddAnimation("walk_up", FromGrid(tex, 188, 90, cols, 18, 10, 8));
        sprite.AddAnimation("walk_down", FromGrid(tex, 188, 90, cols, 18, 10, 8));
        // Row 5: Attack with blood (10 frames)
        sprite.AddAnimation("attack", FromGrid(tex, 188, 90, cols, 72, 10, 10, false));
        // Row 8: Death explosion (18 frames)
        sprite.AddAnimation("death", FromGrid(tex, 188, 90, cols, 126, 18, 8, false));

        sprite.PivotOffsetY = -10f;
        sprite.Play("idle_down");
        return sprite;
    }

    /// <summary>
    /// Load all enemy animated sprites, indexed to match EnemyDatabase order.
    /// 0=TribeHunter, 1=SmallBug, 2=MediumInsect, 3=TribeWarrior, 4=Archer, 5=Guard, 6=Warrior
    /// 7=BigBug, 8=SpinyBeetle, 9=RelicGuardian, 10=RustyRobot, 11=GuardRobot
    /// 12=CircleBot, 13=DeliveryBot, 14=HoodedMinion, 15=BombMinion, 16=RangedMinion
    /// 17=PlanterBot
    /// </summary>
    public static AnimatedSprite?[] LoadEnemySprites(string basePath)
    {
        var sprites = new AnimatedSprite?[18];
        sprites[0] = LoadTribeHunter(basePath);
        sprites[1] = LoadSmallBug(basePath);
        sprites[2] = LoadMediumInsect(basePath);
        sprites[3] = LoadTribeWarrior(basePath);
        sprites[4] = LoadStarterArcher(basePath);
        sprites[5] = LoadStarterGuard(basePath);
        sprites[6] = LoadStarterWarrior(basePath);
        sprites[7] = LoadBigBug(basePath);
        sprites[8] = LoadSpinyBeetle(basePath);
        sprites[9] = LoadTamedBeast(basePath);
        sprites[10] = LoadRustyRobot(basePath);
        sprites[11] = LoadGuardRobot(basePath);
        sprites[12] = LoadCircleBot(basePath);
        sprites[13] = LoadDeliveryBot(basePath);
        sprites[14] = LoadHoodedMinion(basePath);
        sprites[15] = LoadBombMinion(basePath);
        sprites[16] = LoadRangedMinion(basePath);
        sprites[17] = LoadPlanterBot(basePath);
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

        // Wide frame (67x45) — warrior body above center
        sprite.PivotOffsetY = -6f;

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
