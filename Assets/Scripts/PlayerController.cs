// PlayerController.cs - PHIÊN BẢN ĐÃ NÂNG CẤP HOÀN CHỈNH
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    public float jumpForce = 14f;
    public int maxJumps = 1; // Mặc định là 1, power-up sẽ cho nhảy lần 2

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

    // --- BIẾN MỚI CHO POWER-UPS ---
    [Header("Power-ups")]
    [Tooltip("Đối tượng hình ảnh của khiên, sẽ bật/tắt")]
    [SerializeField] private GameObject shieldVisual;
    [Tooltip("Vùng trigger dùng để hút coin")]
    [SerializeField] private CircleCollider2D coinMagnetCollider;

    private int defaultMaxJumps; // Biến để lưu lại số lần nhảy gốc
    private bool isShielded = false; // Biến trạng thái của khiên
    // --- KẾT THÚC BIẾN MỚI ---

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

        // --- CẬP NHẬT HÀM AWAKE ---
        defaultMaxJumps = maxJumps; // Lưu lại số lần nhảy ban đầu
        jumpsLeft = maxJumps;

        // Tắt các hiệu ứng power-up khi bắt đầu game
        if (shieldVisual != null) shieldVisual.SetActive(false);
        if (coinMagnetCollider != null) coinMagnetCollider.enabled = false;
        // --- KẾT THÚC CẬP NHẬT ---

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
                rb.linearVelocity = new Vector2(dir * wallJumpHorizontal, wallJumpVertical);
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

        if (moveInput != 0)
        {
            rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            jumpsLeft = maxJumps;
        }

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
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Clamp(rb.linearVelocity.y, wallSlideSpeed, float.MaxValue));
        }
        else isWallSliding = false;

        if (moveInput > 0 && !isFacingRight) Flip();
        if (moveInput < 0 && isFacingRight) Flip();
    }

    private void Jump(Vector2 force)
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(force, ForceMode2D.Impulse);
        jumpsLeft--;
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
            rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);
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
        animator.SetFloat("VerticalSpeed", rb.linearVelocity.y);
        animator.SetBool("isDashing", isDashing);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // --- PHẦN LOGIC SÁT THƯƠNG VÀ POWER-UPS MỚI ---

    // Hàm nhận sát thương mới, kiểm tra khiên trước
    public void ApplyDamage(int damageAmount)
    {
        if (isShielded)
        {
            isShielded = false; // Tắt trạng thái khiên
            if (shieldVisual != null) shieldVisual.SetActive(false); // Tắt hình ảnh khiên
            Debug.Log("Khiên đã đỡ một lần sát thương!");
            // Dừng coroutine đang chạy để tránh lỗi không mong muốn
            StopCoroutine(ShieldCoroutine());
            return; // Dừng lại, không nhận sát thương
        }

        // Nếu không có khiên, gọi hàm xử lý máu của bạn
        if (HealthManager.instance != null)
        {
            HealthManager.instance.TakeDamage(damageAmount);
        }
    }

    // Hàm Die() cũ giờ sẽ gọi qua hệ thống sát thương mới
    public void Die()
    {
        Debug.Log("[PlayerController] Player chết - Gọi ApplyDamage");
        // Gây một lượng sát thương cực lớn để đảm bảo người chơi chết
        ApplyDamage(999);
    }

    // Hàm trung tâm nhận tín hiệu từ script PowerUp
    public void ActivatePowerUp(PowerUp.PowerUpType type, float duration)
    {
        // Dừng tất cả các coroutine power-up cũ để làm mới thời gian
        StopAllCoroutines();
        // Sau khi dừng, phải chạy lại coroutine Dash nếu đang lướt
        if (isDashing) StartCoroutine(DoDash());

        // Kích hoạt coroutine mới dựa trên loại power-up
        if (type == PowerUp.PowerUpType.DoubleJump)
        {
            StartCoroutine(DoubleJumpCoroutine(duration));
        }
        else if (type == PowerUp.PowerUpType.Shield)
        {
            StartCoroutine(ShieldCoroutine());
        }
        else if (type == PowerUp.PowerUpType.CoinMagnet)
        {
            StartCoroutine(CoinMagnetCoroutine(duration));
        }
    }

    // Coroutine xử lý hiệu ứng Double Jump
    private IEnumerator DoubleJumpCoroutine(float duration)
    {
        Debug.Log("Power-up: Double Jump ĐÃ KÍCH HOẠT!");
        maxJumps = 2; // Cho phép nhảy 2 lần
        yield return new WaitForSeconds(duration); // Chờ hết thời gian
        maxJumps = defaultMaxJumps; // Trả về số lần nhảy gốc
        Debug.Log("Power-up: Double Jump ĐÃ HẾT HẠN!");
    }

    // Coroutine xử lý hiệu ứng Khiên
    private IEnumerator ShieldCoroutine()
    {
        Debug.Log("Power-up: Khiên ĐÃ KÍCH HOẠT!");
        isShielded = true;
        if (shieldVisual != null) shieldVisual.SetActive(true);
        yield return null; // Khiên không cần thời gian, nó sẽ tồn tại cho đến khi bị phá
    }

    // Coroutine xử lý hiệu ứng Nam châm hút Coin
    private IEnumerator CoinMagnetCoroutine(float duration)
    {
        Debug.Log("Power-up: Nam châm ĐÃ KÍCH HOẠT!");
        if (coinMagnetCollider != null) coinMagnetCollider.enabled = true;
        yield return new WaitForSeconds(duration); // Chờ hết thời gian
        if (coinMagnetCollider != null) coinMagnetCollider.enabled = false;
        Debug.Log("Power-up: Nam châm ĐÃ HẾT HẠN!");
    }

    // Xử lý va chạm
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("KillZone"))
        {
            Debug.Log("[PlayerController] Va chạm KillZone -> Gọi Die()");
            Die();
        }

        // Ví dụ: Nếu bạn có kẻ thù với tag "Enemy" và chúng là trigger
        if (collision.CompareTag("Enemy"))
        {
            ApplyDamage(1); // Gây 1 sát thương
        }
    }

    // Thêm hàm này nếu kẻ thù của bạn dùng va chạm vật lý (không phải trigger)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            ApplyDamage(1); // Gây 1 sát thương
        }
    }
}