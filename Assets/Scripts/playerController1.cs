using System.Collections;
using UnityEngine;

public class PlayerController1 : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [Header("Dashing")]
    [SerializeField] private float dashVelocity = 14f;
    [SerializeField] private float dashingTime = 0.5f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private string dashingLayer = "PlayerDashing";
    [SerializeField] private float attackDuration = 0.4f;

    [Header("Slide Settings")]
    [SerializeField] private float slideDeceleration = 2f;
    [SerializeField] private float minSlideSpeed = 0.1f;

    [Header("Wall Jump Settings")]
    [SerializeField] private Transform wallCheck;
    [SerializeField] private float wallSlideSpeed = 2f;
    [SerializeField] private float wallJumpForce = 12f;
    [SerializeField] private Vector2 wallJumpDirection = new Vector2(1, 1.5f);
    [SerializeField] private float wallJumpTime = 0.2f;

    [Header("Double Jump Settings")]
    [SerializeField] private int maxExtraJumps = 1; // Số lần nhảy thêm
    [SerializeField] private float doubleJumpForce = 10f; // Lực nhảy lần 2
    private int extraJumps; // Số lượt nhảy còn lại

    private Vector2 dashingDirection;
    private bool isDashing;
    private bool canDash = true;
    private bool isDashCooldown;
    private int originalLayer;
    public bool canMove = true;

    public bool isGrounded;
    private Animator animator;
    private Rigidbody2D rb;
    private TrailRenderer trailRenderer;
    private PlayerAttack playerAttack;

    private float coyoteTime = 0.2f;
    private float coyoteTimeCounter;
    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    private bool isAttacking = false;
    private bool isSliding;
    private float slideSpeed;

    private bool isWallSliding = false;
    private bool isWallJumping = false;

    private void Awake()
    {
        extraJumps = maxExtraJumps; // Khởi tạo lượt nhảy
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        trailRenderer = GetComponent<TrailRenderer>();
        originalLayer = gameObject.layer;
        playerAttack = GetComponent<PlayerAttack>();
    }

    void Update()
    {
        if (!canMove) return;
        HandleMovement();
        HandleJump();
        HandleWallSlide();
        HandleWallJump();
        UpdateAnimation();
        HandleDashing();
    }

    private void HandleMovement()
    {
        if (!canMove || isDashing || isWallSliding) return;

        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput != 0)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
            isSliding = false;
        }
        else if (isGrounded && Mathf.Abs(rb.linearVelocity.x) > minSlideSpeed)
        {
            isSliding = true;
            slideSpeed = rb.linearVelocity.x;
        }

        if (isSliding)
        {
            ApplySlide();
        }

        if (moveInput > 0) transform.localScale = Vector3.one;
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

    private void ApplySlide()
    {
        if (isGrounded)
        {
            slideSpeed = Mathf.MoveTowards(slideSpeed, 0, slideDeceleration * Time.deltaTime);
            rb.linearVelocity = new Vector2(slideSpeed, rb.linearVelocity.y);

            if (Mathf.Abs(slideSpeed) <= minSlideSpeed)
            {
                isSliding = false;
            }
        }
    }

    private void HandleJump()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            extraJumps = maxExtraJumps; // Thêm dòng này
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if ((coyoteTimeCounter > 0f || extraJumps > 0) && jumpBufferCounter > 0f)
        {
            // Kiểm tra có phải nhảy thêm không
            float jumpPower = isGrounded ? jumpForce : doubleJumpForce;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
            if (!isGrounded)
            {
                extraJumps--;
            }
            jumpBufferCounter = 0f;
        }


        if (Input.GetButtonUp("Jump") && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
            coyoteTimeCounter = 0f;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void HandleWallSlide()
    {
        bool isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, 0.2f, groundLayer);

        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, -wallSlideSpeed, float.MaxValue));

            if (playerAttack != null)
            {
                playerAttack.enabled = false;
            }
        }
        else
        {
            isWallSliding = false;

            if (playerAttack != null)
            {
                playerAttack.enabled = true;
            }
        }
    }

    private void HandleWallJump()
    {
        if (isWallSliding && Input.GetButtonDown("Jump"))
        {
            isWallJumping = true;

            float jumpDirectionX = -transform.localScale.x * wallJumpDirection.x;
            Vector2 jumpDirection = new Vector2(jumpDirectionX, wallJumpDirection.y);

            rb.linearVelocity = jumpDirection * wallJumpForce;
            transform.localScale = new Vector3(Mathf.Sign(jumpDirectionX), 1, 1);

            Invoke(nameof(ResetWallJump), wallJumpTime);
            extraJumps = maxExtraJumps; 
        }
    }

    private void ResetWallJump()
    {
        isWallJumping = false;
    }

    private void HandleDashing()
{
    if (isAttacking || isWallSliding) return;

    bool dashInput = Input.GetButtonDown("Dash");

    if (dashInput && canDash && !isDashCooldown)
    {
        gameObject.layer = LayerMask.NameToLayer(dashingLayer);
        isDashing = true;
        canDash = false;
        isDashCooldown = true;
        trailRenderer.emitting = true;

        // Chỉ lấy hướng ngang, set hướng dọc về 0
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        dashingDirection = new Vector2(
            horizontalInput,
            0f // Luôn set trục Y = 0
        );

        // Nếu không có input ngang, dash theo hướng mặt
        if (dashingDirection == Vector2.zero)
        {
            dashingDirection = new Vector2(transform.localScale.x, 0);
        }

        StartCoroutine(StopDashing());
        StartCoroutine(DashCooldown());
    }

    if (isDashing)
    {
        // Đảm bảo chỉ di chuyển ngang
        rb.linearVelocity = dashingDirection.normalized * dashVelocity;
        return;
    }

    if (isGrounded)
    {
        canDash = true;
    }
}

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashingTime);
        isDashing = false;
        trailRenderer.emitting = false;
        gameObject.layer = originalLayer;

        rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.5f, rb.linearVelocity.y);
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        isDashCooldown = false;
    }

    private void UpdateAnimation()
    {
        bool isRunning = Mathf.Abs(rb.linearVelocity.x) > 0.1f;
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsDashing", isDashing);
        animator.SetBool("IsWallSliding", isWallSliding);
    }

    public IEnumerator LockMovementDuringAttack()
    {
        bool wasGrounded = isGrounded;

        if (!wasGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.75f, rb.linearVelocity.y);
        }

        if (wasGrounded)
        {
            canMove = false;
            rb.linearVelocity = Vector2.zero;
        }

        yield return new WaitForSeconds(attackDuration);

        if (wasGrounded)
        {
            canMove = true;
        }
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }

    public void SetAttacking(bool attacking)
    {
        if (attacking && isDashing)
            return;

        isAttacking = attacking;
    }

    public bool IsDashing()
    {
        return isDashing;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }
}