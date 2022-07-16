using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameTileMap : MonoBehaviour
{
    public Dictionary<Vector2Int, BaseTile> Tiles { get; private set; }
    private void Awake()
    {
        Tiles = new Dictionary<Vector2Int, BaseTile>();
    }

    public void PlaceTile(BaseTile tile, Vector2Int position)
    {
        Tiles[position] = tile;
    }

    public bool IsOccupied(Vector2Int position)
    {
        return Tiles.ContainsKey(position);
    }

    public bool HasAdjacent(Vector2Int position)
    {
        var north = position + Vector2Int.up;
        var south = position + Vector2Int.down;
        var east = position + Vector2Int.right;
        var west = position + Vector2Int.left;

        return Tiles.ContainsKey(north) || Tiles.ContainsKey(east) || Tiles.ContainsKey(west) ||
               Tiles.ContainsKey(south);
    }
}
