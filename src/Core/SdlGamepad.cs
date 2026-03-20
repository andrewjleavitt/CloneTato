using System.Reflection;
using System.Runtime.InteropServices;

namespace CloneTato.Core;

/// <summary>
/// Minimal SDL2 P/Invoke wrapper for gamepad input on macOS,
/// where Raylib's GLFW backend doesn't read controller axes.
/// </summary>
public class SdlGamepad : IDisposable
{
    private const string Lib = "SDL2";

    static SdlGamepad()
    {
        NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), (name, assembly, path) =>
        {
            if (name != Lib) return IntPtr.Zero;
            if (NativeLibrary.TryLoad(name, assembly, path, out IntPtr handle))
                return handle;
            if (NativeLibrary.TryLoad("/opt/homebrew/lib/libSDL2.dylib", out handle))
                return handle;
            if (NativeLibrary.TryLoad("/usr/local/lib/libSDL2.dylib", out handle))
                return handle;
            return IntPtr.Zero;
        });
    }

    private const uint SDL_INIT_GAMECONTROLLER = 0x00002000;

    // SDL_GameControllerAxis
    public const int AXIS_LEFTX = 0;
    public const int AXIS_LEFTY = 1;
    public const int AXIS_RIGHTX = 2;
    public const int AXIS_RIGHTY = 3;
    public const int AXIS_TRIGGERLEFT = 4;
    public const int AXIS_TRIGGERRIGHT = 5;

    // SDL_GameControllerButton
    public const int BUTTON_A = 0;
    public const int BUTTON_B = 1;
    public const int BUTTON_X = 2;
    public const int BUTTON_Y = 3;
    public const int BUTTON_BACK = 4;
    public const int BUTTON_GUIDE = 5;
    public const int BUTTON_START = 6;
    public const int BUTTON_LEFTSTICK = 7;
    public const int BUTTON_RIGHTSTICK = 8;
    public const int BUTTON_LEFTSHOULDER = 9;
    public const int BUTTON_RIGHTSHOULDER = 10;
    public const int BUTTON_DPAD_UP = 11;
    public const int BUTTON_DPAD_DOWN = 12;
    public const int BUTTON_DPAD_LEFT = 13;
    public const int BUTTON_DPAD_RIGHT = 14;

    [DllImport(Lib)] private static extern int SDL_Init(uint flags);
    [DllImport(Lib)] private static extern void SDL_Quit();
    [DllImport(Lib)] private static extern void SDL_PumpEvents();
    [DllImport(Lib)] private static extern int SDL_NumJoysticks();
    [DllImport(Lib)] private static extern int SDL_IsGameController(int joystickIndex);
    [DllImport(Lib)] private static extern IntPtr SDL_GameControllerOpen(int joystickIndex);
    [DllImport(Lib)] private static extern void SDL_GameControllerClose(IntPtr controller);
    [DllImport(Lib)] private static extern short SDL_GameControllerGetAxis(IntPtr controller, int axis);
    [DllImport(Lib)] private static extern byte SDL_GameControllerGetButton(IntPtr controller, int button);
    [DllImport(Lib)] private static extern IntPtr SDL_GameControllerName(IntPtr controller);
    [DllImport(Lib)] private static extern int SDL_GameControllerAddMapping([MarshalAs(UnmanagedType.LPUTF8Str)] string mapping);

    private IntPtr _controller;
    private bool _disposed;

    private readonly bool[] _buttonState = new bool[16];
    private readonly bool[] _buttonPrev = new bool[16];

    public bool Connected => _controller != IntPtr.Zero;
    public string? Name { get; private set; }

    public SdlGamepad()
    {
        if (SDL_Init(SDL_INIT_GAMECONTROLLER) < 0)
            return;

        // Load community mappings
        string[] mapPaths = { "gamecontrollerdb.txt", "assets/gamecontrollerdb.txt" };
        foreach (var mapPath in mapPaths)
        {
            if (!File.Exists(mapPath)) continue;
            foreach (string line in File.ReadLines(mapPath))
            {
                if (!string.IsNullOrWhiteSpace(line) && !line.StartsWith('#'))
                    SDL_GameControllerAddMapping(line);
            }
            break;
        }

        TryOpen();
    }

    private void TryOpen()
    {
        int count = SDL_NumJoysticks();
        for (int i = 0; i < count; i++)
        {
            if (SDL_IsGameController(i) != 0)
            {
                _controller = SDL_GameControllerOpen(i);
                if (_controller != IntPtr.Zero)
                {
                    IntPtr namePtr = SDL_GameControllerName(_controller);
                    Name = namePtr != IntPtr.Zero ? Marshal.PtrToStringUTF8(namePtr) : "Unknown";
                    Console.WriteLine($"[SDL Gamepad] Connected: {Name}");
                    return;
                }
            }
        }
    }

    public void Update()
    {
        SDL_PumpEvents();

        if (_controller == IntPtr.Zero)
        {
            TryOpen();
            if (_controller == IntPtr.Zero) return;
        }

        for (int i = 0; i < _buttonState.Length; i++)
        {
            _buttonPrev[i] = _buttonState[i];
            _buttonState[i] = SDL_GameControllerGetButton(_controller, i) != 0;
        }
    }

    public float GetAxis(int axis)
    {
        if (_controller == IntPtr.Zero) return 0f;
        short raw = SDL_GameControllerGetAxis(_controller, axis);
        return raw / 32767f;
    }

    public bool IsButtonDown(int button)
    {
        if (button < 0 || button >= _buttonState.Length) return false;
        return _buttonState[button];
    }

    public bool IsButtonPressed(int button)
    {
        if (button < 0 || button >= _buttonState.Length) return false;
        return _buttonState[button] && !_buttonPrev[button];
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_controller != IntPtr.Zero)
        {
            SDL_GameControllerClose(_controller);
            _controller = IntPtr.Zero;
        }
        SDL_Quit();
    }
}
