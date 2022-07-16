using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;

public class DrawManager : BaseGameObject
{
    private TokenCardSleeve _cardSleeve;
    protected override void OnAwake()
    {
        base.OnAwake();
        _cardSleeve = SceneObject<TokenCardSleeve>.Instance();
    }

    private void OnEnable()
    {
        DefaultMachinery.AddBasicMachine(_cardSleeve.Draw());
    }
}
