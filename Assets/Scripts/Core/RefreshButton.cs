using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

public class RefreshButton : BaseUIButton
{
    private TokenCardSleeve _tokenCardSleeve;
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
