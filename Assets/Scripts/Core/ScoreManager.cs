using System;
using System.Collections;
using System.Collections.Generic;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using TMPro;

public class ScoreManager : BaseUIObject
{
    public TMP_Text ScoreText;
    public int TrailingZeros;

    public int Score { get; private set; }
    private int _currentShowingScore;
    private ChallengeManager _challengeManager;

    protected override void OnAwake()
    {
        base.OnAwake();
        _challengeManager = SceneObject<ChallengeManager>.Instance();
    }

    private void OnEnable()
    {
        _challengeManager.OnChallengeSuccess += OnChallengeSuccess;
        DefaultMachinery.AddBasicMachine(UpdateScore());
    }

    private void OnDisable()
    {
        _challengeManager.OnChallengeSuccess -= OnChallengeSuccess;
    }

    private void OnChallengeSuccess(Assets.Scripts.Challenges.BaseChallenge obj)
    {
        Score += obj.Score;
    }

    private IEnumerable<IEnumerable<Action>> UpdateScore()
    {
        while (isActiveAndEnabled)
        {
            if (_currentShowingScore < Score)
            {
                _currentShowingScore += Math.Min(Score - _currentShowingScore, 10);
            }

            ScoreText.text = _currentShowingScore.ToString().PadLeft(TrailingZeros, '0');

            yield return TimeYields.WaitMilliseconds(UITimer, 50);
        }
    }
}
