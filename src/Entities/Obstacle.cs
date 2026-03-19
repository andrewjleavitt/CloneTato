using System.Numerics;

namespace CloneTato.Entities;

public class Obstacle
{
    public Vector2 Position;
    public float Radius;
    public int SpriteIndex;     // Kenney tile index (legacy)
    public int TextureIndex;    // STRANDED ObstacleTextures index
    public bool UseStranded;    // true = draw STRANDED texture, false = Kenney tile
    public bool Active;
}
