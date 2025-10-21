using System.Collections;
using UnityEngine;

public class SpikeTrapController : MonoBehaviour
{
    // Đối tượng gai sẽ di chuyển
    public Transform spikes;

    // Vị trí khi bẫy trồi lên
    public Transform activePosition;

    // Vị trí khi bẫy ẩn đi
    public Transform hiddenPosition;

    // Tốc độ di chuyển của bẫy
    public float moveSpeed = 5f;

    // Thời gian bẫy ở trên cao trước khi thụt xuống
    public float delay = 1f;

    // Biến để kiểm tra bẫy đã được kích hoạt chưa, tránh kích hoạt nhiều lần
    private bool isActivated = false;

    // Hàm này được gọi khi có đối tượng khác đi vào trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đó có phải là Player không và bẫy chưa được kích hoạt
        if (other.CompareTag("Player") && !isActivated)
        {
            // Bắt đầu chu trình hoạt động của bẫy
            StartCoroutine(ActivateTrap());
        }
    }

    private IEnumerator ActivateTrap()
    {
        // Đánh dấu là bẫy đã được kích hoạt
        isActivated = true;

        // --- Giai đoạn 1: Di chuyển lên ---
        while (Vector3.Distance(spikes.position, activePosition.position) > 0.01f)
        {
            spikes.position = Vector3.MoveTowards(spikes.position, activePosition.position, moveSpeed * Time.deltaTime);
            yield return null; // Chờ đến frame tiếp theo
        }
        spikes.position = activePosition.position; // Đảm bảo gai đến đúng vị trí

        // --- Giai đoạn 2: Chờ ---
        yield return new WaitForSeconds(delay);

        // --- Giai đoạn 3: Di chuyển xuống ---
        while (Vector3.Distance(spikes.position, hiddenPosition.position) > 0.01f)
        {
            spikes.position = Vector3.MoveTowards(spikes.position, hiddenPosition.position, moveSpeed * Time.deltaTime);
            yield return null; // Chờ đến frame tiếp theo
        }
        spikes.position = hiddenPosition.position; // Đảm bảo gai về đúng vị trí

        // Reset lại để bẫy có thể được kích hoạt lần nữa
        isActivated = false;
    }
}