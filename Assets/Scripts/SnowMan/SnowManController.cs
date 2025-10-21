using UnityEngine;

public class SnowManController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    [Header("Collision Checks")]
    public Transform groundCheck;
    public Transform wallCheck;
    public Transform feetCheck;
    public Transform headCheck;

    public float checkRadius = 0.1f;
    public float feetCheckRadius = 0.2f; // Bán kính riêng cho kiểm tra chân
    public LayerMask whatIsGround;

    private bool isGroundedInFront;
    private bool isTouchingWall;
    private bool isActuallyOnGround;
    private bool isHittingHead;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        isGroundedInFront = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsGround);
        isActuallyOnGround = Physics2D.OverlapCircle(feetCheck.position, feetCheckRadius, whatIsGround); // Sử dụng bán kính riêng
        isHittingHead = Physics2D.OverlapCircle(headCheck.position, checkRadius, whatIsGround);

        //Debug.Log($"WallCheck: {isTouchingWall}, GroundFront: {isGroundedInFront}, OnGround: {isActuallyOnGround}");


        if ((!isGroundedInFront && isActuallyOnGround) || isTouchingWall || isHittingHead)
        {
            Flip();
        }

        if (isFacingRight)
        {
            rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

        float nudgeAmount = isFacingRight ? 0.1f : -0.1f;
        transform.position += new Vector3(nudgeAmount, 0f, 0f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        if (wallCheck != null)
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        if (feetCheck != null)
            Gizmos.DrawWireSphere(feetCheck.position, feetCheckRadius); // Sử dụng bán kính riêng
        if (headCheck != null)
            Gizmos.DrawWireSphere(headCheck.position, checkRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("SnowMan va chạm với đối tượng: " + collision.gameObject.name + ", có tag là: " + collision.gameObject.tag);


        if (collision.gameObject.CompareTag("Player"))
        {
            // Player damage logic here
        }
    }
}