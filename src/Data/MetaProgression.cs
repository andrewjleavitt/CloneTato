using System.Text.Json;
using System.Text.Json.Serialization;

namespace CloneTato.Data;

public class MetaProgression
{
    // Persistent currency
    public int Tokens { get; set; }

    // Lifetime stats
    public int TotalRuns { get; set; }
    public int TotalKills { get; set; }
    public int BestWave { get; set; }
    public int Victories { get; set; }

    // Character unlocks (index into CharacterDatabase)
    public bool[] CharacterUnlocked { get; set; } = { true, false, false };

    // Stat upgrades (each level gives a tiny boost)
    public int MaxHPLevel { get; set; }        // +0.5 per level
    public int MoveSpeedLevel { get; set; }    // +0.3 per level
    public int DamageLevel { get; set; }       // +0.01x per level
    public int ArmorLevel { get; set; }        // +0.1 per level (rounds down)
    public int CritLevel { get; set; }         // +0.005 per level
    public int DodgeLevel { get; set; }        // +0.003 per level
    public int PickupLevel { get; set; }       // +0.5 per level
    public int XPGainLevel { get; set; }       // +0.01x per level

    public const int MaxUpgradeLevel = 50;

    public Stats GetMetaBonus()
    {
        return new Stats
        {
            MaxHP = (int)(MaxHPLevel * 0.5f),
            MoveSpeed = MoveSpeedLevel * 0.3f,
            DamageMultiplier = DamageLevel * 0.01f,
            Armor = (int)(ArmorLevel * 0.1f),
            CritChance = CritLevel * 0.005f,
            DodgeChance = DodgeLevel * 0.003f,
            PickupRange = PickupLevel * 0.5f,
            XPMultiplier = XPGainLevel * 0.01f,
        };
    }

    public static readonly UpgradeDef[] Upgrades =
    {
        new("Max HP", "Vitality", "+0.5 HP per level", 3),
        new("Move Speed", "Agility", "+0.3 speed per level", 3),
        new("Damage", "Power", "+1% damage per level", 4),
        new("Armor", "Toughness", "+0.1 armor per level", 5),
        new("Crit Chance", "Precision", "+0.5% crit per level", 4),
        new("Dodge", "Reflexes", "+0.3% dodge per level", 5),
        new("Pickup Range", "Magnetism", "+0.5 range per level", 3),
        new("XP Gain", "Wisdom", "+1% XP per level", 3),
    };

    public int GetLevel(int upgradeIndex) => upgradeIndex switch
    {
        0 => MaxHPLevel, 1 => MoveSpeedLevel, 2 => DamageLevel, 3 => ArmorLevel,
        4 => CritLevel, 5 => DodgeLevel, 6 => PickupLevel, 7 => XPGainLevel,
        _ => 0,
    };

    public void AddLevel(int upgradeIndex)
    {
        switch (upgradeIndex)
        {
            case 0: MaxHPLevel++; break;
            case 1: MoveSpeedLevel++; break;
            case 2: DamageLevel++; break;
            case 3: ArmorLevel++; break;
            case 4: CritLevel++; break;
            case 5: DodgeLevel++; break;
            case 6: PickupLevel++; break;
            case 7: XPGainLevel++; break;
        }
    }

    public int GetUpgradeCost(int upgradeIndex)
    {
        int level = GetLevel(upgradeIndex);
        int baseCost = Upgrades[upgradeIndex].BaseCost;
        return baseCost + level * 2;
    }

    public int CalculateRunTokens(int waveReached, int enemiesKilled, bool victory)
    {
        int tokens = waveReached * 2 + enemiesKilled / 10;
        if (victory) tokens += 20;
        return tokens;
    }

    public void CheckUnlocks()
    {
        // Ensure array is correct size for current character count
        if (CharacterUnlocked.Length < CharacterDatabase.Characters.Length)
        {
            var old = CharacterUnlocked;
            CharacterUnlocked = new bool[CharacterDatabase.Characters.Length];
            Array.Copy(old, CharacterUnlocked, Math.Min(old.Length, CharacterUnlocked.Length));
            CharacterUnlocked[0] = true; // first character always unlocked
        }

        // Character 1 (Blade Dancer): beat The Waste (biome 1 = 10 waves)
        if (BestWave >= Constants.WavesPerBiome && CharacterUnlocked.Length > 1) CharacterUnlocked[1] = true;
        // Character 2 (Drifter): beat Blood Desert (biome 2 = 20 total waves)
        if (BestWave >= Constants.WavesPerBiome * 2 && CharacterUnlocked.Length > 2) CharacterUnlocked[2] = true;
    }

    // Save/Load
    private static readonly string SavePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Drift", "save.json");

    public void Save()
    {
        var dir = Path.GetDirectoryName(SavePath)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SavePath, json);
    }

    public static MetaProgression Load()
    {
        if (!File.Exists(SavePath)) return new MetaProgression();
        try
        {
            var json = File.ReadAllText(SavePath);
            return JsonSerializer.Deserialize<MetaProgression>(json) ?? new MetaProgression();
        }
        catch
        {
            return new MetaProgression();
        }
    }
}

public record UpgradeDef(string Name, string StatName, string Description, int BaseCost);
