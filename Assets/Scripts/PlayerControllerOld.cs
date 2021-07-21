using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerControllerOld : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float moveAirSpeed = 10f;
    private float inputX;
    private bool isFacingRight = true;
    private bool isMoving;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;
    [SerializeField] private float jumpPressedRememberTime = 1f;
    private bool isGrounded;
    private bool canJump;
    public float jumpPressedRemember;

    [Header("Wall Slide")]
    [SerializeField] private float wallSlideSpeed = 0;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isTouchingWall;
    public bool isWallSliding;
    private bool canWallSlide = true;

    [Header("Wall Jump")]
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpDirection = -1f;
    [SerializeField] private Vector2 wallJumpAngle;

    [Header("Wall Grab")]
    [SerializeField] private float wallGrabDuration = 1f;
    private bool isWallGrabbing = false;
    private bool canWallGrab = false;
    private bool isWallGrabDisabled = false;

    [Header("Wall Climb")]
    [SerializeField] private float wallClimbSpeed = 2.5f;
    private float inputY;
    private bool canWallClimb = true;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.25f;
    private Vector3 dashDirection;
    public bool canDash = false;
    public bool isDashing = false;

    [Header("Misc")]
    [SerializeField] private float gravityScale;
    private Renderer renderer;
    private Rigidbody2D rb;
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody2D>();
        //wallJumpAngle.Normalize();
    }

    private void Update()
    {
        PlayerInput();
        CheckState();
    }

    private void FixedUpdate()
    {
        Movement();
        Jump();
        WallSlide();
        WallJump();
        //WallGrab();
        //WallClimb();
        //Dash();
    }

    private void PlayerInput()
    {
        //// Horizontal Movement
        //inputX = Input.GetAxisRaw("Horizontal");

        //// Vertical Movement
        //inputY = Input.GetAxisRaw("Vertical");

        //// Jumping
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    canJump = true;
        //    jumpPressedRemember = jumpPressedRememberTime;
        //}

        //// Wall Grab
        //if (Input.GetKey(KeyCode.LeftShift))
        //{
        //    if (!isWallGrabDisabled)
        //    {
        //        canWallGrab = true;
        //    }
        //}
        //else
        //{
        //    canWallGrab = false;
        //}

        //// Dash 
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    canDash = true;

        //    dashDirection = new Vector3(inputX, inputY, 0);
        //    dashDirection.Normalize();
        //    rb.MovePosition(transform.position + dashDirection * dashSpeed);
        //    rb.velocity = Vector2.zero;
        //    Debug.Log(dashDirection);
        //    rb.AddForce(dashDirection * 5, ForceMode2D.Impulse);
        //    canDash = false;
        //}
    }
    private void CheckState()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);
        if (isGrounded)
        {
            isWallGrabDisabled = false;
        }
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);
        if (isTouchingWall && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
    }

    private void Movement()
    {
        if (inputX != 0)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        if (!canDash)
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
            }
            else if (!isGrounded && !isWallSliding && inputX != 0)
            {
                rb.AddForce(new Vector2(moveAirSpeed * inputX, 0));
                if (Mathf.Abs(rb.velocity.x) > moveSpeed)
                {
                    rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
                }
            }
        }

        if (inputX < 0 && isFacingRight)
        {
            Flip();
        }
        else if (inputX > 0 && !isFacingRight)
        {
            Flip();
        }
    }
    private void Flip()
    {
        if (!isWallSliding)
        {
            wallJumpDirection *= -1;
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180, 0);
        }
    }
    private void Jump()
    {
        if (canJump)
        {
            jumpPressedRemember -= Time.deltaTime;
            if ((jumpPressedRemember > 0) && isGrounded)
            {
                jumpPressedRemember = 0;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canJump = false;
            }
            else if (!isGrounded && !isTouchingWall)
            {
                canJump = false;
            }

            if (rb.velocity.y < 0 && !isDashing)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier);
            }
            else if (rb.velocity.y > 0 && !Input.GetKey(KeyCode.Space) && !isDashing)
            {
                rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier);
            }
        }

    }
    private void WallSlide()
    {
        if (canWallSlide)
        {
            if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
            {
                if ((!isFacingRight && inputX < 0) || (isFacingRight && inputX > 0))
                {
                    isWallSliding = true;
                }
                else
                {
                    isWallSliding = false;
                }
            }
            else
            {
                isWallSliding = false;
            }

            if (isWallSliding)
            {
                rb.velocity = new Vector2(rb.velocity.x, wallSlideSpeed);
            }
        }
    }
    private void WallJump()
    {
        if ((isWallSliding || isTouchingWall) && canJump)
        {
            rb.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
            canJump = false;
        }
    }
    private void WallGrab()
    {
        if (canWallGrab)
        {
            if (isTouchingWall && !isGrounded)
            {
                isWallGrabbing = true;
            }
            else
            {
                isWallGrabbing = false;
            }

            if (isWallGrabbing)
            {
                canWallSlide = false;

                rb.velocity = new Vector2(0, 0);
                rb.gravityScale = 0;

                StartCoroutine(DisableWallGrab(wallGrabDuration));
            }

        }
        if (!isWallGrabbing && rb.gravityScale != gravityScale)
        {
            canWallSlide = true;
            isWallGrabbing = false;

            rb.gravityScale = gravityScale;
        }
    }
    private void WallClimb()
    {
        if (isWallGrabbing && canWallClimb)
        {
            rb.velocity = new Vector2(rb.velocity.x, inputY * wallClimbSpeed);
        }
    }

    private void Dash()
    {
        if (canDash)
        {
            dashDirection = new Vector3(inputX, inputY, 0);

            dashDirection.Normalize();
            //            rb.MovePosition(transform.position + dashDirection * dashSpeed);
            rb.velocity = Vector2.zero;
            Debug.Log(dashDirection);
            rb.AddForce(dashDirection * dashSpeed * 10, ForceMode2D.Impulse);
            canDash = !canDash;
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Ground Check
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(groundCheckPoint.position, groundCheckSize);

        // Wall Check
        Gizmos.color = Color.red;
        Gizmos.DrawCube(wallCheckPoint.position, wallCheckSize);
    }

    //private IEnumerator Dash(Vector2 dir)
    //{
    //    if (dir.x == 0 && dir.y == 0)
    //    {
    //        yield break;
    //    }

    //    if (canDash)
    //    {
    //        isDashing = true;
    //        canDash = false;


    //        dir.Normalize();
    //        Debug.Log(dir);
    //        rb.velocity = Vector2.zero;
    //        rb.AddForce(new Vector2(dashSpeed * dir.x, dashSpeed * dir.y), ForceMode2D.Impulse);
    //        rb.gravityScale = 0;
    //        yield return new WaitForSeconds(dashDuration);


    //        rb.velocity = Vector2.zero;
    //        isDashing = false;
    //        canDash = true;
    //        rb.gravityScale = gravityScale;
    //    }

    //}
    private IEnumerator DisableWallGrab(float duration)
    {
        if (!isWallGrabbing)
        {
            Debug.Log("Break");
            yield break;
        }
        yield return new WaitForSeconds(duration);
        if (isWallGrabbing)
        {
            isWallGrabDisabled = true;
            canWallGrab = false;
        }
    }

}
