using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

public abstract class BaseTransformation : MonoBehaviour
{
    public ScriptPrefab TileType;

    public abstract bool ValidateSurroundings(GameTileMap map, BaseTile center, Vector2Int position);
}
