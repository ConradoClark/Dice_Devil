using System;
using System.Collections.Generic;
using System.Linq;
using Licht.Impl.Orchestration;
using Licht.Interfaces.Time;
using Licht.Unity.Extensions;
using Licht.Unity.Objects;

namespace Assets.Scripts.Challenges
{
    public class TileTypeChallenge : BaseChallenge
    {
        public ScriptPrefab UIPrefab;
        public ScriptPrefab TimerPrefab;
        private TileTypeChallengeUIPool _uiPool;
        private ChallengeUI _challengeUI;
        private ChallengeManager _challengeManager;
        private TileTypeChallengeUI _currentUI;
        private ChallengeTimer _currentTimer;
        private ChallengeTimerPool _timer;
        private ITimer _gameTimer;

        protected override void OnAwake()
        {
            base.OnAwake();
            _gameTimer = SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;
            _uiPool = SceneObject<TileTypeChallengeUIPoolManager>.Instance().GetEffect(UIPrefab);
            _timer = SceneObject<ChallengeTimerPoolManager>.Instance().GetEffect(TimerPrefab);
            _challengeUI = SceneObject<ChallengeUI>.Instance();
            _challengeManager = SceneObject<ChallengeManager>.Instance();
        }

        public TileTypeRequirement Requirements;
        public override bool CheckRequirements()
        {
            var count = GameTileMap.Tiles.Count(t => t.Value.TileType == Requirements.TileType);
            UpdateProgress(count);
            return count >= Requirements.Amount;
        }

        public override void StartTimer()
        {
            StartTime = (float)_gameTimer.TotalElapsedTimeInMilliseconds;
        }

        private void UpdateProgress(int count)
        {
            if (_currentUI == null) return;
            _currentUI.ProgressText.text = $"<sprite={Requirements.TileType.AsSpriteGlyph()}> {count}/{Requirements.Amount}";
        }

        public override IEnumerable<IEnumerable<Action>> Start()
        {
            if (!_uiPool.TryGetFromPool(out var ui)) yield break;
            if (!_timer.TryGetFromPool(out _currentTimer)) yield break;

            _currentUI = ui;

            _currentTimer.SecondsToExpire = _challengeManager.ChallengeExpirationInSeconds;
            _currentTimer.ParentChallengeUI = _currentUI;
            _currentTimer.transform.SetParent(_currentUI.transform);
            _currentTimer.transform.localPosition = _challengeUI.ChallengeTimerOffset;
            _currentUI.ChallengeText.text = $"Build {Requirements.Amount} \"{Requirements.TileType.AsString()}\"";
            UpdateProgress(GameTileMap.Tiles.Count(t => t.Value.TileType == Requirements.TileType));

            _currentUI.transform.position = _challengeUI.GetChallengePosition();

            yield return ui.transform.GetAccessor()
                .Position
                .X
                .SetTarget(_challengeUI.TargetX)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.BounceEaseOut)
                .UsingTimer(UITimer)
                .Build();
        }

        private IEnumerable<IEnumerable<Action>> Hide()
        {
            yield return _currentUI.transform.GetAccessor()
                .Position
                .X
                .SetTarget(_challengeUI.StartingX)
                .Over(1f)
                .Easing(EasingYields.EasingFunction.BounceEaseOut)
                .UsingTimer(UITimer)
                .Build();

            _currentUI.Hide();
            _currentTimer.Hide();
            _currentUI = null;
        }

        public override IEnumerable<IEnumerable<Action>> Success()
        {
            yield return _currentUI.SetToSuccess().AsCoroutine();
            yield return Hide().AsCoroutine();
        }

        public override IEnumerable<IEnumerable<Action>> Failure()
        {
            yield return _currentUI.SetToFailure().AsCoroutine();
            yield return Hide().AsCoroutine();
        }

        public override bool IsExpired => _currentTimer?.IsExpired ?? false;
    }
}
