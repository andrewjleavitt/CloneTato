using Raylib_cs;

namespace DesertEngine.Assets;

public class SpriteAtlas
{
    public Texture2D Texture { get; }
    public int TileWidth { get; }
    public int TileHeight { get; }
    public int Spacing { get; }
    public int Columns { get; }
    public int Rows { get; }
    public int TotalTiles { get; }

    public SpriteAtlas(Texture2D texture, int tileWidth, int tileHeight, int spacing, int columns, int rows)
    {
        Texture = texture;
        TileWidth = tileWidth;
        TileHeight = tileHeight;
        Spacing = spacing;
        Columns = columns;
        Rows = rows;
        TotalTiles = columns * rows;
    }

    public Rectangle GetSourceRect(int index)
    {
        int col = index % Columns;
        int row = index / Columns;
        float x = col * (TileWidth + Spacing);
        float y = row * (TileHeight + Spacing);
        return new Rectangle(x, y, TileWidth, TileHeight);
    }

    public void Draw(int index, float x, float y, Color tint)
    {
        var src = GetSourceRect(index);
        var dest = new Rectangle(x, y, TileWidth, TileHeight);
        Raylib.DrawTexturePro(Texture, src, dest, System.Numerics.Vector2.Zero, 0f, tint);
    }

    public void DrawCentered(int index, float x, float y, Color tint)
    {
        var src = GetSourceRect(index);
        var dest = new Rectangle(x, y, TileWidth, TileHeight);
        var origin = new System.Numerics.Vector2(TileWidth / 2f, TileHeight / 2f);
        Raylib.DrawTexturePro(Texture, src, dest, origin, 0f, tint);
    }

    public void DrawScaled(int index, float x, float y, float scale, Color tint)
    {
        var src = GetSourceRect(index);
        var dest = new Rectangle(x, y, TileWidth * scale, TileHeight * scale);
        var origin = new System.Numerics.Vector2(TileWidth * scale / 2f, TileHeight * scale / 2f);
        Raylib.DrawTexturePro(Texture, src, dest, origin, 0f, tint);
    }
}
