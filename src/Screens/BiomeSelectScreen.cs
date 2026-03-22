using CloneTato.Core;
using CloneTato.Data;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class BiomeSelectScreen
{
    private int _selectedIndex;

    private static readonly BiomeInfo[] Biomes =
    {
        new("The Waste", "Scorched dunes crawling with bugs and rusted machines.",
            new Color(180, 140, 80, 255), new Color(90, 70, 40, 255)),
        new("Blood Desert", "Tribal warriors and archers guard the red sands.",
            new Color(160, 60, 50, 255), new Color(80, 30, 25, 255)),
        new("The Temple", "Hooded minions and bots patrol the ancient halls.",
            new Color(70, 50, 100, 255), new Color(35, 25, 50, 255)),
    };

    public static string GetBiomeName(int biomeNumber)
    {
        int idx = biomeNumber - 1;
        if (idx >= 0 && idx < Biomes.Length)
            return Biomes[idx].Name;
        return $"Biome {biomeNumber}";
    }

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        int hDir = InputHelper.GetMenuHorizontal();
        if (hDir != 0)
            _selectedIndex = Math.Clamp(_selectedIndex + hDir, 0, Constants.BiomeCount - 1);

        if (InputHelper.IsConfirmPressed())
            StartBiomeRun(state, manager);
        if (InputHelper.IsCancelPressed())
            manager.TransitionTo(GameScreen.CharacterSelect);
    }

    private void StartBiomeRun(GameState state, GameStateManager manager)
    {
        int biome = _selectedIndex + 1; // 1-indexed
        if (!manager.Meta.BiomeUnlocked[_selectedIndex]) return;

        var character = CharacterDatabase.Characters[manager.CharSelect.SelectedCharacterIndex];
        state.StartNewRun(character);
        state.CurrentBiome = biome;
        state.MetaBonus = manager.Meta.GetMetaBonus();
        state.RecomputePlayerStats();
        state.StartWave();
        manager.TransitionTo(GameScreen.Playing);
        state.Assets.PlaySoundVariant("select", 0.5f);
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.ClearBackground(new Color(25, 20, 18, 255));

        string title = "SELECT BIOME";
        int titleW = Raylib.MeasureText(title, 18);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 20, 18, Color.Gold);

        // Character name reminder
        var ch = CharacterDatabase.Characters[manager.CharSelect.SelectedCharacterIndex];
        string charLabel = $"Playing as: {ch.Name}";
        int charW = Raylib.MeasureText(charLabel, 10);
        Raylib.DrawText(charLabel, Constants.LogicalWidth / 2 - charW / 2, 42, 10, Color.LightGray);

        // Biome cards
        int cardW = 180, cardH = 200;
        int gap = 14;
        int totalW = Constants.BiomeCount * cardW + (Constants.BiomeCount - 1) * gap;
        int startX = Constants.LogicalWidth / 2 - totalW / 2;
        int cardY = 60;

        var mouse = Display.ScreenToLogical(Raylib.GetMousePosition());

        for (int i = 0; i < Constants.BiomeCount; i++)
        {
            var biome = Biomes[i];
            int cx = startX + i * (cardW + gap);
            bool selected = i == _selectedIndex;
            bool unlocked = manager.Meta.BiomeUnlocked.Length > i && manager.Meta.BiomeUnlocked[i];

            // Card background
            Color cardBg = !unlocked ? new Color(30, 25, 20, 255) :
                selected ? biome.CardColor : biome.CardColorDark;
            Raylib.DrawRectangle(cx, cardY, cardW, cardH, cardBg);

            // Border
            Color borderColor = !unlocked ? new Color(60, 50, 40, 255) :
                selected ? Color.Gold : Color.Gray;
            Raylib.DrawRectangleLines(cx, cardY, cardW, cardH, borderColor);

            if (unlocked)
            {
                // Biome number
                string biomeNum = $"BIOME {i + 1}";
                int numW = Raylib.MeasureText(biomeNum, 10);
                Raylib.DrawText(biomeNum, cx + cardW / 2 - numW / 2, cardY + 10, 10,
                    new Color(200, 180, 140, 255));

                // Name
                int nameW = Raylib.MeasureText(biome.Name, 14);
                Raylib.DrawText(biome.Name, cx + cardW / 2 - nameW / 2, cardY + 26, 14, Color.White);

                // Description
                UIRenderer.DrawTextSmall(biome.Description, cx + 8, cardY + 50, Color.LightGray);

                // Enemy roster
                string[] enemies = GetBiomeEnemies(i + 1);
                int ey = cardY + 80;
                UIRenderer.DrawTextSmall("Enemies:", cx + 8, ey, Color.Orange);
                ey += 14;
                foreach (var enemy in enemies)
                {
                    UIRenderer.DrawTextSmall($"  {enemy}", cx + 8, ey, new Color(200, 190, 170, 255));
                    ey += 12;
                }

                // Boss name
                string boss = GetBiomeBoss(i + 1);
                UIRenderer.DrawTextSmall($"Boss: {boss}", cx + 8, cardY + cardH - 24, Color.Red);

                // Waves info
                UIRenderer.DrawTextSmall($"{Constants.WavesPerBiome} waves", cx + 8, cardY + cardH - 12,
                    new Color(150, 150, 150, 255));
            }
            else
            {
                // Locked
                int lockW = Raylib.MeasureText("LOCKED", 14);
                Raylib.DrawText("LOCKED", cx + cardW / 2 - lockW / 2, cardY + 70, 14, Color.Gray);

                string hint = i switch
                {
                    1 => "Beat The Waste",
                    2 => "Beat Blood Desert",
                    _ => "???",
                };
                int hintW = Raylib.MeasureText(hint, 10);
                Raylib.DrawText(hint, cx + cardW / 2 - hintW / 2, cardY + 100, 10,
                    new Color(120, 100, 80, 255));
            }

            // Click to select or confirm
            if (mouse.X >= cx && mouse.X <= cx + cardW && mouse.Y >= cardY && mouse.Y <= cardY + cardH)
            {
                if (Raylib.IsMouseButtonPressed(MouseButton.Left))
                {
                    if (_selectedIndex == i)
                        StartBiomeRun(state, manager);
                    else
                        _selectedIndex = i;
                }
            }
        }

        // Start button
        int btnW = 100, btnH = 22;
        if (UIRenderer.DrawButton("START", Constants.LogicalWidth / 2 - btnW / 2,
            Constants.LogicalHeight - 50, btnW, btnH, new Color(60, 100, 60, 255)))
        {
            StartBiomeRun(state, manager);
        }

        // Back button
        if (UIRenderer.DrawButton("BACK", Constants.LogicalWidth / 2 - btnW / 2,
            Constants.LogicalHeight - 24, btnW, btnH, new Color(100, 60, 60, 255)))
        {
            manager.TransitionTo(GameScreen.CharacterSelect);
        }

        string controlHint = InputHelper.GamepadAvailable
            ? "D-Pad select, A start, B back"
            : "Arrow keys select, Enter start, Esc back";
        UIRenderer.DrawTextSmall(controlHint,
            Constants.LogicalWidth / 2 - controlHint.Length * 5 / 2, Constants.LogicalHeight - 8, Color.Gray);
    }

    private static string[] GetBiomeEnemies(int biome) => biome switch
    {
        1 => ["Small Bug", "Medium Insect", "Rusty Robot", "Delivery Bot", "Spiny Beetle", "Big Bug"],
        2 => ["Tribe Warrior", "Archer", "Guard", "Warrior", "Relic Guardian", "Spiny Beetle"],
        3 => ["Hooded Minion", "Circle Bot", "Ranged Minion", "Guard Robot", "Bomb Minion", "Planter Bot"],
        _ => ["???"],
    };

    private static string GetBiomeBoss(int biome) => biome switch
    {
        1 => "Dust Warrior",
        2 => "Blowfish",
        3 => "Tarnished Widow",
        _ => "???",
    };

    private record BiomeInfo(string Name, string Description, Color CardColor, Color CardColorDark);
}
