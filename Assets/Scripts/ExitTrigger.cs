using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour
{
    [Header("Fade Duration (optional)")]
    public float fadeDuration = 0.5f; // thời gian fade mượt

    private bool hasTriggered = false; // Đảm bảo trigger chỉ được gọi một lần

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasTriggered && collision.CompareTag("Player"))
        {
            // Đánh dấu đã kích hoạt để không bị gọi lại
            hasTriggered = true;

            // --- DÒNG MỚI ĐƯỢC THÊM VÀO ---
            // Lấy Rigidbody của Player và đóng băng chuyển động của họ lại
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero; // Đặt vận tốc về 0
            }
            // --- KẾT THÚC DÒNG MỚI ---

            // Ra lệnh cho GameManager bắt đầu chuỗi kết thúc màn chơi
            GameManager.instance.StartLevelCompleteSequence(fadeDuration);

            // Tùy chọn: Bạn có thể không cần vô hiệu hóa trigger ngay lập tức
            // để animation của portal vẫn chạy trong lúc fade.
            // gameObject.SetActive(false);
        }
    }

    // Coroutine LevelExit này có vẻ không được sử dụng trong logic hiện tại
    // bạn có thể xóa nó đi nếu không cần thiết để code gọn hơn.
    private IEnumerator LevelExit()
    {
        // Fade mượt nếu UIManager có
        if (UIManager.instance != null)
        {
            UIManager.instance.FadeToBlack();
            yield return new WaitForSeconds(fadeDuration); // chờ fade xong
        }

        // Gọi GameManager load level mới
        if (GameManager.instance != null)
        {
            GameManager.instance.LevelComplete();
        }

        // Tắt trigger để không bị gọi lại
        GetComponent<Collider2D>().enabled = false;
    }
}