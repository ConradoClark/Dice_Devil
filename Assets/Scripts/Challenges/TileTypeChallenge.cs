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
        public int BaseScore;
        public int Growth;
        public int Level;

        public TileTypeRequirement Requirements;
        public ScriptPrefab UIPrefab;
        public ScriptPrefab TimerPrefab;
        private TileTypeChallengeUIPool _uiPool;
        private ChallengeUI _challengeUI;
        private ChallengeManager _challengeManager;
        private TileTypeChallengeUI _currentUI;
        private ChallengeTimer _currentTimer;
        private ChallengeTimerPool _timer;
        private ITimer _gameTimer;
        private bool _firstCheck;

        protected override void OnAwake()
        {
            base.OnAwake();
            _gameTimer = SceneObject<DefaultGameTimer>.Instance().TimerRef.Timer;
            _uiPool = SceneObject<TileTypeChallengeUIPoolManager>.Instance().GetEffect(UIPrefab);
            _timer = SceneObject<ChallengeTimerPoolManager>.Instance().GetEffect(TimerPrefab);
            _challengeUI = SceneObject<ChallengeUI>.Instance();
            _challengeManager = SceneObject<ChallengeManager>.Instance();
        }

        
        public override bool CheckRequirements(bool markAsChecked = true)
        {
            var count = GameTileMap.Tiles.Count(t => t.Value.TileType == Requirements.TileType);
            UpdateProgress(count);
            var completed = count >= Requirements.Amount;

            if (markAsChecked && _firstCheck)
            {
                CompletedOnFirstCheck = completed;
                _firstCheck = false;
            }

            return completed;
        }

        public override void StartTimer()
        {
            StartTime = (float)_gameTimer.TotalElapsedTimeInMilliseconds;
            _firstCheck = true;
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
            _currentTimer.transform.SetParent(_timer.transform);
            _currentTimer.Hide();
            _currentUI = null;
            _currentTimer = null;
        }

        public override IEnumerable<IEnumerable<Action>> Success()
        {
            if (_currentUI != null)
            {
                yield return _currentUI.SetToSuccess().AsCoroutine();
                yield return Hide().AsCoroutine();
            }

            Progress();
        }

        public override IEnumerable<IEnumerable<Action>> Failure()
        {
            yield return _currentUI.SetToFailure().AsCoroutine();
            yield return Hide().AsCoroutine();
        }

        public override IEnumerable<IEnumerable<Action>> Move(float posY)
        {
            if (_currentUI == null) yield break;

            yield return TimeYields.WaitSeconds(UITimer, 0.55f);

            yield return _currentUI.transform.GetAccessor()
                .Position
                .Y
                .SetTarget(posY)
                .Over(0.25f)
                .Easing(EasingYields.EasingFunction.QuadraticEaseOut)
                .UsingTimer(UITimer)
                .Build();
        }

        public override void Progress()
        {
            Level++;
            Requirements.Amount += Growth;
        }

        public override bool IsExpired => _currentTimer?.IsExpired ?? false;
        public override int Score => BaseScore * Level;
    }
}
