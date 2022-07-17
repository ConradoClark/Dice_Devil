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
    public event Action<List<BaseChallenge>> OnChallengeListChanged;


    private BaseChallenge _lastChallenge;
    private GameTileMap _gameTileMap;
    private bool _skipWait;

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
            challenge.CheckRequirements(false);
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
                var isFirst = challenge.CompletedOnFirstCheck;
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

                    if (isFirst)
                    {
                        _skipWait = true;
                        // break sequence and give next challenge immediately
                    }
                }
            }

            foreach (var challenge in markedForRemoval)
            {
                ActiveChallenges.Remove(challenge);
            }

            if (markedForRemoval.Count > 0)
            {
                OnChallengeListChanged?.Invoke(ActiveChallenges);
            }

            yield return TimeYields.WaitOneFrameX;
        }
    }

    private IEnumerable<IEnumerable<Action>> HandleChallenges()
    {
        while (isActiveAndEnabled)
        {
            _skipWait = false;
            yield return TimeYields.WaitOneFrameX;

            if (ActiveChallenges.Count < MaxSimultaneousChallenges)
            {
                var possibleChallenges = PossibleChallenges.Except(ActiveChallenges.Concat(new[] { _lastChallenge }))
                    .Where(c => c != null)
                    .ToArray();

                var chosenChallenge = possibleChallenges[UnityEngine.Random.Range(0, possibleChallenges.Length)];
                chosenChallenge.StartTimer();
                ActiveChallenges.Add(chosenChallenge);
                _lastChallenge = chosenChallenge;

                OnChallengeStart?.Invoke(chosenChallenge);
            }

            yield return TimeYields.WaitSeconds(GameTimer, 30, breakCondition: () => _skipWait);

            if (_skipWait)
            {
                yield return TimeYields.WaitSeconds(GameTimer, 1);
            }
        }
    }
}
