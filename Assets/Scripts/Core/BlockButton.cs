﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

namespace Assets.Scripts.Core
{
    public class BlockButton : BaseUIButton
    {
        public TokenCardSlot Slot;
        public override int ManaCost => 0;

        private TokenCardSleeve _tokenCardSleeve;

        protected override void OnAwake()
        {
            base.OnAwake();
            _tokenCardSleeve = SceneObject<TokenCardSleeve>.Instance();
        }

        protected override IEnumerable<IEnumerable<Action>> OnClick()
        {
            if (_tokenCardSleeve.TokenCardSlots.Count(slot => slot.IsBlocked) < 3)
            {
                Slot.ToggleBlock();
                yield return TimeYields.WaitSeconds(UITimer, 0.2f);
            }
        }
    }
}
