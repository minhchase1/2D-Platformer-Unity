using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Damage Settings")]
    public int damageAmount = 1;

    // Hàm này được gọi khi có va chạm vật lý xảy ra
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Người chơi đã bị dính bẫy!");

            // Sử dụng PlayerController để xử lý damage thông qua hệ thống mới
            var playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ApplyDamage(damageAmount);
            }
        }
    }
}