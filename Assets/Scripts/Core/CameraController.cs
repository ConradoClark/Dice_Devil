using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : BaseUIObject
{
    public float MinZoomIn;
    public float MaxZoomOut;

    public float CameraSpeed;
    public Camera GameCamera;
    public ScriptBasicMachinery PostUpdateMachinery;

    public ScriptInput HorizontalMove;
    public ScriptInput VerticalMove;
    public ScriptInput RightMouseClick;
    public ScriptInput MouseScroll;

    private InputAction _horizontalMove;
    private InputAction _verticalMove;
    private InputAction _rightMouseClick;
    private InputAction _mouseScroll;

    private Vector3 _currentVelocity;
    private float _currentZoom;

    protected override void OnAwake()
    {
        base.OnAwake();
        var playerInput = PlayerInput.GetPlayerByIndex(0);
        _horizontalMove = playerInput.actions[HorizontalMove.ActionName];
        _verticalMove = playerInput.actions[VerticalMove.ActionName];
        _rightMouseClick = playerInput.actions[RightMouseClick.ActionName];
        _mouseScroll = playerInput.actions[MouseScroll.ActionName];

    }

    private void OnEnable()
    {
        PostUpdateMachinery.Machinery.AddBasicMachine(HandleKeysMovement());
        PostUpdateMachinery.Machinery.AddBasicMachine(HandleZoom());
    }

    private IEnumerable<IEnumerable<Action>> HandleZoom()
    {
        var targetZoom = GameCamera.orthographicSize;
        while (isActiveAndEnabled)
        {
            var zoom = _mouseScroll.ReadValue<float>();

            targetZoom += zoom * 0.01f;
            targetZoom = Mathf.Clamp(targetZoom, MinZoomIn, MaxZoomOut);

            var result = Mathf.SmoothDamp(GameCamera.orthographicSize, targetZoom, ref _currentZoom, 0.5f);
            GameCamera.orthographicSize = result;

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleKeysMovement()
    {
        while (isActiveAndEnabled)
        {
            var movement = new Vector2(_horizontalMove.ReadValue<float>(), _verticalMove.ReadValue<float>());

            if (movement.magnitude <= 0f)
            {
                yield return TimeYields.WaitOneFrameX;
                continue;
            }

            var translation =  CameraSpeed * movement * 0.01f;
            var pos = Vector3.SmoothDamp(GameCamera.transform.position, GameCamera.transform.position + (Vector3) translation,
                ref _currentVelocity, (float)UITimer.UpdatedTimeInMilliseconds * 0.01f);

            GameCamera.transform.position = new Vector3(pos.x,pos.y, GameCamera.transform.position.z);

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleRightClickDrag()
    {
        yield return TimeYields.WaitOneFrameX;
    }
}
