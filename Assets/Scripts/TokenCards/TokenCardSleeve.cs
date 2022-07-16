using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Licht.Impl.Generation;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Generation;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using UnityEngine;
using Random = UnityEngine.Random;

public class TokenCardSleeve : BaseGameObject, IGenerator<int,float>
{
    public float TargetY;
    public float StartingY;

    public float DrawTimeInSeconds;
    public float DelayBetweenDrawsInSeconds;

    private bool _drawn = false;
    public TokenCardSlot[] TokenCardSlots;

    public WeightedTokenCard[] PossibleTokenCards;
    private Dictionary<ScriptPrefab, TokenCardPool> _tokenCardPools;

    public Vector3 TokenCardOffset;

    [Serializable]
    public struct WeightedTokenCard : IWeighted<float>
    {
        public ScriptPrefab Prefab;
        public int TokenWeight;
        public float Weight => TokenWeight;
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _tokenCardPools = new Dictionary<ScriptPrefab, TokenCardPool>();

        var manager = SceneObject<TokenCardPoolManager>.Instance();
        foreach (var tokenCardDef in PossibleTokenCards)
        {
            _tokenCardPools[tokenCardDef.Prefab] = manager.GetEffect(tokenCardDef.Prefab);
        }
    }

    private void OnEnable()
    {

    }

    public IEnumerable<IEnumerable<Action>> Recall()
    {
        for (var i = 0; i < 6; i++)
        {
            DefaultMachinery.AddBasicMachine(RecallCard(i));
            yield return TimeYields.WaitSeconds(GameTimer, 0.2f);
        }

        yield return TimeYields.WaitOneFrameX;
    }

    private IEnumerable<IEnumerable<Action>> RecallCard(int index)
    {
        var tokenCard = TokenCardSlots[index];

        yield return tokenCard.transform.GetAccessor()
            .Position.Y
            .SetTarget(StartingY)
            .Over(DrawTimeInSeconds / 6)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .Build();
    }

    private IEnumerable<IEnumerable<Action>> DrawCard(int index)
    {
        var tokenCardSlot = TokenCardSlots[index];

        var rng = new WeightedDice<WeightedTokenCard>(PossibleTokenCards, this);

        var chosenCard = rng.Generate();

        if (_tokenCardPools[chosenCard.Prefab].TryGetFromPool(out var tokenCard))
        {
            tokenCard.transform.SetParent(tokenCardSlot.transform);
            tokenCard.transform.localPosition = TokenCardOffset;
        }

        yield return tokenCardSlot.transform.GetAccessor()
            .Position.Y
            .SetTarget(TargetY)
            .Over(DrawTimeInSeconds / 6)
            .UsingTimer(GameTimer)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .Build();
    }

    public IEnumerable<IEnumerable<Action>> Draw()
    {
        if (_drawn)
        {
            yield return Recall().AsCoroutine();
        }

        foreach (var tokenCard in TokenCardSlots)
        {
            tokenCard.transform.position =
                new Vector3(tokenCard.transform.position.x, StartingY, tokenCard.transform.position.z);
        }

        for (var i = 0; i < 6; i++)
        {
            DefaultMachinery.AddBasicMachine(DrawCard(i));
            yield return TimeYields.WaitSeconds(GameTimer, DelayBetweenDrawsInSeconds);
        }

        yield return TimeYields.WaitOneFrameX;
    }

    public int Seed { get; set; }
    public float Generate()
    {
        return Random.value;
    }
}
