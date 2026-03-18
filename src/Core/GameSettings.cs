using System.Text.Json;
using System.Text.Json.Serialization;
using Raylib_cs;

namespace CloneTato.Core;

public enum GameAction
{
    MoveUp,
    MoveDown,
    MoveLeft,
    MoveRight,
    Dash,
    Pause,
}

public class GameSettings
{
    private const string SettingsFile = "settings.json";

    public static GameSettings Current { get; private set; } = new();

    public float MasterVolume { get; set; } = 1.0f;
    public float SFXVolume { get; set; } = 1.0f;
    public bool Fullscreen { get; set; }

    public Dictionary<string, string> KeyBindingData { get; set; } = new();

    [JsonIgnore]
    public Dictionary<GameAction, KeyboardKey> KeyBindings { get; set; } = DefaultKeyBindings();

    public static Dictionary<GameAction, KeyboardKey> DefaultKeyBindings() => new()
    {
        { GameAction.MoveUp, KeyboardKey.W },
        { GameAction.MoveDown, KeyboardKey.S },
        { GameAction.MoveLeft, KeyboardKey.A },
        { GameAction.MoveRight, KeyboardKey.D },
        { GameAction.Dash, KeyboardKey.Space },
        { GameAction.Pause, KeyboardKey.Escape },
    };

    public void Save()
    {
        KeyBindingData.Clear();
        foreach (var kvp in KeyBindings)
            KeyBindingData[kvp.Key.ToString()] = kvp.Value.ToString();

        var options = new JsonSerializerOptions { WriteIndented = true };
        try { File.WriteAllText(SettingsFile, JsonSerializer.Serialize(this, options)); }
        catch { }
    }

    public static GameSettings Load()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = File.ReadAllText(SettingsFile);
                var settings = JsonSerializer.Deserialize<GameSettings>(json);
                if (settings != null)
                {
                    settings.SyncKeyBindings();
                    Current = settings;
                    return Current;
                }
            }
        }
        catch { }

        Current = new GameSettings();
        return Current;
    }

    private void SyncKeyBindings()
    {
        var defaults = DefaultKeyBindings();
        KeyBindings = new Dictionary<GameAction, KeyboardKey>(defaults);

        foreach (var kvp in KeyBindingData)
        {
            if (Enum.TryParse<GameAction>(kvp.Key, out var action) &&
                Enum.TryParse<KeyboardKey>(kvp.Value, out var key))
                KeyBindings[action] = key;
        }
    }

    public void Apply()
    {
        Raylib.SetMasterVolume(MasterVolume);
    }

    public KeyboardKey GetKey(GameAction action)
    {
        return KeyBindings.GetValueOrDefault(action, KeyboardKey.Null);
    }

    public string GetKeyName(GameAction action)
    {
        var key = GetKey(action);
        return key switch
        {
            KeyboardKey.Null => "UNBOUND",
            KeyboardKey.Space => "SPACE",
            KeyboardKey.Escape => "ESC",
            KeyboardKey.Enter => "ENTER",
            KeyboardKey.Tab => "TAB",
            KeyboardKey.LeftShift => "L-SHIFT",
            KeyboardKey.RightShift => "R-SHIFT",
            KeyboardKey.LeftControl => "L-CTRL",
            KeyboardKey.RightControl => "R-CTRL",
            KeyboardKey.LeftAlt => "L-ALT",
            KeyboardKey.RightAlt => "R-ALT",
            KeyboardKey.Up => "UP",
            KeyboardKey.Down => "DOWN",
            KeyboardKey.Left => "LEFT",
            KeyboardKey.Right => "RIGHT",
            KeyboardKey.Backspace => "BACKSPACE",
            KeyboardKey.Delete => "DELETE",
            _ => key.ToString().ToUpper(),
        };
    }
}
