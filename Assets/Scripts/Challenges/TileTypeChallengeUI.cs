using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Extensions;
using Licht.Unity.Pooling;
using TMPro;
using UnityEngine;

public class TileTypeChallengeUI : EffectPoolable
{
    public SpriteRenderer Label;
    public TMP_Text ChallengeText;
    public TMP_Text ProgressText;

    public Color SuccessColorize;
    public Color FailureColorize;
    private Color _originalColor;

    protected override void OnAwake()
    {
        base.OnAwake();
        _originalColor = Label.material.GetColor("_Colorize");
    }

    public override void OnActivation()
    {
        Label.material.SetColor("_Colorize", _originalColor);
    }

    public IEnumerable<IEnumerable<Action>> SetToSuccess()
    {
        yield return Label.GetAccessor()
            .Material("_Colorize")
            .AsColor()
            .ToColor(SuccessColorize)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();
    }

    public IEnumerable<IEnumerable<Action>> SetToFailure()
    {
        yield return Label.GetAccessor()
            .Material("_Colorize")
            .AsColor()
            .ToColor(FailureColorize)
            .Over(0.5f)
            .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
            .UsingTimer(GameTimer)
            .Build();
    }

    public void Hide()
    {
        IsEffectOver = true;
    }

    public override bool IsEffectOver { get; protected set; }
}
