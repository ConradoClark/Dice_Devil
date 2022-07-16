using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Update;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class TokenCardSlot : BaseUIObject, IResettable
{
    public Transform EmptyCard;
    public bool IsSelected { get; private set; }
    public TokenCard TokenCard { get; set; }

    private ClickableObjectMixin _clickable;
    private Cursor _cursor;

    protected override void OnAwake()
    {
        base.OnAwake();
        var inputStandards = SceneObject<InputStandards>.Instance();
        _cursor = SceneObject<Cursor>.Instance();
        var uiCamera = SceneObject<UICamera>.Instance().Camera;
        _clickable =
            new ClickableObjectMixinBuilder(this, inputStandards.MousePosInput, inputStandards.LeftClickInput)
                .WithCamera(uiCamera)
                .Build();
    }

    public void MarkAsSelected()
    {
        IsSelected = true;
        DefaultMachinery.AddBasicMachine(HandleSelect());
    }

    public bool PerformReset()
    {
        Discard();
        IsSelected = false;
        EmptyCard.gameObject.SetActive(false);
        return true;
    }

    public void Discard()
    {
        if (TokenCard.IsActive)
        {
            TokenCard.Discard();
        }

    }
    private IEnumerable<IEnumerable<Action>> HandleSelect()
    {
        if (!TokenCard.IsActive)
        {
            //play boo sound 
            yield return TimeYields.WaitSeconds(UITimer, 0.5f);
            IsSelected = false;
            yield break;
        }


        while (!_clickable.WasClickedThisFrame() || !IsSelected)
        {
            yield return TimeYields.WaitOneFrameX;
        }

        if (!IsSelected) // recalled
        {
            yield break;
        }

        _cursor.SetSprite(TokenCard.SpriteRenderer.sprite);

        yield return TokenCard.Use().AsCoroutine();

        _cursor.SetSprite(null);

        IsSelected = false;
        EmptyCard.gameObject.SetActive(true);
    }
}
