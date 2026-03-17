using System.Numerics;
using Raylib_cs;

namespace DesertEngine.Core;

public class GameCamera
{
    public Camera2D Camera;
    private Vector2 _target;
    private readonly int _logicalWidth;
    private readonly int _logicalHeight;
    private const float LerpSpeed = 5f;

    public GameCamera(int logicalWidth, int logicalHeight)
    {
        _logicalWidth = logicalWidth;
        _logicalHeight = logicalHeight;
        Camera = new Camera2D
        {
            Offset = new Vector2(logicalWidth / 2f, logicalHeight / 2f),
            Target = Vector2.Zero,
            Rotation = 0f,
            Zoom = 1f,
        };
    }

    public void Update(Vector2 followTarget, float dt, float worldWidth, float worldHeight)
    {
        _target = Vector2.Lerp(_target, followTarget, LerpSpeed * dt);

        float halfW = _logicalWidth / 2f;
        float halfH = _logicalHeight / 2f;
        _target.X = Math.Clamp(_target.X, halfW, Math.Max(halfW, worldWidth - halfW));
        _target.Y = Math.Clamp(_target.Y, halfH, Math.Max(halfH, worldHeight - halfH));

        // Snap to integer pixels to prevent sub-pixel seams between tiles
        Camera.Target = new Vector2(MathF.Round(_target.X), MathF.Round(_target.Y));
    }

    public void SnapTo(Vector2 pos, float worldWidth, float worldHeight)
    {
        _target = pos;
        float halfW = _logicalWidth / 2f;
        float halfH = _logicalHeight / 2f;
        _target.X = Math.Clamp(_target.X, halfW, Math.Max(halfW, worldWidth - halfW));
        _target.Y = Math.Clamp(_target.Y, halfH, Math.Max(halfH, worldHeight - halfH));
        Camera.Target = new Vector2(MathF.Round(_target.X), MathF.Round(_target.Y));
    }
}
