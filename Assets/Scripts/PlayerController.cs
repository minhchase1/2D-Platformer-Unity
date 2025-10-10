using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 14f;
    public int maxJumps = 2;

    [Header("Dash")]
    public float dashSpeed = 18f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.6f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Wall")]
    public bool enableWallSlide = true;
    public float wallSlideSpeed = -2f;
    public float wallJumpHorizontal = 8f;
    public float wallJumpVertical = 14f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.12f;
    public Transform wallCheck;
    public float wallCheckDistance = 0.1f;

    [Header("Death Settings")]
    public float deathY = -10f;

    [Header("References")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private float moveInput;
    private bool isGrounded;
    private int jumpsLeft;
    private bool isFacingRight = true;
    private bool isWallSliding = false;
    private bool isWallAdjacent = false;
    private float lastDashTime = -999f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        jumpsLeft = maxJumps;

        if (groundCheck == null)
        {
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0, -1f, 0);
            groundCheck = gc.transform;
        }

        if (wallCheck == null)
        {
            GameObject wc = new GameObject("WallCheck");
            wc.transform.SetParent(transform);
            wc.transform.localPosition = new Vector3(0.5f, 0, 0);
            wallCheck = wc.transform;
        }
    }

    void Update()
    {
        if (isDashing) return;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            if (isWallSliding)
            {
                int dir = isFacingRight ? -1 : 1;
                rb.velocity = new Vector2(dir * wallJumpHorizontal, wallJumpVertical);
                isWallSliding = false;
                jumpsLeft = maxJumps - 1;
            }
            else if (jumpsLeft > 0)
            {
                Jump(Vector2.up * jumpForce);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && Time.time - lastDashTime > dashCooldown)
        {
            StartCoroutine(DoDash());
        }

        UpdateAnimations();

        if (transform.position.y < deathY)
        {
            Die();
        }
    }

    void FixedUpdate()
    {
        if (isDashing) return;

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded && !wasGrounded) jumpsLeft = maxJumps;

        RaycastHit2D hit = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * (isFacingRight ? 1 : -1),
            wallCheckDistance,
            groundLayer
        );
        isWallAdjacent = hit.collider != null;

        if (!isGrounded && isWallAdjacent && moveInput != 0 && enableWallSlide)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, wallSlideSpeed, float.MaxValue));
        }
        else isWallSliding = false;

        if (moveInput > 0 && !isFacingRight) Flip();
        if (moveInput < 0 && isFacingRight) Flip();



    private void Jump(Vector2 force)
    {

    }

    private IEnumerator DoDash()
    {
        isDashing = true;
        canDash = false;
        lastDashTime = Time.time;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        float dir = isFacingRight ? 1f : -1f;
        float start = Time.time;
        while (Time.time < start + dashDuration)
        {
            rb.velocity = new Vector2(dir * dashSpeed, 0f);
            yield return null;
        }

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void UpdateAnimations()
    {
        if (animator == null) return;
        animator.SetFloat("Speed", Mathf.Abs(moveInput));
        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetFloat("VerticalSpeed", rb.velocity.y);
        animator.SetBool("isDashing", isDashing);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    public void Die()
    {
        Debug.Log("[PlayerController] Player chết - Gọi TakeDamage");
        if (HealthManager.instance != null)
        {
            // Trừ toàn bộ máu còn lại để kích hoạt Game Over
            HealthManager.instance.TakeDamage(HealthManager.instance.currentHealth);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("KillZone"))
        {
            Debug.Log("[PlayerController] Va chạm KillZone -> Gọi Die()");
            Die();
        }
    }
}