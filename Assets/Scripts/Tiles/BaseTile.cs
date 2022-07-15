using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

public class BaseTile : BaseGameObject
{
    protected Grid Grid;
    protected override void OnAwake()
    {
        base.OnAwake();
        Grid = SceneObject<Grid>.Instance();

        Debug.Log(Grid.WorldToCell(transform.position));
    }
}
