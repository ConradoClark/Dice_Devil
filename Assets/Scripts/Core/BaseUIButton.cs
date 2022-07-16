using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using UnityEngine;

public abstract class BaseUIButton : BaseUIObject
{
    public Sprite ButtonSprite;
    public Sprite ClickedSprite;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer DarkBackground;
    public ScriptInput MousePosInput;
    public ScriptInput MouseClickInput;

    public bool IsActive { get; protected set; }

    protected ClickableObjectMixin Clickable;

    protected Vector3 OriginalPosition;

    protected override void OnAwake()
    {
        base.OnAwake();
        var uiCamera = SceneObject<UICamera>.Instance().Camera;
        OriginalPosition = transform.position;
        Clickable = new ClickableObjectMixinBuilder(this, MousePosInput, MouseClickInput)
            .WithCamera(uiCamera)
            .Build();
    }

    protected virtual void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleClick());
    }

    protected IEnumerable<IEnumerable<Action>> HandleClick()
    {
        while (isActiveAndEnabled)
        {
            if (Clickable.WasClickedThisFrame() && !IsActive)
            {
                IsActive = true;
                SpriteRenderer.sprite = ClickedSprite;
                DarkBackground.enabled = true;
                transform.position -= new Vector3(0, 0.05f, 0);

                yield return OnClick().AsCoroutine();

                IsActive = false;
                SpriteRenderer.sprite = ButtonSprite;
                DarkBackground.enabled = false;
                transform.position = OriginalPosition;
            }
            yield return TimeYields.WaitOneFrameX;
        }
    }

    protected abstract IEnumerable<IEnumerable<Action>> OnClick();
}
