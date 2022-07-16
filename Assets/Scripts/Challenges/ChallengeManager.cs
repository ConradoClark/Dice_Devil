using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Challenges;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

public class ChallengeManager : BaseGameObject
{
    public int ChallengeExpirationInSeconds;
    public int MaxSimultaneousChallenges;
    public BaseChallenge[] PossibleChallenges;
    public List<BaseChallenge> ActiveChallenges;
    public event Action<BaseChallenge> OnChallengeStart;
    
    
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

    private IEnumerable<IEnumerable<Action>> HandleChallenges()
    {
        while (isActiveAndEnabled)
        {
            yield return TimeYields.WaitOneFrameX;


            if (ActiveChallenges.Count < MaxSimultaneousChallenges)
            {
                var chosenChallenge = PossibleChallenges[UnityEngine.Random.Range(0, PossibleChallenges.Length)];
                ActiveChallenges.Add(chosenChallenge);

                OnChallengeStart?.Invoke(chosenChallenge);
            }

            yield return TimeYields.WaitSeconds(GameTimer, 30);
        }
    }
}
