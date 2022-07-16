using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class TokenCard : EffectPoolable
{
    public SpriteRenderer SpriteRenderer;

    public override void OnActivation()
    {
        
    }

    public virtual IEnumerable<IEnumerable<Action>> Use()
    {
        IsEffectOver = true;
        yield break;
    }

    public override bool IsEffectOver { get; protected set; }
}
