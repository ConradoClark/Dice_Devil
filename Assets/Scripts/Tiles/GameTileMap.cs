using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
}
