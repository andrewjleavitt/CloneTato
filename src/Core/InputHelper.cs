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

        // SDL gamepad left stick (radial deadzone with rescaling)
        if (_sdl?.Connected == true)
        {
            float gx = _sdl.GetAxis(SdlGamepad.AXIS_LEFTX);
            float gy = _sdl.GetAxis(SdlGamepad.AXIS_LEFTY);
            var stick = ApplyRadialDeadzone(gx, gy);
            input.X += stick.X;
            input.Y += stick.Y;
        }
        // Raylib fallback
        else if (Raylib.IsGamepadAvailable(0))
        {
            float gx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftX);
            float gy = Raylib.GetGamepadAxisMovement(0, GamepadAxis.LeftY);
            var stick = ApplyRadialDeadzone(gx, gy);
            input.X += stick.X;
            input.Y += stick.Y;
        }

        if (input.LengthSquared() > 1f)
            input = Vector2.Normalize(input);

        return input;
    }

    public static Vector2 GetAimInput()
    {
        // SDL gamepad right stick (radial deadzone with rescaling)
        if (_sdl?.Connected == true)
        {
            float gx = _sdl.GetAxis(SdlGamepad.AXIS_RIGHTX);
            float gy = _sdl.GetAxis(SdlGamepad.AXIS_RIGHTY);
            return ApplyRadialDeadzone(gx, gy);
        }
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0))
        {
            float gx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightX);
            float gy = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightY);
            return ApplyRadialDeadzone(gx, gy);
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
        // SDL: RT axis rising edge (trigger just crossed threshold)
        if (_sdl?.Connected == true && _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERRIGHT) > 0.3f && !_prevRT) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.RightTrigger2)) return true;
        return false;
    }

    // Secondary weapon: right mouse / LT trigger (rising edge)
    public static bool IsSecondaryFirePressed()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Right)) return true;
        // SDL: LT axis crosses threshold (rising edge via state tracking)
        if (_sdl?.Connected == true && _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERLEFT) > 0.3f && !_prevLT) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftTrigger2)) return true;
        return false;
    }

    // Secondary weapon held: right mouse / LT trigger (sustained)
    public static bool IsSecondaryFireDown()
    {
        if (Raylib.IsMouseButtonDown(MouseButton.Right)) return true;
        // SDL: LT axis above threshold
        if (_sdl?.Connected == true && _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERLEFT) > 0.3f) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonDown(0, GamepadButton.LeftTrigger2)) return true;
        return false;
    }

    // Special ability: Q key / LB button
    public static bool IsSpecialPressed()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Q)) return true;
        // SDL: LB button
        if (_sdl?.IsButtonPressed(SdlGamepad.BUTTON_LEFTSHOULDER) == true) return true;
        // Raylib fallback
        if (Raylib.IsGamepadAvailable(0) && Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftTrigger1)) return true;
        return false;
    }

    // Track trigger previous states for rising-edge detection
    private static bool _prevRT;
    private static bool _prevLT;
    /// <summary>
    /// Must be called BEFORE UpdateGamepad() each frame to capture previous trigger states.
    /// </summary>
    public static void UpdateInputState()
    {
        _prevRT = _sdl?.Connected == true && _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERRIGHT) > 0.3f;
        _prevLT = _sdl?.Connected == true && _sdl.GetAxis(SdlGamepad.AXIS_TRIGGERLEFT) > 0.3f;
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

    /// <summary>
    /// Radial deadzone with rescaling. Inside the deadzone = zero.
    /// Outside, output ramps smoothly from 0 at the edge to 1.0 at full tilt.
    /// Prevents the input jump that per-axis deadzones cause.
    /// </summary>
    private static Vector2 ApplyRadialDeadzone(float x, float y)
    {
        float mag = MathF.Sqrt(x * x + y * y);
        if (mag < DeadZone) return Vector2.Zero;
        // Rescale so output is 0..1 across the usable range
        float rescaled = (mag - DeadZone) / (1f - DeadZone);
        if (rescaled > 1f) rescaled = 1f;
        return new Vector2(x / mag * rescaled, y / mag * rescaled);
    }
}
