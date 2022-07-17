using System;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Challenges
{
    public abstract class BaseChallenge : BaseUIObject
    {
        public float StartTime { get; protected set; }
        protected GameTileMap GameTileMap;
        protected override void OnAwake()
        {
            base.OnAwake();
            GameTileMap = SceneObject<GameTileMap>.Instance();
        }

        public abstract bool CheckRequirements(bool markAsChecked=true);

        public abstract void StartTimer();

        public abstract IEnumerable<IEnumerable<Action>> Start();

        public abstract IEnumerable<IEnumerable<Action>> Success();

        public abstract IEnumerable<IEnumerable<Action>> Failure();

        public abstract IEnumerable<IEnumerable<Action>> Move(float posY);

        public abstract void Progress();

        public abstract bool IsExpired { get; }

        public bool CompletedOnFirstCheck { get; protected set; }

        public abstract int Score { get; }
    }
}
