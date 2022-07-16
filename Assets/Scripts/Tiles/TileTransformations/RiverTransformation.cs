using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RiverTransformation : BaseTransformation
{
    public override bool ValidateSurroundings(GameTileMap map, BaseTile center, Vector2Int position)
    {
        if (center.TileType != global::TileType.Water) return false;

        var hasHorizontalRiver = map.TryGetTile(position + Vector2Int.right, out var rightTile) &&
                                 rightTile.TileType == global::TileType.Fields &&
                                 map.TryGetTile(position + Vector2Int.left, out var leftTile) &&
                                 leftTile.TileType == global::TileType.Fields;

        var hasVerticalRiver = map.TryGetTile(position + Vector2Int.up, out var upTile) &&
                               upTile.TileType == global::TileType.Fields &&
                               map.TryGetTile(position + Vector2Int.down, out var downTile) &&
                               downTile.TileType == global::TileType.Fields;

        return hasHorizontalRiver || hasVerticalRiver;
    }
}
