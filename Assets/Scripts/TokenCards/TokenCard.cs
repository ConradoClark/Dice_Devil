using System.Collections;
using System.Collections.Generic;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

public class TokenCard : EffectPoolable
{
    public override void OnActivation()
    {
        
    }

    public virtual void Use()
    {
        // maybe do a effect before setting this to true
        IsEffectOver = true;
        // go to respective inventory, blablabla
    }

    public override bool IsEffectOver { get; protected set; }
}
