using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffSwitch : OnOff
{
    protected override void Awake()
    {
        base.Awake();
        UpdateSprite();
    }

    public override void UpdateSprite()
    {
        base.UpdateSprite();
        switch (type)
        {
            case Type.Off:
                spriteRenderer.sprite = sprites[0];
                break;
            case Type.On:
                spriteRenderer.sprite = sprites[1];
                break;
            default:
                break;
        }
    }
}
