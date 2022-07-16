using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

public class Cursor : BaseUIObject
{
    public SpriteRenderer SpriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        SpriteRenderer.sprite = sprite;
    }
}
