using CloneTato;
using CloneTato.Assets;
using CloneTato.Core;
using Raylib_cs;

// Load settings before anything else
var settings = GameSettings.Load();

Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
Raylib.InitWindow(Constants.WindowWidth, Constants.WindowHeight, "DRIFT — Desert Survivor");
Raylib.SetExitKey(KeyboardKey.Null); // disable ESC auto-close, we handle it ourselves
Raylib.SetTargetFPS(60);
Raylib.InitAudioDevice();

// SDL gamepad for macOS Xbox controller support (GLFW can't read axes)
InputHelper.InitGamepad();

// Apply saved settings
settings.Apply();
if (settings.Fullscreen)
    Raylib.ToggleBorderlessWindowed();

// Render texture for pixel-perfect scaling
var renderTarget = Raylib.LoadRenderTexture(Constants.LogicalWidth, Constants.LogicalHeight);
Raylib.SetTextureFilter(renderTarget.Texture, TextureFilter.Point);

// Load assets
var assets = new AssetManager();
assets.LoadAll();

// Init game
var manager = new GameStateManager();
manager.Init(assets);

while (!Raylib.WindowShouldClose() && !manager.QuitRequested)
{
    float dt = Raylib.GetFrameTime();

    // Poll SDL gamepad
    InputHelper.UpdateGamepad();

    // Update display scaling (handles fullscreen/windowed transitions)
    Display.Update();

    // Hide OS cursor during unpaused gameplay (we have a custom reticle)
    if (manager.CurrentScreen == GameScreen.Playing && !manager.IsPaused)
    {
        if (!Raylib.IsCursorHidden()) Raylib.HideCursor();
    }
    else
    {
        if (Raylib.IsCursorHidden()) Raylib.ShowCursor();
    }

    // Update
    manager.Update(dt);

    // Render to logical resolution
    Raylib.BeginTextureMode(renderTarget);
    manager.Draw();
    Raylib.EndTextureMode();

    // Draw scaled to window (with letterboxing support)
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    // Apply color grading post-process during gameplay screens
    bool applyColorGrade = assets.ColorGradeShader.Id != 0
        && manager.CurrentScreen is GameScreen.Playing or GameScreen.Shop
            or GameScreen.LevelUp or GameScreen.GameOver or GameScreen.Victory;
    if (applyColorGrade) Raylib.BeginShaderMode(assets.ColorGradeShader);

    // Flip the render texture vertically (render textures are flipped in OpenGL)
    var srcRect = new Rectangle(0, 0, renderTarget.Texture.Width, -renderTarget.Texture.Height);
    Raylib.DrawTexturePro(renderTarget.Texture, srcRect, Display.DestRect,
        System.Numerics.Vector2.Zero, 0f, Color.White);

    if (applyColorGrade) Raylib.EndShaderMode();

    Raylib.DrawFPS(4, 4);
    Raylib.EndDrawing();
}

// Cleanup
InputHelper.ShutdownGamepad();
Raylib.UnloadRenderTexture(renderTarget);
assets.UnloadAll();
Raylib.CloseAudioDevice();
Raylib.CloseWindow();
