using UnityEngine;
using System.Collections;

public class IcicleTrap : MonoBehaviour
{
    [SerializeField] private bool dealsDamage = true;
    [SerializeField] private int damageAmount = 2;
    [SerializeField] private float fallDelay = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.05f;

    private Rigidbody2D rb;
    private PolygonCollider2D polyCollider;
    private bool isTriggered = false;
    private Vector3 initialPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        polyCollider = GetComponent<PolygonCollider2D>();
        initialPosition = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            isTriggered = true;
            // THAY ĐỔI 1: Truyền thông tin của đối tượng va chạm (Player) vào Coroutine
            StartCoroutine(FallSequence(collision));
        }
    }

    // THAY ĐỔI 2: Coroutine giờ sẽ nhận Collider của Player
    private IEnumerator FallSequence(Collider2D playerCollider)
    {
        float timer = 0;
        while (timer < fallDelay)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.position = initialPosition + new Vector3(x, 0, 0);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = initialPosition;

        polyCollider.isTrigger = false;
        rb.bodyType = RigidbodyType2D.Dynamic;

        // THAY ĐỔI 3 (QUAN TRỌNG): Tạm thời bỏ qua va chạm với người chơi để nó rơi xuyên qua
        if (playerCollider != null)
        {
            Physics2D.IgnoreCollision(polyCollider, playerCollider, true);
        }
    }

    // Dán để thay thế cho hàm OnCollisionEnter2D cũ trong IcicleTrap.cs

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Dòng log này sẽ chạy cho MỌI va chạm vật lý
        Debug.Log("Đã va chạm vật lý với đối tượng tên: '" + collision.gameObject.name + "', có Tag là: '" + collision.gameObject.tag + "'");

        // Gây sát thương nếu va vào Player (và nếu được bật)
        if (dealsDamage && collision.gameObject.CompareTag("Player"))
        {
            HealthManager.instance.TakeDamage(damageAmount);
        }

        // Kiểm tra va chạm với nền đất (Tilemap của bạn nên có Tag này)
        if (collision.gameObject.CompareTag("Ground"))
        {
            // DÒNG LOG MỚI: Ghi nhận va chạm cụ thể với Ground/Tilemap
            Debug.Log("Cột băng đã va chạm với Ground (Tilemap) và sẽ bị phá hủy.");

            // Phá hủy cột băng
            Destroy(gameObject);
        }
    }
}