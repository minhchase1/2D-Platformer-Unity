using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // Tên của scene hang động băng mà bạn muốn chuyển đến
    public string sceneNameToLoad;

    // Hàm này sẽ tự động được gọi khi có một đối tượng khác đi vào trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem đối tượng va chạm có phải là người chơi không (bằng cách so sánh Tag)
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the cave! Loading scene: " + sceneNameToLoad);
            // Tải scene mới
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}