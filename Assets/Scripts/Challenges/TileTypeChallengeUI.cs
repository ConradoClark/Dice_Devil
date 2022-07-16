using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Pooling;
using TMPro;

public class TileTypeChallengeUI : EffectPoolable
{
    public TMP_Text ChallengeText;
    public TMP_Text ProgressText;

    public override void OnActivation()
    {
        
    }

    public override bool IsEffectOver { get; protected set; }
}
