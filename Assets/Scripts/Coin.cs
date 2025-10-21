// Coin.cs
using UnityEngine;

public class Coin : MonoBehaviour
{
    public float moveSpeed = 12f; // Tốc độ coin bay về người chơi
    private Transform playerTransform;
    private bool isAttracted = false;

    void Update()
    {
        if (isAttracted && playerTransform != null)
        {
            // Di chuyển coin về phía người chơi
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, moveSpeed * Time.deltaTime);
        }
    }

    // Hàm này được gọi bởi nam châm của người chơi để kích hoạt việc hút
    public void AttractToPlayer(Transform target)
    {
        playerTransform = target;
        isAttracted = true;
    }

    // Xử lý khi người chơi thực sự chạm vào coin
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.CollectCoin();
            }
            Destroy(gameObject);
        }
    }
}