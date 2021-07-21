using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Refill : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite spriteActive;
    [SerializeField] private Sprite spriteInactive;

    SpriteRenderer spriteRenderer;
    bool hasBeenPickedUp = false;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    public void MoveRefill(PlayerController controller)
    {
        if (!hasBeenPickedUp)
        {
            spriteRenderer.sprite = spriteInactive;
            controller.jumpAmount++;
            controller.dashAmount++;
            hasBeenPickedUp = true;
        }
    }

    public void ResetState()
    {
        spriteRenderer.sprite = spriteActive;
        hasBeenPickedUp = false;
    }

    public void Interact(GameObject interact)
    {
        PlayerController controller = interact.GetComponent<PlayerController>();
        MoveRefill(controller);
    }
}
