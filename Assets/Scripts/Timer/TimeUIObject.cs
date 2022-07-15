using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;
public class TimeUIObject : BaseUIObject
{
    protected TimeSettings TimeSettings;
    protected ITimer GameTimer;
    protected override void OnAwake()
    {
        base.OnAwake();
        TimeSettings = SceneObject<TimeSettings>.Instance();
        GameTimer = SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;
    }
}



