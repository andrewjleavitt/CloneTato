using System.Numerics;

namespace DesertEngine.Core;

public static class CollisionHelper
{
    public static bool CircleOverlap(Vector2 a, float ra, Vector2 b, float rb)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        float distSq = dx * dx + dy * dy;
        float rSum = ra + rb;
        return distSq <= rSum * rSum;
    }

    public static bool PointInCircle(Vector2 point, Vector2 center, float radius)
    {
        float dx = point.X - center.X;
        float dy = point.Y - center.Y;
        return dx * dx + dy * dy <= radius * radius;
    }

    public static bool PointInRect(Vector2 point, float rx, float ry, float rw, float rh)
    {
        return point.X >= rx && point.X <= rx + rw && point.Y >= ry && point.Y <= ry + rh;
    }
}
