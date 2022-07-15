using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using TMPro;
using UnityEngine;

public class DayCounter : TimeUIObject
{
    public TMP_Text TextComponent;
    private float _internalTimeCounterMs;
    private void OnEnable()
    {
        _internalTimeCounterMs = 0;
        if (TimeSettings.SecondsToADay > 0) DefaultMachinery.AddBasicMachine(HandleCounter());
    }

    private IEnumerable<IEnumerable<Action>> HandleCounter()
    {
        while (isActiveAndEnabled)
        {
            _internalTimeCounterMs += (float)GameTimer.UpdatedTimeInMilliseconds;
            TextComponent.text = Mathf.FloorToInt(_internalTimeCounterMs * 0.001f / TimeSettings.SecondsToADay).ToString();

            yield return TimeYields.WaitOneFrameX;
        }
    }
}
