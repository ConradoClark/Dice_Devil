using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class TileTokenCard : TokenCard
{
    public ScriptPrefab TileType;
    private TilePool _tilePool;

    private ClickDetectionMixin _clicks;
    private Grid _grid;
    private bool _placed;

    protected override void OnAwake()
    {
        base.OnAwake();
        _tilePool = SceneObject<TilePoolManager>.Instance().GetEffect(TileType);
        _grid = SceneObject<Grid>.Instance();
        var standards = SceneObject<InputStandards>.Instance();
        _clicks = new ClickDetectionMixinBuilder(this, standards.MousePosInput, standards.LeftClickInput).Build();
    }

    public override IEnumerable<IEnumerable<Action>> Use()
    {
        yield return HandleClickOnWorld().AsCoroutine();

        IsEffectOver = true;
    }

    private IEnumerable<IEnumerable<Action>> HandleClickOnWorld()
    {
        _placed = false;
        if (_tilePool.TryGetFromPool(out var tile))
        {
            while (!_placed)
            {
                var cellPos = _grid.WorldToCell(_clicks.GetMousePosition());
                var pos = _grid.CellToWorld(cellPos);

                tile.transform.position = new Vector3(pos.x, pos.y, 0);
                tile.SetAllow();

                if (_clicks.WasClickedThisFrame(out _))
                {
                    var screenPoint = Camera.main.WorldToViewportPoint(pos);
                    if (screenPoint.x is > 0 and < 1 && screenPoint.y is > 0 and < 1) // is in viewport
                    {
                        tile.Place();
                        _placed = true;
                    }
                }

                yield return TimeYields.WaitOneFrameX;
            }
        }

        yield return TimeYields.WaitOneFrameX;
    }
}
