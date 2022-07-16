using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public class RollButton : BaseUIObject
{
    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;
    private ClickableObjectMixin _clickable;
    protected override void OnAwake()
    {
        base.OnAwake();
        var uiCamera = SceneObject<UICamera>.Instance().Camera;
        _clickable = new ClickableObjectMixinBuilder(this, MousePosInput, MouseClickInput)
            .WithCamera(uiCamera)
            .Build();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleClick());
    }

    private IEnumerable<IEnumerable<Action>> HandleClick()
    {
        while (isActiveAndEnabled)
        {
            if (_clickable.WasClickedThisFrame())
            {
                Debug.Log("clicked on roll button!");
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }
}
