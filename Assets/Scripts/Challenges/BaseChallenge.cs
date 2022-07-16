using System;
using System.Collections.Generic;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Challenges
{
    public abstract class BaseChallenge : BaseUIObject
    {
        protected GameTileMap GameTileMap;
        protected override void OnAwake()
        {
            base.OnAwake();
            GameTileMap = SceneObject<GameTileMap>.Instance();
        }

        public abstract bool CheckRequirements();

        public abstract IEnumerable<IEnumerable<Action>> Start();

        public abstract bool IsExpired { get; }
    }
}
