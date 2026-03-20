using System.Numerics;
using Raylib_cs;

namespace CloneTato.Core;

public static class InputHelper
{
    private const float DeadZone = 0.2f;

    // SDL gamepad for macOS Xbox controller support
    private static SdlGamepad? _sdl;

    public static bool GamepadAvailable => (_sdl?.Connected ?? false) || Raylib.IsGamepadAvailable(0);
    public static string? GamepadName => _sdl?.Connected == true ? _sdl.Name : null;

    public static void InitGamepad()
    {
        try
        {
            _sdl = new SdlGamepad();
            if (_sdl.Connected)
                Console.WriteLine($"[InputHelper] SDL gamepad active: {_sdl.Name}");
            else
                Console.WriteLine("[InputHelper] SDL gamepad initialized, no controller found yet");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[InputHelper] SDL gamepad unavailable: {ex.Message}");
            _sdl = null;
        }
    }

    public static void UpdateGamepad()
    {
        _sdl?.Update();
    }

    public static void ShutdownGamepad()
    {
        _sdl?.Dispose();
        _sdl = null;
    }

    // Debug info for the main menu overlay
    public static (float lx, float ly, float rx, float ry, float lt, float rt) GetSdlAxes()
    {
        if (_sdl == null || !_sdl.Connected) return default;
        return (
            _sdl.GetAxis(SdlGamepad.AXIS_LEFTX),
            _sdl.GetAxis(SdlGamepad.AXIS_LEFTY),
            _sdl.GetAxis(SdlGamepad.AXIS_RIGHTX),
            _sdl.GetAxis(SdlGamepad.AXIS_RIGHTY),
            _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERLEFT),
            _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERRIGHT)
        );
    }

    public static string GetSdlButtonsDebug()
    {
        if (_sdl == null || !_sdl.Connected) return "";
        var pressed = new List<string>();
        string[] names = { "A", "B", "X", "Y", "Back", "Guide", "Start", "LS", "RS", "LB", "RB",
            "Up", "Down", "Left", "Right" };
        for (int i = 0; i <= 14; i++)
        {
            if (_sdl.IsButtonDown(i))
                pressed.Add(i < names.Length ? names[i] : $"B{i}");
        }
        return pressed.Count > 0 ? string.Join(" ", pressed) : "";
    }

    public static Vector2 GetMoveInput()
    {
        Vector2 input = Vector2.Zero;
        var settings = GameSettings.Current;

        // Keyboard
        if (Raylib.IsKeyDown(settings.GetKey(GameAction.MoveUp)) || Raylib.IsKeyDown(KeyboardKey.Up)) input.Y -= 1;
        if (Raylib.IsKeyDown(settings.GetKey(GameAction.MoveDown)) || Raylib.IsKeyDown(KeyboardKey.Down)) input.Y += 1;
        if (Raylib.IsKeyDown(settings.GetKey(GameAction.MoveLeft)) || Raylib.IsKeyDown(KeyboardKey.Left)) input.X -= 1;
        if (Raylib.IsKeyDown(settings.GetKey(GameAction.MoveRight)) || Raylib.IsKeyDown(KeyboardKey.Right)) input.X += 1;

        // SDL gamepad left stick
        if (_sdl?.Connected == true)
        {
            float gx = _sdl.GetAxis(SdlGamepad.AXIS_LEFTX);
            float gy = _sdl.GetAxis(SdlGamepad.AXIS_LEFTY);
            if (MathF.Abs(gx) > DeadZone || MathF.Abs(gy) > DeadZone)
            {
                input.X += gx;
                input.Y += gy;
            }
        }
        // Raylib fallback
        else if (Raylib.IsGamepadAvailable(0))
        {
            float gx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftX);
            float gy = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftY);
            if (MathF.Abs(gx) > DeadZone || MathF.Abs(gy) > DeadZone)
            {
                input.X += gx;
                input.Y += gy;
            }
        }

        if (input.LengthSquared() > 1f)
            input = Vector2.Normalize(input);
        else if (input.LengthSquared() > 0.001f && input.LengthSquared() < 1f)
        { } // keep analog magnitude
        else if (input.LengthSquared() > 0)
            input = Vector2.Normalize(input);

        return input;
    }

    public static Vector2 GetAimInput()
    {
        // SDL gamepad right stick
        if (_sdl?.Connected == true)
        {
            float gx = _sdl.GetAxis(SdlGamepad.AXIS_RIGHTX);
            float gy = _sdl.GetAxis(SdlGamepad.AXIS_RIGHTY);
            if (gx * gx + gy * gy > DeadZone * DeadZone)
                return new Vector2(gx, gy);
        }
        // Raylib fallback
        else if (Raylib.IsGamepadAvailable(0))
        {
            float gx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightX);
            float gy = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightY);
            if (gx * gx + gy * gy > DeadZone * DeadZone)
                return new Vector2(gx, gy);
        }

        return Vector2.Zero;
    }

    public static bool IsDashPressed()
    {
        var dashKey = GameSettings.Current.GetKey(GameAction.Dash);
        if (dashKey != KeyboardKey.Null && Raylib.IsKeyPressed(dashKey)) return true;
        // SDL: A button
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_A) == true) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftTrigger1)) return true;
        return false;
    }

    public static bool IsFireDown()
    {
        if (Raylib.IsMouseButtonDown(MouseButton.Left)) return true;
        // SDL: RT axis > threshold
        if (_sdl?.Connected == true && _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERRIGHT) > 0.3f) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonDown(0, GamepadButton.RightTrigger2)) return true;
        return false;
    }

    public static bool IsFirePressed()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) return true;
        // SDL: X button as single-shot fire
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_X) == true) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.RightTrigger2)) return true;
        return false;
    }

    public static bool IsConfirmPressed()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space)) return true;
        // SDL: A or Start
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_A) == true) return true;
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_START) == true) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.RightFaceDown)) return true;
        return false;
    }

    public static bool IsCancelPressed()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape)) return true;
        // SDL: B
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_B) == true) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.RightFaceRight)) return true;
        return false;
    }

    public static bool IsPausePressed()
    {
        var pauseKey = GameSettings.Current.GetKey(GameAction.Pause);
        if (pauseKey != KeyboardKey.Null && Raylib.IsKeyPressed(pauseKey)) return true;
        // SDL: Start or Back
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_START) == true) return true;
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_BACK) == true) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.MiddleRight)) return true;
        return false;
    }

    public static int GetMenuHorizontal()
    {
        int dir = 0;
        if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A)) dir--;
        if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D)) dir++;
        // SDL: D-pad
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_DPAD_LEFT) == true) dir--;
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_DPAD_RIGHT) == true) dir++;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0))
        {
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftFaceLeft)) dir--;
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftFaceRight)) dir++;
        }
        return dir;
    }

    public static int GetMenuVertical()
    {
        int dir = 0;
        if (Raylib.IsKeyPressed(KeyboardKey.Up) || Raylib.IsKeyPressed(KeyboardKey.W)) dir--;
        if (Raylib.IsKeyPressed(KeyboardKey.Down) || Raylib.IsKeyPressed(KeyboardKey.S)) dir++;
        // SDL: D-pad
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_DPAD_UP) == true) dir--;
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_DPAD_DOWN) == true) dir++;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0))
        {
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftFaceUp)) dir--;
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftFaceDown)) dir++;
        }
        return dir;
    }
}
