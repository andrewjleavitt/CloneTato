using System.Numerics;
using Raylib_cs;

namespace CloneTato.Core;

public class GameCamera
{
    public Camera2D Camera;
    private Vector2 _target;
    private const float LerpSpeed = 12f; // snappy camera — tight twin-stick feel

    public GameCamera()
    {
        Camera = new Camera2D
        {
            Offset = new Vector2(Constants.LogicalWidth / 2f, Constants.LogicalHeight / 2f),
            Target = Vector2.Zero,
            Rotation = 0f,
            Zoom = 1f,
        };
    }

    public void Update(Vector2 playerPos, float dt)
    {
        _target = Vector2.Lerp(_target, playerPos, LerpSpeed * dt);

        // Clamp so camera doesn't show outside arena
        float halfW = Constants.LogicalWidth / 2f;
        float halfH = Constants.LogicalHeight / 2f;
        _target.X = Math.Clamp(_target.X, halfW, Constants.ArenaWidth - halfW);
        _target.Y = Math.Clamp(_target.Y, halfH, Constants.ArenaHeight - halfH);

        Camera.Target = _target;
    }

    public void SnapTo(Vector2 pos)
    {
        _target = pos;
        _target.X = Math.Clamp(_target.X, Constants.LogicalWidth / 2f, Constants.ArenaWidth - Constants.LogicalWidth / 2f);
        _target.Y = Math.Clamp(_target.Y, Constants.LogicalHeight / 2f, Constants.ArenaHeight - Constants.LogicalHeight / 2f);
        Camera.Target = _target;
    }
}
