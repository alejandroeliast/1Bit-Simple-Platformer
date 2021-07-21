using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOff : MonoBehaviour
{
    public enum Type
    {
        Off,
        On
    }
    public Type type;

    [HideInInspector] public bool isOn = false;
    protected BoxCollider2D collider;

    [SerializeField] protected List<Sprite> sprites = new List<Sprite>();
    protected SpriteRenderer spriteRenderer;

    protected virtual void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    public virtual void UpdateCollider() { }
    public virtual void UpdateSprite() { }
}
