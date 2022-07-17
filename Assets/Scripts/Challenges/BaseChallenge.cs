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

        public abstract bool CheckRequirements();

        public abstract void StartTimer();

        public abstract IEnumerable<IEnumerable<Action>> Start();

        public abstract IEnumerable<IEnumerable<Action>> Success();

        public abstract IEnumerable<IEnumerable<Action>> Failure(); 

        public abstract bool IsExpired { get; }
    }
}
