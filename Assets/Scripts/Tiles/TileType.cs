using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum TileType
{
    Fields,
    Water,
    River,
}

public static class TileTypeExtensions
{
    public static string AsString(this TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Fields: return "fields";
            case TileType.Water: return "water";
            case TileType.River: return "river";
            default:
                return tileType.ToString();
        }
    }

    public static int AsSpriteGlyph(this TileType tileType)
    {
        switch (tileType)
        {
            case TileType.Fields: return 0;
            case TileType.Water: return 1;
            case TileType.River: return 2;
            default:
                return -1;
        }
    }
}
