// PowerUp.cs
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    // Dùng enum để định nghĩa các loại power-up một cách rõ ràng
    public enum PowerUpType
    {
        DoubleJump,
        Shield,
        CoinMagnet
    }

    [Tooltip("Chọn loại vật phẩm này trong Inspector")]
    public PowerUpType type;

    [Tooltip("Thời gian hiệu lực của vật phẩm (giây). Đối với Khiên, nó sẽ mất khi va chạm.")]
    public float duration = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Kiểm tra xem có phải Player đã nhặt không
        if (collision.CompareTag("Player"))
        {
            // 2. Lấy component PlayerController từ đối tượng Player
            PlayerController player = collision.GetComponent<PlayerController>();

            // 3. Nếu tìm thấy, gọi hàm kích hoạt hiệu ứng trên Player
            if (player != null)
            {
                player.ActivatePowerUp(type, duration);
            }

            // 4. Tự hủy sau khi được nhặt
            Destroy(gameObject);
        }
    }
}