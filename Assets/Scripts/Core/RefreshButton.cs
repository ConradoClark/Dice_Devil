using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using TMPro;

public class RefreshButton : BaseUIButton
{
    public int BaseManaCost;
    private TokenCardSleeve _tokenCardSleeve;
    public override int ManaCost
    {
        get
        {
            var cost = BaseManaCost - _tokenCardSleeve.TokenCardSlots?.Select(t => t.TokenCard == null || t.TokenCard.IsActive ? 0 : 100).Sum() ?? 0;
            var extraCost = (_tokenCardSleeve.TokenCardSlots?.Select(t => t.IsLocked ? 50 : 0).Sum() ?? 0) * 2;
            cost += extraCost;

            return Math.Clamp(cost, 0, cost);
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        _tokenCardSleeve = SceneObject<TokenCardSleeve>.Instance();
    }

    protected override IEnumerable<IEnumerable<Action>> OnClick()
    {
        yield return _tokenCardSleeve.Recall().AsCoroutine();
        yield return _tokenCardSleeve.Draw().AsCoroutine();

        yield return TimeYields.WaitOneFrameX;
    }
}
