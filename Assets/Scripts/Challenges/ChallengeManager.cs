using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Challenges;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using UnityEditor;

public class ChallengeManager : BaseGameObject
{
    public int ChallengeExpirationInSeconds;
    public int MaxSimultaneousChallenges;
    public BaseChallenge[] PossibleChallenges;
    public List<BaseChallenge> ActiveChallenges;
    public event Action<BaseChallenge> OnChallengeStart;
    public event Action<BaseChallenge> OnChallengeFail;
    public event Action<BaseChallenge> OnChallengeSuccess;


    private GameTileMap _gameTileMap;

    protected override void OnAwake()
    {
        base.OnAwake();
        ActiveChallenges = new List<BaseChallenge>();
        _gameTileMap = SceneObject<GameTileMap>.Instance();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(HandleChallenges());
        DefaultMachinery.AddBasicMachine(HandleChallengeResult());
        _gameTileMap.OnTilesChanged += OnTilesChanged;
    }


    private void OnDisable()
    {
        _gameTileMap.OnTilesChanged -= OnTilesChanged;
    }

    private void OnTilesChanged(BaseTile obj)
    {
        foreach (var challenge in ActiveChallenges)
        {
            challenge.CheckRequirements();
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleChallengeResult()
    {
        var markedForRemoval = new List<BaseChallenge>();
        while (isActiveAndEnabled)
        {
            markedForRemoval.Clear();
            foreach (var challenge in ActiveChallenges)
            {
                var requirementsCompleted = challenge.CheckRequirements();
                if (challenge.IsExpired && !requirementsCompleted)
                {
                    OnChallengeFail?.Invoke(challenge);
                    markedForRemoval.Add(challenge);
                }
                else if (requirementsCompleted && GameTimer.TotalElapsedTimeInMilliseconds - challenge.StartTime > 2000)
                {
                    OnChallengeSuccess?.Invoke(challenge);
                    markedForRemoval.Add(challenge);
                }
            }

            foreach (var challenge in markedForRemoval)
            {
                ActiveChallenges.Remove(challenge);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleChallenges()
    {
        while (isActiveAndEnabled)
        {
            yield return TimeYields.WaitOneFrameX;


            if (ActiveChallenges.Count < MaxSimultaneousChallenges)
            {
                var possibleChallenges = PossibleChallenges.Except(ActiveChallenges).ToArray();
                var chosenChallenge = possibleChallenges[UnityEngine.Random.Range(0, possibleChallenges.Length)];
                chosenChallenge.StartTimer();
                ActiveChallenges.Add(chosenChallenge);

                OnChallengeStart?.Invoke(chosenChallenge);
            }

            yield return TimeYields.WaitSeconds(GameTimer, 30);
        }
    }
}
