using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class RollButton : BaseUIButton
{
    public ScriptPrefab CardSelectorPrefab;
    private TokenCardSelectorPool _selectorPool;
    protected override void OnAwake()
    {
        base.OnAwake();
        _selectorPool = SceneObject<TokenCardSelectorPoolManager>.Instance().GetEffect(CardSelectorPrefab);
    }

    protected override IEnumerable<IEnumerable<Action>> OnClick()
    {
        Debug.Log("clicked on roll button!");

        if (_selectorPool.TryGetFromPool(out var selector))
        {
            yield return selector.Roll().AsCoroutine();

            while (selector.SelectedSlot.IsSelected)
            {
                yield return TimeYields.WaitOneFrameX;
            }

            selector.MarkAsUsed();
        }

        yield return TimeYields.WaitOneFrameX;
    }
}
