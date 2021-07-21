using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffBlock : OnOff
{
    protected override void Awake()
    {
        base.Awake();

        UpdateCollider();
        UpdateSprite();
    }
    public override void UpdateCollider()
    {
        base.UpdateCollider();
        collider.enabled = isOn;
    }

    public override void UpdateSprite()
    {
        base.UpdateSprite();
        switch (type)
        {
            case Type.Off:
                if (isOn)
                    spriteRenderer.sprite = sprites[0];
                else
                    spriteRenderer.sprite = sprites[1];
                break;
            case Type.On:
                if (isOn)
                    spriteRenderer.sprite = sprites[2];
                else
                    spriteRenderer.sprite = sprites[3];
                break;
            default:
                break;
        }
    }
}
