using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ConveyorBelt : MonoBehaviour
{
    public enum Facing
    {
        up,
        down,
        left,
        right
    }
    public Facing facing;
    [SerializeField] private float force;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();

        if (facing == Facing.up || facing == Facing.right)
        {
            animator.SetFloat("Direction", 1);
        }
        else
        {
            animator.SetFloat("Direction", -1);
        }
    }

    public void Push(Rigidbody2D rb, PlayerController controller)
    {
        switch (facing)
        {
            case Facing.up:
                rb.AddForce(Vector2.up * force, ForceMode2D.Force);
                controller.wallSlideSpeed = 1;
                break;
            case Facing.down:
                rb.AddForce(Vector2.down * force, ForceMode2D.Force);
                controller.wallSlideSpeed = -3;
                break;
            case Facing.left:
                rb.AddForce(Vector2.left * force, ForceMode2D.Force);
                break;
            case Facing.right:                
                rb.AddForce(Vector2.right * force, ForceMode2D.Force);
                break;
            default:
                break;
        }
    }
}
