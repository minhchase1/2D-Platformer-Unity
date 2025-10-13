using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Thêm dòng này để có thể dùng Button

public class MainMenu : MonoBehaviour
{
    public Button continueButton; // Kéo nút "Continue" vào đây trong Inspector

    void Start()
    {
        // Kiểm tra xem có tiến trình nào để tiếp tục không
        if (PlayerPrefs.HasKey("LastUnlockedLevel"))
        {
            // Nếu có, hiện nút Continue
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(true);
            }
        }
        else
        {
            // Nếu không, ẩn nút Continue
            if (continueButton != null)
            {
                continueButton.gameObject.SetActive(false);
            }
        }
    }

    // Nút PLAY bây giờ sẽ là "New Game"
    public void NewGame()
    {
        // Xóa toàn bộ dữ liệu game cũ
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        // Bắt đầu từ Level 1 (scene có build index là 1)
        SceneManager.LoadScene(1);
    }

    // Nút CONTINUE sẽ gọi hàm này
    public void ContinueGame()
    {
        // Load màn chơi cuối cùng đã được mở khóa
        int lastLevel = PlayerPrefs.GetInt("LastUnlockedLevel");
        SceneManager.LoadScene(lastLevel);
    }

    public void QuitGame()
    {
        Debug.Log("Thoát game!");
        Application.Quit();
    }
}
