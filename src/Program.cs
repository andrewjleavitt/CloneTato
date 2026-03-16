using CloneTato;
using CloneTato.Assets;
using CloneTato.Core;
using Raylib_cs;

Raylib.SetConfigFlags(ConfigFlags.VSyncHint);
Raylib.InitWindow(Constants.WindowWidth, Constants.WindowHeight, "CloneTato - Desert Survivor");
Raylib.SetTargetFPS(60);
Raylib.InitAudioDevice();

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

    // Update
    manager.Update(dt);

    // Render to logical resolution
    Raylib.BeginTextureMode(renderTarget);
    manager.Draw();
    Raylib.EndTextureMode();

    // Draw scaled to window
    Raylib.BeginDrawing();
    Raylib.ClearBackground(Color.Black);

    // Flip the render texture vertically (render textures are flipped in OpenGL)
    var srcRect = new Rectangle(0, 0, renderTarget.Texture.Width, -renderTarget.Texture.Height);
    var destRect = new Rectangle(0, 0, Constants.WindowWidth, Constants.WindowHeight);
    Raylib.DrawTexturePro(renderTarget.Texture, srcRect, destRect,
        System.Numerics.Vector2.Zero, 0f, Color.White);

    Raylib.DrawFPS(4, 4);
    Raylib.EndDrawing();
}

// Cleanup
Raylib.UnloadRenderTexture(renderTarget);
assets.UnloadAll();
Raylib.CloseAudioDevice();
Raylib.CloseWindow();
