using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class GameTileMap : BaseGameObject
{
    public BaseTransformation[] PossibleTransformations;
    public Dictionary<Vector2Int, BaseTile> Tiles { get; private set; }

    private Dictionary<ScriptPrefab, TilePool> _tilePools;

    public event Action<BaseTile> OnTilesChanged;

    protected override void OnAwake()
    {
        base.OnAwake();
        Tiles = new Dictionary<Vector2Int, BaseTile>();
        _tilePools = new Dictionary<ScriptPrefab, TilePool>();

        // Initialize pools for transformations
        var tileOutputs = PossibleTransformations.Select(t => t.TileType).Distinct().ToArray();
        var manager = SceneObject<TilePoolManager>.Instance();

        foreach (var tileOutput in tileOutputs)
        {
            _tilePools[tileOutput] = manager.GetEffect(tileOutput);
        }
    }

    public void RemoveTile(BaseTile tile, bool checkTransformations)
    {
        Tiles.Remove(tile.CurrentPosition);
        OnTilesChanged?.Invoke(tile);

        if (checkTransformations)
        {
            DefaultMachinery.AddBasicMachine(Validate9X9Tiles(tile.CurrentPosition));
        }
    }

    private void TransformTile(BaseTile tile, ScriptPrefab targetTile)
    {
        tile.Release();

        if (_tilePools[targetTile].TryGetFromPool(out var newTile))
        {
            newTile.transform.position = tile.transform.position;
            newTile.Place();
            OnTilesChanged?.Invoke(newTile);
        }
    }

    public IEnumerable<IEnumerable<Action>> PlaceTile(BaseTile tile)
    {
        Tiles[tile.CurrentPosition] = tile;
        OnTilesChanged?.Invoke(tile);
        yield return Validate9X9Tiles(tile.CurrentPosition).AsCoroutine();
    }

    public IEnumerable<IEnumerable<Action>> Validate9X9Tiles(Vector2Int center)
    {
        if (TryGetTile(center, out var tile))
        {
            foreach (var transformation in PossibleTransformations)
            {
                var needsToTransform = transformation.ValidateSurroundings(this, tile, tile.CurrentPosition);
                if (!needsToTransform) continue;

                TransformTile(tile, transformation.TileType);

                break;
            }

            yield return TimeYields.WaitSeconds(GameTimer, 0.2f);
        }

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                var pos = tile.CurrentPosition - new Vector2Int(i - 1, j - 1);
                if (!TryGetTile(pos, out var neighbor)) continue; // no tiles here, keep going
                
                foreach (var transformation in PossibleTransformations)
                {
                        var needsToTransform = transformation.ValidateSurroundings(this, neighbor, pos);
                    if (!needsToTransform) continue;

                    TransformTile(neighbor, transformation.TileType);

                    yield return TimeYields.WaitSeconds(GameTimer, 0.2f);

                    break;
                }
            }
        }
    }

    public bool TryGetTile(Vector2Int position, out BaseTile tile)
    {
        if (Tiles.ContainsKey(position))
        {
            tile = Tiles[position];
            return true;
        }

        tile = null;
        return false;
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
