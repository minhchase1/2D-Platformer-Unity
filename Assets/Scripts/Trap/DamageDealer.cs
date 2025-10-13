using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    // Hàm này được gọi khi có va chạm vật lý xảy ra
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có phải là Player không
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Người chơi đã bị dính bẫy!");

            // Tại đây, bạn sẽ gọi đến script quản lý máu của người chơi để trừ máu
            // Ví dụ:
            // PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(10); // Trừ 10 máu
            // }
        }
    }
}