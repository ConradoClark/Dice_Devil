using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;
using Random = UnityEngine.Random;

public class TokenCardSelector : EffectPoolable
{
    public int RollRepeats;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer FillSpriteRenderer;
    public float TimeBetweenColorBlinksInMs;
    public float TimeBetweenRollsInSeconds;
    private TokenCardSleeve _sleeve;

    public TokenCardSlot SelectedSlot;

    private bool _rolling;
    private bool _used;

    public void MarkAsUsed()
    {
        if (!_rolling && !_used)
        {
            _used = true;
            DefaultMachinery.AddBasicMachine(Return());
        }
    }

    public IEnumerable<IEnumerable<Action>> MarkAsUsedAndWait()
    {
        if (!_rolling && !_used)
        {
            _used = true;
            yield return Return().AsCoroutine();
        }
    }

    protected override void OnAwake()
    {
        base.OnAwake();
        _sleeve = SceneObject<TokenCardSleeve>.Instance();
    }

    public override void OnActivation()
    {
        SelectedSlot = null;
        _used = false;
    }

    private IEnumerable<IEnumerable<Action>> BlinkColors()
    {
        FillSpriteRenderer.enabled = true;
        var originalColor = FillSpriteRenderer.color;
        while (_rolling)
        {
            var randomColor = Color.HSVToRGB(Random.value, 0.55f, 0.85f);
            randomColor.a = originalColor.a;
            FillSpriteRenderer.color = randomColor;
            yield return TimeYields.WaitMilliseconds(GameTimer, TimeBetweenColorBlinksInMs);
        }

        FillSpriteRenderer.color = originalColor;
    }

    private IEnumerable<IEnumerable<Action>> BlinkBorder()
    {
        var originalColor = SpriteRenderer.color;
        while (!_used)
        {
            var randomColor = Color.HSVToRGB(Random.value, 0.65f, 0.95f);
            randomColor.a = originalColor.a;
            SpriteRenderer.color = randomColor;
            yield return TimeYields.WaitMilliseconds(GameTimer, TimeBetweenColorBlinksInMs);

            SpriteRenderer.color = originalColor;
            yield return TimeYields.WaitMilliseconds(GameTimer, TimeBetweenColorBlinksInMs);
        }

        SpriteRenderer.color = originalColor;
    }

    public IEnumerable<IEnumerable<Action>> Roll()
    {
        _rolling = true;
        DefaultMachinery.AddBasicMachine(BlinkColors());
        for (var repeat = 0; repeat < RollRepeats; repeat++)
        {
            for (var i = 0; i < _sleeve.TokenCardSlots.Length; i++)
            {
                if (_sleeve.TokenCardSlots[i].IsBlocked) continue;
                transform.position = _sleeve.TokenCardSlots[i].transform.position;
                yield return TimeYields.WaitSeconds(GameTimer, TimeBetweenRollsInSeconds);
            }

            for (var i = _sleeve.TokenCardSlots.Length - 2; i > 0; i--)
            {
                if (_sleeve.TokenCardSlots[i - 1].IsBlocked) continue;
                transform.position = _sleeve.TokenCardSlots[i - 1].transform.position;
                yield return TimeYields.WaitSeconds(GameTimer, TimeBetweenRollsInSeconds);
            }
        }

        transform.position = _sleeve.TokenCardSlots.First(t=>!t.IsBlocked).transform.position;

        var validSlots = _sleeve.TokenCardSlots.Where(slot => !slot.IsBlocked).ToArray();
        var chosenSlot = validSlots[Random.Range(0, validSlots.Length)]; // roll of the dice

        for (var i = 0; i < _sleeve.TokenCardSlots.Length; i++)
        {
            if (_sleeve.TokenCardSlots[i].IsBlocked) continue;
            transform.position = _sleeve.TokenCardSlots[i].transform.position;
            if (_sleeve.TokenCardSlots[i] == chosenSlot)
            {
                break;
            }

            yield return TimeYields.WaitSeconds(GameTimer, TimeBetweenRollsInSeconds*2);
        }

        _rolling = false;

        transform.SetParent(chosenSlot.transform);
        SelectedSlot = chosenSlot;
        SelectedSlot.MarkAsSelected();
        FillSpriteRenderer.enabled = false;
        yield return chosenSlot.transform.GetAccessor()
            .Position
            .Y
            .Increase(0.5f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();

        DefaultMachinery.AddBasicMachine(BlinkBorder());
    }

    public IEnumerable<IEnumerable<Action>> Return()
    {
        yield return SelectedSlot.transform.GetAccessor()
            .Position
            .Y
            .Decrease(0.5f)
            .Over(0.25f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseInOut)
            .UsingTimer(GameTimer)
            .Build();
        IsEffectOver = true;
    }

    public override bool IsEffectOver { get; protected set; }
}
