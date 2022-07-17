using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

public class RollButton : BaseUIButton
{
    public int BaseManaCost;

    public override int ManaCost => BaseManaCost;

    public ScriptPrefab CardSelectorPrefab;

    private TokenCardSelectorPool _selectorPool;

    protected override void OnAwake()
    {
        base.OnAwake();
        _selectorPool = SceneObject<TokenCardSelectorPoolManager>.Instance().GetEffect(CardSelectorPrefab);

    }

    protected override IEnumerable<IEnumerable<Action>> OnClick()
    {
        roll:
        if (_selectorPool.TryGetFromPool(out var selector))
        {
            yield return selector.Roll().AsCoroutine();

            yield return Lift().AsCoroutine(); // allow clicking again to reroll

            while (selector.SelectedSlot.IsSelected)
            {
                if (Clickable.WasClickedThisFrame())
                {
                    yield return selector.MarkAsUsedAndWait().AsCoroutine();
                    yield return TimeYields.WaitOneFrameX;
                    goto roll;
                }

                yield return TimeYields.WaitOneFrameX;
            }

            selector.MarkAsUsed();
        }

        yield return TimeYields.WaitOneFrameX;
    }
}
