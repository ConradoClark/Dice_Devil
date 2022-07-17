using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Mixins;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;

public abstract class BaseUIButton : BaseUIObject
{
    public Sprite ButtonSprite;
    public Sprite ClickedSprite;
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer DarkBackground;
    public TMP_Text Cost;
    public abstract int ManaCost { get; }

    public bool IsActive { get; protected set; }

    protected ClickableObjectMixin Clickable;
    protected ManaManager ManaManager;
    protected Vector3 OriginalPosition;

    protected override void OnAwake()
    {
        base.OnAwake();
        var uiCamera = SceneObject<UICamera>.Instance().Camera;
        var standards = SceneObject<InputStandards>.Instance();
        OriginalPosition = transform.position;
        Clickable = new ClickableObjectMixinBuilder(this, standards.MousePosInput, standards.LeftClickInput)
            .WithCamera(uiCamera)
            .Build();
        ManaManager = SceneObject<ManaManager>.Instance();
    }

    protected virtual void OnEnable()
    {
        Cost.text = ManaCost.ToString();
        DefaultMachinery.AddBasicMachine(HandleCost());
        DefaultMachinery.AddBasicMachine(HandleClick());
    }


    private IEnumerable<IEnumerable<Action>> HandleCost()
    {
        while (isActiveAndEnabled)
        {
            Cost.text = ManaCost.ToString();
            yield return TimeYields.WaitMilliseconds(UITimer, 50);
        }
    }

    protected IEnumerable<IEnumerable<Action>> HandleClick()
    {
        while (isActiveAndEnabled)
        {
            if (Clickable.WasClickedThisFrame() && !IsActive)
            {
                if (!ManaManager.SpendMana(ManaCost))
                {
                    yield return TimeYields.WaitOneFrameX;
                    continue;
                }

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

    protected IEnumerable<IEnumerable<Action>> Lift()
    {
        IsActive = false;
        SpriteRenderer.sprite = ButtonSprite;
        DarkBackground.enabled = false;
        transform.position = OriginalPosition;
        yield break;
    }

    protected abstract IEnumerable<IEnumerable<Action>> OnClick();
}
