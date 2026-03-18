using CloneTato.Core;
using CloneTato.UI;
using Raylib_cs;

namespace CloneTato.Screens;

public class SettingsScreen
{
    private int _currentTab;
    private int _selectedItem;
    private GameAction? _rebindingAction;
    private bool _sliderDirty;

    private const int TabCount = 3;
    private static readonly string[] TabNames = { "AUDIO", "DISPLAY", "CONTROLS" };
    private static readonly GameAction[] BindableActions =
    {
        GameAction.MoveUp, GameAction.MoveDown,
        GameAction.MoveLeft, GameAction.MoveRight,
        GameAction.Dash, GameAction.Pause,
    };

    private static readonly string[] ActionNames =
    {
        "Move Up", "Move Down", "Move Left", "Move Right", "Dash", "Pause",
    };

    private static readonly string[] GamepadLabels =
    {
        "L-Stick Up", "L-Stick Down", "L-Stick Left", "L-Stick Right", "LB", "Start",
    };

    public void Reset()
    {
        _selectedItem = 0;
        _rebindingAction = null;
    }

    private int ItemCount => _currentTab switch
    {
        0 => 2,
        1 => 1,
        2 => BindableActions.Length + 1,
        _ => 0,
    };

    public void Update(float dt, GameState state, GameStateManager manager)
    {
        var settings = GameSettings.Current;

        // Key rebinding capture
        if (_rebindingAction.HasValue)
        {
            if (InputHelper.GamepadAvailable &&
                Raylib.IsGamepadButtonPressed(0, GamepadButton.RightFaceRight))
            {
                _rebindingAction = null;
                return;
            }

            int keyCode = Raylib.GetKeyPressed();
            if (keyCode != 0)
            {
                var newKey = (KeyboardKey)keyCode;
                if (newKey == KeyboardKey.Escape)
                {
                    _rebindingAction = null;
                }
                else
                {
                    // Swap conflicting binding
                    var oldKey = settings.KeyBindings[_rebindingAction.Value];
                    foreach (var action in settings.KeyBindings.Keys.ToList())
                    {
                        if (action != _rebindingAction.Value && settings.KeyBindings[action] == newKey)
                        {
                            settings.KeyBindings[action] = oldKey;
                            break;
                        }
                    }
                    settings.KeyBindings[_rebindingAction.Value] = newKey;
                    settings.Save();
                    _rebindingAction = null;
                }
            }
            return;
        }

        // Tab navigation (LB/RB or Q/E)
        int tabDir = 0;
        if (Raylib.IsKeyPressed(KeyboardKey.Q)) tabDir--;
        if (Raylib.IsKeyPressed(KeyboardKey.E)) tabDir++;
        if (InputHelper.GamepadAvailable)
        {
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftTrigger1)) tabDir--;
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.RightTrigger1)) tabDir++;
        }
        if (tabDir != 0)
        {
            _currentTab = (_currentTab + tabDir + TabCount) % TabCount;
            _selectedItem = 0;
        }

        // Item navigation
        int vDir = InputHelper.GetMenuVertical();
        if (vDir != 0 && ItemCount > 0)
            _selectedItem = (_selectedItem + vDir + ItemCount) % ItemCount;

        // Horizontal adjustment
        int hDir = InputHelper.GetMenuHorizontal();

        switch (_currentTab)
        {
            case 0: // Audio
                if (hDir != 0)
                {
                    if (_selectedItem == 0)
                    {
                        settings.MasterVolume = Math.Clamp(settings.MasterVolume + hDir * 0.1f, 0f, 1f);
                        settings.Apply();
                    }
                    else if (_selectedItem == 1)
                    {
                        settings.SFXVolume = Math.Clamp(settings.SFXVolume + hDir * 0.1f, 0f, 1f);
                    }
                    settings.Save();
                }
                break;

            case 1: // Display
                if ((hDir != 0 || InputHelper.IsConfirmPressed()) && _selectedItem == 0)
                {
                    settings.Fullscreen = !settings.Fullscreen;
                    Raylib.ToggleBorderlessWindowed();
                    settings.Save();
                }
                break;

            case 2: // Controls
                if (InputHelper.IsConfirmPressed())
                {
                    if (_selectedItem < BindableActions.Length)
                        _rebindingAction = BindableActions[_selectedItem];
                    else
                        ResetDefaults(settings);
                }
                break;
        }

        // Back
        if (InputHelper.IsCancelPressed())
        {
            settings.Save();
            manager.CloseSettings();
        }
    }

    private static void ResetDefaults(GameSettings settings)
    {
        settings.KeyBindings = GameSettings.DefaultKeyBindings();
        settings.Save();
    }

    public void Draw(GameState state, GameStateManager manager)
    {
        Raylib.DrawRectangle(0, 0, Constants.LogicalWidth, Constants.LogicalHeight, new Color(0, 0, 0, 180));

        string title = "SETTINGS";
        int titleW = Raylib.MeasureText(title, 20);
        Raylib.DrawText(title, Constants.LogicalWidth / 2 - titleW / 2, 30, 20, Color.White);

        var mouse = Display.ScreenToLogical(Raylib.GetMousePosition());

        // Tabs
        int tabW = 90, tabH = 18;
        int tabTotalW = TabCount * (tabW + 6) - 6;
        int tabStartX = Constants.LogicalWidth / 2 - tabTotalW / 2;
        int tabY = 58;

        for (int i = 0; i < TabCount; i++)
        {
            int tx = tabStartX + i * (tabW + 6);
            bool active = i == _currentTab;
            bool tabHovered = mouse.X >= tx && mouse.X <= tx + tabW && mouse.Y >= tabY && mouse.Y <= tabY + tabH;

            if (tabHovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
            {
                _currentTab = i;
                _selectedItem = 0;
                active = true;
            }

            Color bg = active ? new Color(80, 70, 50, 255) : tabHovered ? new Color(55, 48, 38, 255) : new Color(40, 35, 30, 255);
            Raylib.DrawRectangle(tx, tabY, tabW, tabH, bg);
            Raylib.DrawRectangleLines(tx, tabY, tabW, tabH, active ? Color.Gold : Color.Gray);
            int textW = TabNames[i].Length * 5;
            UIRenderer.DrawTextSmall(TabNames[i], tx + tabW / 2 - textW / 2, tabY + 5, active ? Color.White : Color.Gray);
        }

        // Content
        int contentY = tabY + tabH + 16;
        int contentX = Constants.LogicalWidth / 2 - 150;

        switch (_currentTab)
        {
            case 0: DrawAudioTab(contentX, contentY, mouse); break;
            case 1: DrawDisplayTab(contentX, contentY, mouse); break;
            case 2: DrawControlsTab(contentX, contentY, mouse); break;
        }

        // Save on mouse release after slider drag
        if (_sliderDirty && Raylib.IsMouseButtonReleased(MouseButton.Left))
        {
            GameSettings.Current.Save();
            _sliderDirty = false;
        }

        // Hints
        string tabHint = InputHelper.GamepadAvailable ? "LB/RB = tab" : "Q/E = tab";
        UIRenderer.DrawTextSmall(tabHint, 8, Constants.LogicalHeight - 20, Color.Gray);
        string backHint = InputHelper.GamepadAvailable ? "B = back" : "ESC = back";
        UIRenderer.DrawTextSmall(backHint, 8, Constants.LogicalHeight - 10, Color.Gray);
    }

    private void DrawAudioTab(int x, int y, System.Numerics.Vector2 mouse)
    {
        var settings = GameSettings.Current;

        float master = settings.MasterVolume;
        DrawSliderRow("Master Volume", ref master, x, y, _selectedItem == 0, mouse);
        if (master != settings.MasterVolume) { settings.MasterVolume = master; settings.Apply(); _sliderDirty = true; }

        float sfx = settings.SFXVolume;
        DrawSliderRow("SFX Volume", ref sfx, x, y + 24, _selectedItem == 1, mouse);
        if (sfx != settings.SFXVolume) { settings.SFXVolume = sfx; _sliderDirty = true; }

        UIRenderer.DrawTextSmall("Left/Right to adjust", x, y + 56, Color.Gray);
    }

    private void DrawDisplayTab(int x, int y, System.Numerics.Vector2 mouse)
    {
        var settings = GameSettings.Current;
        bool selected = _selectedItem == 0;

        int rowY = y;
        bool rowHovered = mouse.Y >= rowY && mouse.Y < rowY + 16 && mouse.X >= x && mouse.X < x + 200;
        if (rowHovered) _selectedItem = 0;

        UIRenderer.DrawTextSmall("Fullscreen", x, rowY + 2, selected ? Color.White : Color.LightGray);

        int toggleX = x + 130, toggleW = 50, toggleH = 14;
        if (selected)
        {
            Raylib.DrawRectangle(toggleX, rowY, toggleW, toggleH, new Color(50, 45, 35, 255));
            Raylib.DrawRectangleLines(toggleX, rowY, toggleW, toggleH, Color.Gold);
        }

        string val = settings.Fullscreen ? "ON" : "OFF";
        Color valColor = settings.Fullscreen ? Color.Green : Color.Red;
        int valW = val.Length * 5;
        UIRenderer.DrawTextSmall(val, toggleX + toggleW / 2 - valW / 2, rowY + 3, valColor);

        if (rowHovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
        {
            settings.Fullscreen = !settings.Fullscreen;
            Raylib.ToggleBorderlessWindowed();
            settings.Save();
        }

        UIRenderer.DrawTextSmall("Enter or Left/Right to toggle", x, y + 24, Color.Gray);
    }

    private void DrawControlsTab(int x, int y, System.Numerics.Vector2 mouse)
    {
        var settings = GameSettings.Current;

        UIRenderer.DrawTextSmall("Keyboard", x + 100, y - 12, Color.Gray);
        UIRenderer.DrawTextSmall("Gamepad", x + 190, y - 12, Color.Gray);

        for (int i = 0; i < BindableActions.Length; i++)
        {
            var action = BindableActions[i];
            bool selected = _selectedItem == i;
            bool rebinding = _rebindingAction == action;
            int rowY = y + i * 18;

            bool rowHovered = mouse.Y >= rowY && mouse.Y < rowY + 16 && mouse.X >= x && mouse.X < x + 240;
            if (rowHovered && !_rebindingAction.HasValue)
                _selectedItem = i;

            Color nameColor = selected ? Color.White : Color.LightGray;
            UIRenderer.DrawTextSmall(ActionNames[i], x, rowY + 2, nameColor);

            if (rebinding)
            {
                Raylib.DrawRectangle(x + 96, rowY, 86, 14, new Color(80, 40, 40, 255));
                Raylib.DrawRectangleLines(x + 96, rowY, 86, 14, Color.Red);
                UIRenderer.DrawTextSmall("PRESS A KEY...", x + 100, rowY + 3, Color.Yellow);
            }
            else
            {
                string keyName = settings.GetKeyName(action);
                if (selected)
                {
                    Raylib.DrawRectangle(x + 96, rowY, 86, 14, new Color(50, 45, 35, 255));
                    Raylib.DrawRectangleLines(x + 96, rowY, 86, 14, Color.Gold);
                }
                UIRenderer.DrawTextSmall(keyName, x + 100, rowY + 3, selected ? Color.Gold : Color.Gray);

                if (rowHovered && Raylib.IsMouseButtonPressed(MouseButton.Left) && !_rebindingAction.HasValue)
                    _rebindingAction = action;
            }

            UIRenderer.DrawTextSmall(GamepadLabels[i], x + 190, rowY + 2, new Color(100, 100, 100, 255));
        }

        // Reset defaults button
        int resetIdx = BindableActions.Length;
        bool resetSelected = _selectedItem == resetIdx;
        int resetY = y + BindableActions.Length * 18 + 10;

        bool resetHovered = mouse.Y >= resetY && mouse.Y < resetY + 16 && mouse.X >= x && mouse.X < x + 110;
        if (resetHovered && !_rebindingAction.HasValue)
            _selectedItem = resetIdx;

        Color resetBg = resetSelected ? new Color(80, 50, 40, 255) : new Color(50, 35, 30, 255);
        Raylib.DrawRectangle(x, resetY, 110, 16, resetBg);
        Raylib.DrawRectangleLines(x, resetY, 110, 16, resetSelected ? Color.Gold : Color.Gray);
        UIRenderer.DrawTextSmall("RESET DEFAULTS", x + 10, resetY + 4, resetSelected ? Color.White : Color.Gray);

        if (resetHovered && Raylib.IsMouseButtonPressed(MouseButton.Left))
            ResetDefaults(settings);
    }

    private static void DrawSliderRow(string label, ref float value, int x, int y, bool selected,
        System.Numerics.Vector2 mouse)
    {
        int barX = x + 100, barW = 80, barH = 8;
        int barY = y + 2;

        bool barHovered = mouse.X >= barX && mouse.X <= barX + barW &&
                          mouse.Y >= barY - 4 && mouse.Y <= barY + barH + 4;

        // Mouse drag on slider
        if (barHovered && Raylib.IsMouseButtonDown(MouseButton.Left))
            value = Math.Clamp((mouse.X - barX) / barW, 0f, 1f);

        UIRenderer.DrawTextSmall(label, x, y + 2, selected ? Color.White : Color.LightGray);

        if (selected)
            Raylib.DrawRectangleLines(barX - 2, barY - 2, barW + 4, barH + 4, Color.Gold);

        Raylib.DrawRectangle(barX, barY, barW, barH, new Color(30, 25, 20, 255));
        Raylib.DrawRectangle(barX, barY, (int)(barW * value), barH, Color.Gold);
        Raylib.DrawRectangleLines(barX, barY, barW, barH, Color.Gray);

        string pctText = $"{(int)(value * 100)}%";
        UIRenderer.DrawTextSmall(pctText, barX + barW + 6, y + 2, selected ? Color.White : Color.Gray);
    }
}
