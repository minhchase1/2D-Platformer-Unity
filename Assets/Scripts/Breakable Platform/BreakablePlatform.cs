using UnityEngine;
using System.Collections;

public class BreakablePlatform : MonoBehaviour
{
    [Tooltip("Thời gian chờ trước khi nền tảng bắt đầu vỡ")]
    public float delayBeforeBreak = 0.5f;

    [Tooltip("Thời gian để nền tảng biến mất sau khi vỡ")]
    public float destroyDelay = 1f;

    private bool hasBeenTriggered = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem có phải là người chơi va chạm từ phía trên không
        if (collision.gameObject.CompareTag("Player") && !hasBeenTriggered)
        {
            // Lấy điểm va chạm đầu tiên để xác định phương
            ContactPoint2D contact = collision.contacts[0];

            // Nếu người chơi đáp từ trên xuống (normal.y gần bằng 1)
            if (contact.normal.y < -0.5f)
            {
                hasBeenTriggered = true;
                StartCoroutine(BreakSequence());
            }
        }
    }

    private IEnumerator BreakSequence()
    {
        // Chờ một khoảng thời gian ngắn
        yield return new WaitForSeconds(delayBeforeBreak);

        // TODO: Thêm hiệu ứng rung lắc hoặc đổi màu ở đây nếu muốn
        // Ví dụ: GetComponent<Animator>().SetTrigger("Shake");

        // Làm cho nền tảng rơi xuống
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // Chuyển sang Dynamic để nó rơi
            rb.gravityScale = 2f; // Cho nó rơi nhanh hơn một chút
        }

        // Vô hiệu hóa va chạm để người chơi không thể đứng trên nó nữa
        GetComponent<Collider2D>().enabled = false;

        // Chờ thêm một lúc rồi tự hủy
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}