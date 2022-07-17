using System;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Pooling;
using UnityEngine;

public class ChallengeTimer : EffectPoolable
{
    public float MaxSize;
    public float MinSize;

    public SpriteRenderer Bar;

    public Material GreenBar;
    public Material YellowBar;
    public Material RedBar;

    public EffectPoolable ParentChallengeUI;

    public float SecondsToExpire;
    private float _secondsLeft;

    public bool IsExpired { get; private set; }

    protected override void OnAwake()
    {
        base.OnAwake();
    }

    public void Hide()
    {
        IsEffectOver = true;
    }

    private IEnumerable<IEnumerable<Action>> TickAway()
    {
        yield return TimeYields.WaitOneFrameX; // wait for any changes to the timer

        if (SecondsToExpire <= 0) yield break;

        _secondsLeft = SecondsToExpire;
        Bar.material = GreenBar;
        var changedToYellow = false;
        var changedToRed = false;

        yield return TimeYields.WaitSeconds(GameTimer, _secondsLeft, time =>
        {
            var step = (float)time / (SecondsToExpire * 1000);
            _secondsLeft = step * SecondsToExpire;

            if (!changedToYellow && step > 0.5f)
            {
                Bar.material = YellowBar;
                changedToYellow = true;
            }

            if (!changedToRed && step > 0.75f)
            {
                Bar.material = RedBar;
                changedToRed = true;
            }

            Bar.size = new Vector2(MaxSize - step * (MaxSize - MinSize), Bar.size.y);
        });

        IsExpired = true;
    }

    public override void OnActivation()
    {
        DefaultMachinery.AddBasicMachine(TickAway());
    }

    private bool _isEffectOver;
    public override bool IsEffectOver
    {
        get => _isEffectOver || (ParentChallengeUI?.IsEffectOver ?? false);
        protected set => _isEffectOver = value;
    }
}
