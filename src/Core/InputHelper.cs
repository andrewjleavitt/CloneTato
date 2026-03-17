using System.Numerics;
using Raylib_cs;

namespace CloneTato.Core;

public static class InputHelper
{
    private const float DeadZone = 0.2f;

    public static bool GamepadAvailable => Raylib.IsGamepadAvailable(0);

    public static Vector2 GetMoveInput()
    {
        Vector2 input = Vector2.Zero;

        // Keyboard
        if (Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up)) input.Y -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down)) input.Y += 1;
        if (Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left)) input.X -= 1;
        if (Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right)) input.X += 1;

        // Gamepad left stick
        if (GamepadAvailable)
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
        if (!GamepadAvailable) return Vector2.Zero;

        float gx = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightX);
        float gy = Raylib.GetGamepadAxisMovement(0, GamepadAxis.RightY);
        if (gx * gx + gy * gy > DeadZone * DeadZone)
            return new Vector2(gx, gy);
        return Vector2.Zero;
    }

    public static bool IsDashPressed()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Space)) return true;
        if (GamepadAvailable && Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftTrigger1)) return true;
        return false;
    }

    public static bool IsFireDown()
    {
        if (Raylib.IsMouseButtonDown(MouseButton.Left)) return true;
        if (GamepadAvailable && Raylib.IsGamepadButtonDown(0, GamepadButton.RightTrigger1)) return true;
        return false;
    }

    public static bool IsFirePressed()
    {
        if (Raylib.IsMouseButtonPressed(MouseButton.Left)) return true;
        if (GamepadAvailable && Raylib.IsGamepadButtonPressed(0, GamepadButton.RightTrigger1)) return true;
        return false;
    }

    public static bool IsConfirmPressed()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Enter) || Raylib.IsKeyPressed(KeyboardKey.Space)) return true;
        if (GamepadAvailable && Raylib.IsGamepadButtonPressed(0, GamepadButton.RightFaceDown)) return true; // A button
        return false;
    }

    public static bool IsCancelPressed()
    {
        if (Raylib.IsKeyPressed(KeyboardKey.Escape)) return true;
        if (GamepadAvailable && Raylib.IsGamepadButtonPressed(0, GamepadButton.RightFaceRight)) return true; // B button
        return false;
    }

    public static int GetMenuHorizontal()
    {
        int dir = 0;
        if (Raylib.IsKeyPressed(KeyboardKey.Left) || Raylib.IsKeyPressed(KeyboardKey.A)) dir--;
        if (Raylib.IsKeyPressed(KeyboardKey.Right) || Raylib.IsKeyPressed(KeyboardKey.D)) dir++;
        if (GamepadAvailable)
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
        if (GamepadAvailable)
        {
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftFaceUp)) dir--;
            if (Raylib.IsGamepadButtonPressed(0, GamepadButton.LeftFaceDown)) dir++;
        }
        return dir;
    }
}
