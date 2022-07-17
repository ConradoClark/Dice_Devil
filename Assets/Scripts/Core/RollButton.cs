using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

public class RollButton : BaseUIButton
{
    public int BaseManaCost;
    private TokenCardSleeve _tokenCardSleeve;

    public override int ManaCost {
        get
        {
            var baseCost = BaseManaCost -
                           15 * _tokenCardSleeve.TokenCardSlots.Count(t =>
                               t.TokenCard != null && !t.TokenCard.IsActive);
            
            var extraCost = (_tokenCardSleeve.TokenCardSlots?.Select(t => t.IsBlocked ? 50 : 0).Sum() ?? 0) * 2;

            var cost = baseCost + extraCost;
            return Math.Clamp(cost, 0, cost);
        }
    }

    public ScriptPrefab CardSelectorPrefab;

    private TokenCardSelectorPool _selectorPool;

    protected override void OnAwake()
    {
        base.OnAwake();
        _selectorPool = SceneObject<TokenCardSelectorPoolManager>.Instance().GetEffect(CardSelectorPrefab);
        _tokenCardSleeve = SceneObject<TokenCardSleeve>.Instance();
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
