using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class BaseTile : EffectPoolable
{
    public TileType TileType;
    public bool IsStarting;
    public bool IsPlaced { get; private set; }
    public SpriteRenderer Sprite;
    public Vector2Int CurrentPosition { get; set; }

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
        IsPlaced = false;
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
        CurrentPosition = (Vector2Int) pos;
        DefaultMachinery.AddBasicMachine(GameTileMap.PlaceTile(this));
        Sprite.material.SetColor(MatColor, Color.white);
        Sprite.material.SetColor(MatColorize, NoColorize);
        IsPlaced = true;
    }

    public void Release()
    {
        GameTileMap.RemoveTile(this, false);
        IsPlaced = false;
        IsEffectOver = true;
    }

    public override bool IsEffectOver { get; protected set; }
}
