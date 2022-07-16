using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class BaseTile : EffectPoolable
{
    public bool IsStarting;
    public bool IsPlaced { get; private set; }
    public SpriteRenderer Sprite;

    protected Grid Grid;
    protected GameTileMap GameTileMap;

    protected static string MatColor = "_Color";
    protected static string MatColorize = "_Colorize";
    protected static Color AllowPlacementColor = new (0, 1, 0, 0.5f);
    protected static Color DisallowPlacementColor = new (1, 0, 0, 0.5f);
    protected static Color NoColorize = new(0, 0, 0, 0);

    protected override void OnAwake()
    {
        base.OnAwake();
        Grid = SceneObject<Grid>.Instance();
        GameTileMap = SceneObject<GameTileMap>.Instance();
    }

    private void OnEnable()
    {
        if (IsStarting)
        {
            Place();
        }
    }

    public override void OnActivation()
    {
        Sprite.material.SetColor(MatColor, new Color(1, 1, 1, 0.5f));
    }

    public void SetAllow(bool allow)
    {
        if (IsPlaced) return;
        Sprite.material.SetColor(MatColorize, allow ? AllowPlacementColor : DisallowPlacementColor);
    }

    public void Place()
    {
        if (IsPlaced) return;
        var pos = Grid.WorldToCell(transform.position);
        GameTileMap.PlaceTile(this, (Vector2Int) pos);
        Sprite.material.SetColor(MatColor, Color.white);
        Sprite.material.SetColor(MatColorize, NoColorize);
        IsPlaced = true;
    }

    public override bool IsEffectOver { get; protected set; }
}
