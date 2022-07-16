using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEngine;

public class ChallengeUI : BaseUIObject
{
    public float StartingX;
    public float TargetX;
    public float YOffset;
    public float DistanceBetweenChallenges;

    public Vector3 ChallengeTimerOffset;

    private ChallengeManager _challengeManager;
    protected override void OnAwake()
    {
        base.OnAwake();
        _challengeManager = SceneObject<ChallengeManager>.Instance();
    }

    private void OnEnable()
    {
        _challengeManager.OnChallengeStart += OnChallengeStart;
    }

    private void OnDisable()
    {
        _challengeManager.OnChallengeStart -= OnChallengeStart;
    }

    private void OnChallengeStart(Assets.Scripts.Challenges.BaseChallenge obj)
    {
        DefaultMachinery.AddBasicMachine(obj.Start());
    }

    public Vector3 GetChallengePosition()
    {
        return new Vector3(StartingX, transform.position.y + YOffset - DistanceBetweenChallenges * _challengeManager.ActiveChallenges.Count);
    }
}

