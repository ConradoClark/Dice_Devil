using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class ManaManager : BaseGameObject
{
    public float RechargeRate;

    public float Amount;
    public float MaxAmount = 1000;

    public float MinSize;
    public float MaxSize;

    public SpriteRenderer SpriteRenderer;

    protected override void OnAwake()
    {
        base.OnAwake();
        Amount = 0;
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(RechargeMana());
    }

    public bool SpendMana(int amount)
    {
        if (Amount < amount) return false;
        Amount -= amount;
        AdjustSize();
        return true;
    }

    private IEnumerable<IEnumerable<Action>> RechargeMana()
    {
        while (isActiveAndEnabled)
        {
            Amount += (float) GameTimer.UpdatedTimeInMilliseconds * RechargeRate * 0.01f;
            Amount = Mathf.Clamp(Amount, 0, MaxAmount);
            AdjustSize();

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private void AdjustSize()
    {
        SpriteRenderer.size = new Vector2(MinSize + Amount / MaxAmount * (MaxSize - MinSize), SpriteRenderer.size.y);
    }
}
