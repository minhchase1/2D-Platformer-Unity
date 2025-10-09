using UnityEngine;
using System.Collections;

public class IcicleTrap : MonoBehaviour
{
    [SerializeField] private int damageAmount = 2; // Sát thương gây ra (2 = 1 tim)
    [SerializeField] private float fallDelay = 0.5f; // Thời gian chờ trước khi rơi
    [SerializeField] private float shakeMagnitude = 0.05f; // Độ rung

    private Rigidbody2D rb;
    private bool isTriggered = false;
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = transform.position;
    }

    // Hàm này được gọi khi có đối tượng đi vào vùng Trigger
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem có phải là Player không và bẫy chưa được kích hoạt
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(FallSequence());
        }
    }

    private IEnumerator FallSequence()
    {
        // Rung nhẹ trước khi rơi
        float timer = 0;
        while (timer < fallDelay)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position = initialPosition + new Vector3(x, 0, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;

        // Kích hoạt trọng lực để cột băng rơi xuống
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    // Hàm này được gọi khi cột băng va chạm vật lý với đối tượng khác
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Gây sát thương nếu va vào Player
        if (collision.gameObject.CompareTag("Player"))
        {
            HealthManager.instance.TakeDamage(damageAmount);
        }

        // Sau khi va chạm (với Player hoặc đất), cột băng sẽ biến mất
        // Bạn có thể thêm hiệu ứng vỡ tan ở đây
        Debug.Log("Cột băng đã va chạm và biến mất!");
        Destroy(gameObject);
    }
}