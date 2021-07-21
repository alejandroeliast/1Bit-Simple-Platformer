using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour, PlayerControls.IGameplayActions
{
    #region Movement Variables
    [Header("Movement")]    
    [SerializeField] [Range(0, 1)] float groundedDampingBasic = 0.5f;
    [SerializeField] [Range(0, 1)] float groundedDampingWhenStopping = 0.5f;
    [SerializeField] [Range(0, 1)] float groundedDampingWhenTurning = 0.5f;
    [HideInInspector] public Vector2 movement;
    private bool isFacingRight = true;
    #endregion

    #region Jump Variables
    [Header("Jump")]
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private int jumpAmountDefault;
    [Space]
    [SerializeField] [Range(0, 1)] float airDampingBasic = 0.5f;
    [SerializeField] [Range(0, 1)] float airDampingWhenStopping = 0.5f;
    [SerializeField] [Range(0, 1)] float airDampingWhenTurning = 0.5f;
    [Space]
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] [Range(0, 1)] float jumpCutHeight = 0.5f;
    [Space]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize;
    private bool wasGrounded;
    private bool isGrounded;
    private bool canJump = true;
    [HideInInspector] public int jumpAmount;
    #endregion

    #region Wall Slide Variables
    [Header("Wall Slide")]
    [SerializeField] public float wallSlideSpeed = 0;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private Transform wallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize;
    public bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallHugging;
    private bool canWallSlide = true;
    #endregion

    #region Wall Jump Variables
    [Header("Wall Jump")]
    [SerializeField] private float wallJumpForce = 10f;
    [SerializeField] private float wallJumpDirection = -1f;
    [SerializeField] private Vector2 wallJumpAngle;
    #endregion

    #region Wall Grab Variables
    [Header("Wall Grab")]
    [SerializeField] private float wallGrabDuration = 1f;
    private bool isWallGrabbing = false;
    private bool canWallGrab = false;
    private bool isWallGrabDisabled = false;
    #endregion

    [Header("Wall Climb")]
    [SerializeField] private float wallClimbSpeed = 2.5f;
    private bool canWallClimb = true;

    [Header("Dash")]
    [SerializeField] private float dashSpeed = 10f;
    [SerializeField] private float dashDuration = 0.25f;
    [SerializeField] private int dashAmountDefault;
    [SerializeField] private GameObject dashParticle;
    private Vector3 dashDirection;
    public bool canDash = true;
    public bool isDashing = false;
    [HideInInspector] public int dashAmount;

    [Header("Timers")]
    [SerializeField] float jumpPressedRememberTime = 0.2f;
    [SerializeField] float groundedRememberTime = 0.25f;
    [SerializeField] float wallTouchRememberTime = 0.25f;
    private float jumpPressedRemember = 0;
    private float groundedRemember = 0;
    private float wallTouchRemember = 0;

    private Transform checkpoint;

    [Header("Misc")]
    List<Refill> refills = new List<Refill>();
    public event GroundedDelegate OnGroundEvent;
    public delegate void GroundedDelegate(bool b);

    private Rigidbody2D rb;
    private PlayerControls playerControls;

    [HideInInspector] public BoxCollider2D collider;

    private OnOffManager onOffManager;
    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        collider = GetComponent<BoxCollider2D>();

        playerControls = new PlayerControls();
        playerControls.Gameplay.SetCallbacks(this);

        onOffManager = FindObjectOfType<OnOffManager>();

        checkpoint = GameObject.FindGameObjectWithTag("Start Point").transform;
        
        //jumpAmount = jumpAmountDefault;
    }

    private void Start()
    {
        refills.Clear();
        foreach (var item in GameObject.FindGameObjectsWithTag("Refill"))
        {
            refills.Add(item.GetComponent<Refill>());
        }
    }

    private void PlayerController_OnGroundedChanged(bool b)
    {
        Debug.Log(b);
    }

    private void OnEnable()
    {
        playerControls.Enable();
        OnGroundEvent += PlayerController_OnGroundedChanged;
    }
    private void OnDisable()
    {
        playerControls.Disable();
        OnGroundEvent -= PlayerController_OnGroundedChanged;
    }

    private void Update()
    {
        CheckState();

        groundedRemember -= Time.deltaTime;
        if (isGrounded)
        {
            groundedRemember = groundedRememberTime;
        }

        wallTouchRemember -= Time.deltaTime;
        if (isTouchingWall)
        {
            wallTouchRemember = wallTouchRememberTime;
        }

        jumpPressedRemember -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if (isGrounded && !isWallGrabbing)
        {
            float fHorizontalVelocity = rb.velocity.x;
            fHorizontalVelocity += movement.x;

            if (Mathf.Abs(movement.x) < 0.01f)
                fHorizontalVelocity *= Mathf.Pow(1f - groundedDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(movement.x) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - groundedDampingWhenTurning, Time.deltaTime * 10f);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - groundedDampingBasic, Time.deltaTime * 10f);

            rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);
        }
        else if (!isGrounded)
        {
            float fHorizontalVelocity = rb.velocity.x;
            fHorizontalVelocity += movement.x;

            if (Mathf.Abs(movement.x) < 0.01f)
                fHorizontalVelocity *= Mathf.Pow(1f - airDampingWhenStopping, Time.deltaTime * 10f);
            else if (Mathf.Sign(movement.x) != Mathf.Sign(fHorizontalVelocity))
                fHorizontalVelocity *= Mathf.Pow(1f - airDampingWhenTurning, Time.deltaTime * 10f);
            else
                fHorizontalVelocity *= Mathf.Pow(1f - airDampingBasic, Time.deltaTime * 10f);

            rb.velocity = new Vector2(fHorizontalVelocity, rb.velocity.y);
        }

        JumpTweaks();
        WallSlide();
        WallClimb();
    }

    private void CheckState()
    {
        // Grounded
        isGrounded = Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);

        if (isGrounded != wasGrounded)
        {
            wasGrounded = isGrounded;
            OnGroundEvent?.Invoke(isGrounded);
        }

        if (isGrounded )
        {

        }

        if (isGrounded)
        {
            isWallGrabDisabled = false;
        }
        // Wall Touch
        isTouchingWall = Physics2D.OverlapBox(wallCheckPoint.position, wallCheckSize, 0, wallLayer);

        // Wall Hug
        if (isTouchingWall && isGrounded && ((!isFacingRight && movement.x < 0) || (isFacingRight && movement.x > 0)))
        {
            isWallHugging = true;
        }
        else
        {
            isWallHugging = false;
        }

        if (isGrounded || isTouchingWall || isWallSliding)
        {
            jumpAmount = jumpAmountDefault;
            dashAmount = dashAmountDefault;
            foreach (var item in refills)
            {
                item.ResetState();
            }
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        movement = context.ReadValue<Vector2>();

        if (movement.x < 0 && isFacingRight)
        {
            Flip();
        }
        else if (movement.x > 0 && !isFacingRight)
        {
            Flip();
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressedRemember = jumpPressedRememberTime;
            Jump();
        }
        else if (context.canceled)
        {
            if (rb.velocity.y > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutHeight);
            }
        }

        if (isWallGrabbing)
        {
            WallGrabStop();
        }
    }
    void Jump()
    {
        if (canJump)
        {
            if (jumpPressedRemember > 0 && groundedRemember > 0)
            {
                jumpPressedRemember = 0;
                groundedRemember = 0;
                if (isTouchingWall)
                {
                    if (isWallHugging)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    }
                    else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    }
                }
                else
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                }
            }

            if (jumpPressedRemember > 0 && wallTouchRemember > 0)
            {
                jumpPressedRemember = 0;
                wallTouchRemember = 0;
                if (!isGrounded && (isWallSliding || isTouchingWall))
                {
                    //rb.velocity = Vector2.zero;
                    rb.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
                }
                else if (!isGrounded)
                {
                    rb.AddForce(new Vector2(wallJumpForce * wallJumpDirection * wallJumpAngle.x * -1, wallJumpForce * wallJumpAngle.y), ForceMode2D.Impulse);
                }
            }

            if (!isGrounded && wallTouchRemember <= 0 && groundedRemember <= 0)
            {
                if (jumpAmount > 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    jumpAmount--;
                }
            }
        }
    }
    void JumpTweaks()
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier);
        }
        //else if (rb.velocity.y > 0 && !isJumpKeyDown)
        //{
        //    rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier);
        //}
    }

    public void OnWallGrab(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            WallGrabStart();
        }
        else if (context.canceled)
        {
            WallGrabStop();
        }

    }
    private void WallGrabStart()
    {
        if (isTouchingWall)
        {
            isWallGrabbing = true;
            canWallSlide = false;
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
        }
    }
    private void WallGrabStop()
    {
        isWallGrabbing = false;
        canWallSlide = true;
        rb.gravityScale = 3;
    }
    private void WallClimb()
    {
        if (canWallClimb)
        {
            if (isWallGrabbing && isTouchingWall)
            {
                rb.velocity = new Vector2(rb.velocity.x, movement.y * wallClimbSpeed);
            }
        }
    }
    private void WallSlide()
    {
        if (canWallSlide)
        {
            if (isTouchingWall && !isGrounded && rb.velocity.y < 0)
            {
                if ((!isFacingRight && movement.x < 0) || (isFacingRight && movement.x > 0))
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

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Dash();
        }
    }
    private void Dash()
    {
        if (canDash)
        {
            if (dashAmount == 0 && (isGrounded || isTouchingWall || isWallSliding))
            {
                dashDirection = new Vector3(movement.x, movement.y, 0);

                dashDirection.Normalize();
                rb.velocity = Vector2.zero;

                Vector3 currentPos = transform.position;
                Vector3 targetPos = currentPos + dashDirection * dashSpeed / 3;

                for (int i = 0; i < 3; i++)
                {
                    Instantiate(dashParticle, currentPos, new Quaternion(0, 0, 0, 0));
                    if (currentPos != targetPos)
                    {
                        rb.MovePosition(transform.position + dashDirection * dashSpeed);
                    }
                    currentPos = targetPos;
                    targetPos = currentPos + dashDirection * dashSpeed / 3;
                }

                rb.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);

                dashAmount--;
            }
            else if (dashAmount > 0)
            {
                dashDirection = new Vector3(movement.x, movement.y, 0);

                dashDirection.Normalize();
                rb.velocity = Vector2.zero;

                Vector3 currentPos = transform.position;
                Vector3 targetPos = currentPos + dashDirection * dashSpeed / 3;

                for (int i = 0; i < 3; i++)
                {
                    Instantiate(dashParticle, currentPos, new Quaternion(0, 0, 0, 0));
                    if (currentPos != targetPos)
                    {
                        rb.MovePosition(transform.position + dashDirection * dashSpeed);
                    }
                    currentPos = targetPos;
                    targetPos = currentPos + dashDirection * dashSpeed / 3;
                }

                rb.AddForce(dashDirection * dashSpeed, ForceMode2D.Impulse);

                dashAmount--;
            }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IInteractable interactable = collision.GetComponent<IInteractable>();
        if(interactable != null)
        {
            interactable.Interact(gameObject);
        }

        switch (collision.tag)
        {
            case "Spikes":

                rb.velocity = Vector2.zero;
                transform.position = checkpoint.position;
                break;
            case "On Off Switch":
                onOffManager.ReverseState();
                break;
            case "Checkpoint":
                checkpoint = collision.transform;
                break;
            case "Death Bound":
                rb.velocity = Vector2.zero;
                transform.position = checkpoint.position;
                break;
            case "Diamond":
                collision.GetComponent<DiamondPickUp>().Collected();
                break;
            case "Goal":
                SceneManager.LoadScene(0);
                break;
            default:
                break;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Conveyor Belt":
                collision.GetComponent<ConveyorBelt>().Push(rb, this);
                break;
            default:
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (collision.tag)
        {
            case "Conveyor Belt":
                wallSlideSpeed = -0.5f;
                break;
            default:
                break;
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

    public IEnumerator DisableHitSpring()
    {
        yield return new WaitForSeconds(0.25f);
    }


    

    public class OnGroundedChangedEventArgs : EventArgs
    {
        public bool IsGrounded { get; set; }
    }
}
