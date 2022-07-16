using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class FollowMouse : BaseUIObject
{
    public ScriptBasicMachinery PostUpdate;

    private InputAction _mousePos;
    private Camera _uiCamera;
    protected override void OnAwake()
    {
        base.OnAwake();
        var playerInput = PlayerInput.GetPlayerByIndex(0);
        var mousePosInput = SceneObject<InputStandards>.Instance().MousePosInput.ActionName;
        _mousePos = playerInput.actions[mousePosInput];
        _uiCamera = SceneObject<UICamera>.Instance().Camera;
    }

    private void OnEnable()
    {
        PostUpdate.Machinery.AddBasicMachine(HandleFollow());
    }

    private IEnumerable<IEnumerable<Action>> HandleFollow()
    {
        while (isActiveAndEnabled)
        {
            var mousePos = _mousePos.ReadValue<Vector2>();
            var worldPos = _uiCamera.ScreenToWorldPoint(mousePos);

            transform.position = new Vector3(worldPos.x, worldPos.y, 0);

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
