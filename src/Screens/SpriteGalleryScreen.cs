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
    private int _scrollOffset; // for scrolling the entry list

    private GalleryEntry[] _entries = Array.Empty<GalleryEntry>();
    private bool _initialized;

    private class GalleryEntry
    {
        public string Name = "";
        public string Category = "";
        public AnimatedSprite? Sprite;
        public string[] AnimationNames = Array.Empty<string>();
        public float Radius;
        public float DrawScale = 1f;
    }

    private void InitEntries(GameState state)
    {
        if (_initialized) return;
        _initialized = true;

        var list = new List<GalleryEntry>();

        // Heroes
        if (state.Assets.HeroGunSprite != null)
        {
            list.Add(new GalleryEntry
            {
                Name = "Gunslinger", Category = "HERO",
                Sprite = state.Assets.HeroGunSprite,
                AnimationNames = new[]
                {
                    "idle_right", "idle_up", "idle_down",
                    "run_right", "run_up", "run_down",
                    "roll_right", "roll_up", "roll_down",
                    "death"
                },
                Radius = 10f, DrawScale = 1f,
            });
        }
        if (state.Assets.HeroSwordSprite != null)
        {
            list.Add(new GalleryEntry
            {
                Name = "Blade Dancer", Category = "HERO",
                Sprite = state.Assets.HeroSwordSprite,
                AnimationNames = new[]
                {
                    "idle_right", "idle_up", "idle_down",
                    "run_right", "run_up", "run_down",
                    "roll_right", "roll_up", "roll_down",
                    "slash_right", "slash_up", "slash_down",
                    "death"
                },
                Radius = 10f, DrawScale = 1f,
            });
        }
        if (state.Assets.StarterHeroSprite != null)
        {
            list.Add(new GalleryEntry
            {
                Name = "Drifter", Category = "HERO",
                Sprite = state.Assets.StarterHeroSprite,
                AnimationNames = new[]
                {
                    "idle_right", "idle_up", "idle_down",
                    "run_right", "run_up", "run_down",
                    "roll_right", "roll_up", "roll_down",
                    "death"
                },
                Radius = 8f, DrawScale = 1.5f,
            });
        }
        if (state.Assets.CompanionSprite != null)
        {
            list.Add(new GalleryEntry
            {
                Name = "Companion", Category = "HERO",
                Sprite = state.Assets.CompanionSprite,
                AnimationNames = new[] { "idle", "move", "gather", "attack" },
                Radius = 4f, DrawScale = 2f,
            });
        }

        // Enemy definitions with categories
        var enemyInfo = new (string name, string category, float radius)[]
        {
            ("Tribe Hunter", "TRIBE", 10f),
            ("Small Bug", "INSECTS", 8f),
            ("Medium Insect", "INSECTS", 9f),
            ("Tribe Warrior", "TRIBE", 12f),
            ("Archer", "HUMANOIDS", 10f),
            ("Guard", "HUMANOIDS", 12f),
            ("Warrior", "HUMANOIDS", 11f),
            ("Big Bug", "INSECTS", 14f),
            ("Spiny Beetle", "INSECTS", 11f),
            ("Relic Guardian", "BEASTS", 14f),
            ("Rusty Robot", "ROBOTS", 8f),
            ("Guard Robot", "ROBOTS", 11f),
            ("Circle Bot", "ROBOTS", 10f),
            ("Delivery Bot", "ROBOTS", 7f),
            ("Hooded Minion", "MINIONS", 10f),
            ("Bomb Minion", "MINIONS", 5f),
            ("Ranged Minion", "MINIONS", 8f),
        };

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

            var info = i < enemyInfo.Length ? enemyInfo[i] : ($"Enemy {i}", "OTHER", 10f);
            list.Add(new GalleryEntry
            {
                Name = info.Item1, Category = info.Item2,
                Sprite = eSprite, AnimationNames = anims.ToArray(),
                Radius = info.Item3, DrawScale = 1f,
            });
        }

        // Bosses
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
                Name = "Dust Warrior", Category = "BOSSES",
                Sprite = state.Assets.BossSprite,
                AnimationNames = bossAnims.ToArray(),
                Radius = 20f, DrawScale = 1.5f,
            });
        }

        if (state.Assets.BlowfishSprite != null)
        {
            var bfAnims = new List<string>();
            foreach (var name in new[] { "idle_right", "idle_up", "idle_down",
                "walk_right", "walk_up", "walk_down", "attack", "death" })
            {
                if (state.Assets.BlowfishSprite.HasAnimation(name)) bfAnims.Add(name);
            }
            list.Add(new GalleryEntry
            {
                Name = "Blowfish", Category = "BOSSES",
                Sprite = state.Assets.BlowfishSprite,
                AnimationNames = bfAnims.ToArray(),
                Radius = 20f, DrawScale = 1.5f,
            });
        }

        if (state.Assets.TarnishedWidowSprite != null)
        {
            var twAnims = new List<string>();
            foreach (var name in new[] { "idle_right", "idle_up", "idle_down",
                "walk_right", "walk_up", "walk_down", "attack", "death" })
            {
                if (state.Assets.TarnishedWidowSprite.HasAnimation(name)) twAnims.Add(name);
            }
            list.Add(new GalleryEntry
            {
                Name = "Tarnished Widow", Category = "BOSSES",
                Sprite = state.Assets.TarnishedWidowSprite,
                AnimationNames = twAnims.ToArray(),
                Radius = 25f, DrawScale = 1.5f,
            });
        }

        // Sort by category for grouped display
        string[] categoryOrder = { "HERO", "TRIBE", "INSECTS", "BEASTS", "HUMANOIDS", "ROBOTS", "MINIONS", "BOSSES" };
        list.Sort((a, b) =>
        {
            int ai = Array.IndexOf(categoryOrder, a.Category);
            int bi = Array.IndexOf(categoryOrder, b.Category);
            if (ai < 0) ai = 99;
            if (bi < 0) bi = 99;
            return ai != bi ? ai.CompareTo(bi) : string.Compare(a.Name, b.Name, StringComparison.Ordinal);
        });

        _entries = list.ToArray();
    }

    public void Reset()
    {
        _selectedEntry = 0;
        _selectedAnim = 0;
        _animTimer = 0;
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

        if (Raylib.IsKeyPressed(KeyboardKey.H))
            _showHitbox = !_showHitbox;

        _animTimer += dt;
    }

    private const int ListX = 8;
    private const int ListTop = 36;
    private const int RowH = 11;
    private const int CategoryH = 13;
    private const int PanelDivider = 170; // x position where right panel starts

    public void Draw(GameState state, GameStateManager manager)
    {
        InitEntries(state);
        Raylib.ClearBackground(new Color(30, 30, 35, 255));

        // Header
        UIRenderer.DrawTextMedium("SPRITE GALLERY", 10, 6, Color.Gold);
        UIRenderer.DrawTextSmall("Up/Down: select   Left/Right: anim   H: hitbox   ESC: back",
            10, 22, Color.Gray);

        if (_entries.Length == 0)
        {
            UIRenderer.DrawTextSmall("No STRANDED sprites loaded.", 10, 60, Color.Red);
            return;
        }

        // Divider line
        Raylib.DrawLine(PanelDivider - 4, ListTop, PanelDivider - 4, Constants.LogicalHeight, new Color(60, 60, 65, 255));

        DrawEntryList();
        DrawPreviewPanel();
    }

    private void DrawEntryList()
    {
        // Build display rows: category headers + entries
        // Figure out how many visual rows we have and which row is selected
        var rows = new List<(int entryIdx, string? categoryLabel)>(); // entryIdx=-1 for category headers
        string? lastCat = null;
        int selectedRow = 0;

        for (int i = 0; i < _entries.Length; i++)
        {
            if (_entries[i].Category != lastCat)
            {
                lastCat = _entries[i].Category;
                rows.Add((-1, lastCat));
            }
            if (i == _selectedEntry) selectedRow = rows.Count;
            rows.Add((i, null));
        }

        // Scrolling: keep selected row visible within the viewable window
        int maxVisibleRows = (Constants.LogicalHeight - ListTop - 8) / RowH;
        // Ensure selectedRow is visible with some padding
        if (selectedRow < _scrollOffset + 1)
            _scrollOffset = Math.Max(0, selectedRow - 1);
        else if (selectedRow >= _scrollOffset + maxVisibleRows - 1)
            _scrollOffset = selectedRow - maxVisibleRows + 2;
        _scrollOffset = Math.Clamp(_scrollOffset, 0, Math.Max(0, rows.Count - maxVisibleRows));

        int y = ListTop;
        for (int r = _scrollOffset; r < rows.Count && y < Constants.LogicalHeight - 4; r++)
        {
            var (entryIdx, categoryLabel) = rows[r];

            if (categoryLabel != null)
            {
                // Category header
                UIRenderer.DrawTextSmall(categoryLabel, ListX, y + 1, new Color(120, 180, 255, 255));
                Raylib.DrawLine(ListX, y + 10, PanelDivider - 10, y + 10, new Color(50, 70, 100, 255));
                y += CategoryH;
            }
            else
            {
                bool selected = entryIdx == _selectedEntry;
                Color c = selected ? Color.Gold : Color.LightGray;

                // Highlight bar
                if (selected)
                    Raylib.DrawRectangle(ListX - 2, y - 1, PanelDivider - ListX - 6, RowH, new Color(50, 50, 60, 255));

                string prefix = selected ? "> " : "  ";
                UIRenderer.DrawTextSmall($"{prefix}{_entries[entryIdx].Name}", ListX, y, c);
                y += RowH;
            }
        }

        // Scroll indicators
        if (_scrollOffset > 0)
            UIRenderer.DrawTextSmall("^", PanelDivider - 14, ListTop, Color.Gray);
        if (_scrollOffset + maxVisibleRows < rows.Count)
            UIRenderer.DrawTextSmall("v", PanelDivider - 14, Constants.LogicalHeight - 12, Color.Gray);
    }

    private void DrawPreviewPanel()
    {
        var entry = _entries[_selectedEntry];
        if (entry.Sprite == null || entry.AnimationNames.Length == 0) return;

        string animName = entry.AnimationNames[_selectedAnim];
        int rightX = PanelDivider + 4;
        int rightW = Constants.LogicalWidth - rightX - 4;
        int centerX = rightX + rightW / 2;

        // Sprite preview — centered in upper portion of right panel
        float previewY = ListTop + 80;
        float previewScale = entry.DrawScale * 3f;

        // Checkerboard background
        int bgSize = 90;
        int bgX = centerX - bgSize / 2;
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

        entry.Sprite.DrawAnimationFrame(animName, frame, false,
            centerX, previewY, Color.White, previewScale);

        // Hitbox overlay
        if (_showHitbox)
        {
            float hitR = entry.Radius * previewScale / entry.DrawScale;
            Raylib.DrawCircleLines(centerX, (int)previewY, hitR, new Color(0, 255, 0, 180));
            Raylib.DrawCircleV(new System.Numerics.Vector2(centerX, previewY), 2f, Color.Red);
            Raylib.DrawLineV(
                new System.Numerics.Vector2(centerX - 6, previewY),
                new System.Numerics.Vector2(centerX + 6, previewY), Color.Red);
            Raylib.DrawLineV(
                new System.Numerics.Vector2(centerX, previewY - 6),
                new System.Numerics.Vector2(centerX, previewY + 6), Color.Red);
        }

        // Entry name + category
        UIRenderer.DrawTextSmall(entry.Name, rightX, ListTop, Color.Gold);
        UIRenderer.DrawTextSmall(entry.Category, rightX + entry.Name.Length * 5 + 8, ListTop, new Color(120, 180, 255, 255));

        // Animation selector (below preview)
        int animY = (int)previewY + bgSize / 2 + 8;
        UIRenderer.DrawTextSmall("Animation:", rightX, animY, Color.White);

        // Show animations in a compact row, wrapping if needed
        int ax = rightX;
        int ay = animY + 12;
        for (int i = 0; i < entry.AnimationNames.Length; i++)
        {
            string aName = entry.AnimationNames[i];
            int tw = aName.Length * 5 + 6;

            // Wrap to next line if too wide
            if (ax + tw > Constants.LogicalWidth - 4)
            {
                ax = rightX;
                ay += 11;
            }

            Color c = i == _selectedAnim ? Color.Gold : Color.Gray;
            if (i == _selectedAnim)
                Raylib.DrawRectangle(ax - 1, ay - 1, tw, 10, new Color(50, 50, 60, 255));
            UIRenderer.DrawTextSmall(aName, ax, ay, c);
            ax += tw;
        }

        // Info block below animations
        int infoY = ay + 18;
        UIRenderer.DrawTextSmall($"Frames: {frameCount}   FPS: {(frameDur > 0 ? (int)(1f / frameDur) : 0)}", rightX, infoY, Color.White);
        UIRenderer.DrawTextSmall($"Size: {entry.Sprite.GetFrameWidth(animName)}x{entry.Sprite.GetFrameHeight(animName)}   " +
            $"Scale: {previewScale:F1}x", rightX, infoY + 11, Color.White);
        UIRenderer.DrawTextSmall($"Hitbox: {entry.Radius:F0}px", rightX, infoY + 22,
            _showHitbox ? Color.Green : Color.Gray);

        string hitboxText = _showHitbox ? "[H] ON" : "[H] OFF";
        UIRenderer.DrawTextSmall(hitboxText, rightX + 70, infoY + 22,
            _showHitbox ? Color.Green : Color.DarkGray);
    }
}
