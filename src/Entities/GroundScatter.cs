using System.Numerics;

namespace CloneTato.Entities;

public struct GroundScatter
{
    public Vector2 Position;
    public int TextureIndex; // index into GroundScatterTextures or LargeScatterTextures
    public bool FlipH;
    public bool IsLarge; // true = LargeScatterTextures, false = GroundScatterTextures
}
