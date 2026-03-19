using CloneTato.Assets;
using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class SpriteGalleryScreen
{
    private int _selectedEntry;
    private int _selectedAnim;
    private bool _showHitbox = true;
    private float _animTimer;

    private GalleryEntry[] _entries = Array.Empty<GalleryEntry>();
    private bool _initialized;

    private class GalleryEntry
    {
        public string Name = "";
        public AnimatedSprite? Sprite;
        public string[] AnimationNames = Array.Empty<string>();
        public float Radius; // hitbox radius
        public float DrawScale = 1f;
    }

    private void InitEntries(GameState state)
    {
        if (_initialized) return;
        _initialized = true;

        var list = new List<GalleryEntry>();

        // Hero (gun)
        if (state.Assets.HeroSprite != null)
        {
            list.Add(new GalleryEntry
            {
                Name = "Hero (Gun)",
                Sprite = state.Assets.HeroSprite,
                AnimationNames = new[]
                {
                    "idle_right", "idle_up", "idle_down",
                    "run_right", "run_up", "run_down",
                    "roll_right", "roll_up", "roll_down",
                    "death"
                },
                Radius = 10f,
                DrawScale = 1f,
            });
        }

        // Enemies
        string[] enemyNames = { "Tribe Hunter", "Small Bug", "Medium Insect", "Tribe Warrior",
            "Archer", "Guard", "Warrior" };
        float[] enemyRadii = { 10f, 8f, 9f, 12f, 10f, 12f, 11f };
        for (int i = 0; i < state.Assets.EnemySprites.Length; i++)
        {
            var eSprite = state.Assets.EnemySprites[i];
            if (eSprite == null) continue;

            var anims = new List<string>();
            foreach (var name in new[] { "idle_right", "idle_up", "idle_down",
                "walk_right", "walk_up", "walk_down", "death" })
            {
                if (eSprite.HasAnimation(name)) anims.Add(name);
            }

            list.Add(new GalleryEntry
            {
                Name = i < enemyNames.Length ? enemyNames[i] : $"Enemy {i}",
                Sprite = eSprite,
                AnimationNames = anims.ToArray(),
                Radius = i < enemyRadii.Length ? enemyRadii[i] : 10f,
                DrawScale = 1f,
            });
        }

        // Boss
        if (state.Assets.BossSprite != null)
        {
            var bossAnims = new List<string>();
            foreach (var name in new[] { "idle_right", "idle_up", "idle_down",
                "walk_right", "walk_up", "walk_down", "attack", "death" })
            {
                if (state.Assets.BossSprite.HasAnimation(name)) bossAnims.Add(name);
            }

            list.Add(new GalleryEntry
            {
                Name = "Dust Warrior (Boss)",
                Sprite = state.Assets.BossSprite,
                AnimationNames = bossAnims.ToArray(),
                Radius = 20f,
                DrawScale = 1.5f,
            });
        }

        _entries = list.ToArray();
    }

    public void Reset()
    {
        _selectedEntry = 0;
        _selectedAnim = 0;
        _animTimer = 0;
        // Don't reset _initialized — sprites stay loaded
    }

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        InitEntries(state);

        if (InputHelper.IsCancelPressed())
        {
            manager.TransitionTo(GameScreen.MainMenu);
            return;
        }

        // Up/Down to change entry
        int vDir = InputHelper.GetMenuVertical();
        if (vDir != 0 && _entries.Length > 0)
        {
            _selectedEntry = (_selectedEntry + vDir + _entries.Length) % _entries.Length;
            _selectedAnim = 0;
            _animTimer = 0;
        }

        // Left/Right to change animation
        int hDir = InputHelper.GetMenuHorizontal();
        if (hDir != 0 && _entries.Length > 0)
        {
            var entry = _entries[_selectedEntry];
            if (entry.AnimationNames.Length > 0)
            {
                _selectedAnim = (_selectedAnim + hDir + entry.AnimationNames.Length) % entry.AnimationNames.Length;
                _animTimer = 0;
            }
        }

        // H to toggle hitbox
        if (Raylib.IsKeyPressed(KeyboardKey.H))
            _showHitbox = !_showHitbox;

        _animTimer += dt;
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        InitEntries(state);
        Raylib.ClearBackground(new Color(30, 30, 35, 255));

        // Title
        UIRenderer.DrawTextMedium("SPRITE GALLERY", 10, 8, Color.Gold);
        UIRenderer.DrawTextSmall("Up/Down: character  Left/Right: animation  H: toggle hitbox  ESC: back",
            10, 24, Color.Gray);

        if (_entries.Length == 0)
        {
            UIRenderer.DrawTextSmall("No STRANDED sprites loaded.", 10, 60, Color.Red);
            return;
        }

        // Left panel: entry list
        int listX = 10;
        int listY = 44;
        int listSpacing = 14;
        for (int i = 0; i < _entries.Length; i++)
        {
            Color c = i == _selectedEntry ? Color.Gold : Color.LightGray;
            string prefix = i == _selectedEntry ? "> " : "  ";
            UIRenderer.DrawTextSmall($"{prefix}{_entries[i].Name}", listX, listY + i * listSpacing, c);
        }

        // Preview area
        var entry = _entries[_selectedEntry];
        if (entry.Sprite == null || entry.AnimationNames.Length == 0) return;

        string animName = entry.AnimationNames[_selectedAnim];

        // Animation name list (horizontal below title area)
        int animListY = listY + _entries.Length * listSpacing + 8;
        UIRenderer.DrawTextSmall("Animations:", listX, animListY, Color.White);
        int animX = listX;
        int animY = animListY + 12;
        for (int i = 0; i < entry.AnimationNames.Length; i++)
        {
            Color c = i == _selectedAnim ? Color.Gold : Color.Gray;
            UIRenderer.DrawTextSmall(entry.AnimationNames[i], animX, animY, c);
            animY += 11;
        }

        // Draw the sprite preview (large, centered in right side of screen)
        float previewX = Constants.LogicalWidth * 0.65f;
        float previewY = Constants.LogicalHeight * 0.45f;
        float previewScale = entry.DrawScale * 3f; // scale up for visibility

        // Checkerboard background behind sprite
        int bgSize = 80;
        int bgX = (int)previewX - bgSize / 2;
        int bgY = (int)previewY - bgSize / 2;
        for (int cx = 0; cx < bgSize; cx += 8)
        {
            for (int cy = 0; cy < bgSize; cy += 8)
            {
                bool dark = ((cx / 8) + (cy / 8)) % 2 == 0;
                Raylib.DrawRectangle(bgX + cx, bgY + cy, 8, 8,
                    dark ? new Color(40, 40, 45, 255) : new Color(55, 55, 60, 255));
            }
        }

        // Compute animation frame
        int frameCount = entry.Sprite.GetFrameCount(animName);
        float frameDur = entry.Sprite.GetFrameDuration(animName);
        int frame = frameCount > 0 ? (int)(_animTimer / frameDur) % frameCount : 0;

        // Draw with flipH for "left" viewing
        bool flipH = false;
        entry.Sprite.DrawAnimationFrame(animName, frame, flipH,
            previewX, previewY, Color.White, previewScale);

        // Hitbox circle (offset to show where entity position is relative to sprite)
        if (_showHitbox)
        {
            // Entity position is at (previewX, previewY) — the sprite is offset by pivot
            Raylib.DrawCircleLines((int)previewX, (int)previewY, entry.Radius * previewScale / entry.DrawScale,
                new Color(0, 255, 0, 180));
            // Entity position dot
            Raylib.DrawCircleV(new System.Numerics.Vector2(previewX, previewY), 2f, Color.Red);
            // Cross at entity position
            Raylib.DrawLineV(
                new System.Numerics.Vector2(previewX - 6, previewY),
                new System.Numerics.Vector2(previewX + 6, previewY), Color.Red);
            Raylib.DrawLineV(
                new System.Numerics.Vector2(previewX, previewY - 6),
                new System.Numerics.Vector2(previewX, previewY + 6), Color.Red);
        }

        // Info text
        int infoX = (int)previewX - 60;
        int infoY = (int)previewY + bgSize / 2 + 10;
        UIRenderer.DrawTextSmall($"Animation: {animName}", infoX, infoY, Color.White);
        UIRenderer.DrawTextSmall($"Frames: {frameCount}  FPS: {(frameDur > 0 ? (int)(1f / frameDur) : 0)}",
            infoX, infoY + 12, Color.White);
        UIRenderer.DrawTextSmall($"Frame Size: {entry.Sprite.GetFrameWidth(animName)}x{entry.Sprite.GetFrameHeight(animName)}",
            infoX, infoY + 24, Color.White);
        UIRenderer.DrawTextSmall($"Hitbox Radius: {entry.Radius:F0}px", infoX, infoY + 36,
            _showHitbox ? Color.Green : Color.Gray);
        UIRenderer.DrawTextSmall($"Draw Scale: {previewScale:F1}x", infoX, infoY + 48, Color.White);

        // Hitbox toggle indicator
        string hitboxText = _showHitbox ? "[H] Hitbox: ON" : "[H] Hitbox: OFF";
        UIRenderer.DrawTextSmall(hitboxText, infoX, infoY + 64,
            _showHitbox ? Color.Green : Color.DarkGray);
    }
}
