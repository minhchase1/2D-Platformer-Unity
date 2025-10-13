using UnityEngine;

public class SnowManController : MonoBehaviour
{
    public float moveSpeed = 2f;
    private Rigidbody2D rb;
    private bool isFacingRight = true;

    [Header("Collision Checks")]
    public Transform groundCheck;   // Điểm kiểm tra đất phía trước (để biết mép vực)
    public Transform wallCheck;     // Điểm kiểm tra tường phía trước
    public Transform feetCheck;     // (MỚI) Điểm kiểm tra ngay dưới chân (để biết có đang đứng trên đất không)

    public float checkRadius = 0.1f;
    public LayerMask whatIsGround;

    private bool isGroundedInFront; // Tên biến mới cho rõ ràng
    private bool isTouchingWall;
    private bool isActuallyOnGround; // (MỚI) Biến mới để kiểm tra chân

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    void FixedUpdate()
    {
        // Thực hiện tất cả các kiểm tra
        isGroundedInFront = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isTouchingWall = Physics2D.OverlapCircle(wallCheck.position, checkRadius, whatIsGround);
        isActuallyOnGround = Physics2D.OverlapCircle(feetCheck.position, checkRadius, whatIsGround);

        // DÒNG DEBUG QUAN TRỌNG NHẤT: In ra trạng thái của các biến kiểm tra
        // Chúng ta chỉ cần xem 2 biến quan trọng nhất cho việc lật lại ở mép vực
        //Debug.Log($"GroundedFront: {isGroundedInFront} | ActuallyOnGround: {isActuallyOnGround}");

        // LOGIC QUAY ĐẦU
        if ((!isGroundedInFront || isTouchingWall) && isActuallyOnGround)
        {
            Debug.Log("!!! FLIP SHOULD HAPPEN NOW !!!"); // Thêm dòng này để biết khi nào nó định lật
            Flip();
        }

        // Di chuyển
        if (isFacingRight)
        {
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(-moveSpeed, rb.velocity.y);
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
        if (wallCheck != null)
            Gizmos.DrawWireSphere(wallCheck.position, checkRadius);
        if (feetCheck != null) // (MỚI) Vẽ điểm kiểm tra dưới chân
            Gizmos.DrawWireSphere(feetCheck.position, checkRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("SnowMan va chạm với đối tượng: " + collision.gameObject.name + ", có tag là: " + collision.gameObject.tag);

        if (collision.gameObject.CompareTag("Player"))
        {
            // Gọi hàm trừ máu của Player ở đây
        }
    }
}