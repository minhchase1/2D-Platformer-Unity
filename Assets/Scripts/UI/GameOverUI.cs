using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public void RestartGame()
    {
        // Rất quan trọng: reset lại timeScale trước khi load scene mới
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Debug.Log("[GameOverUI] Quit game được gọi!");

#if UNITY_EDITOR
    // Dừng chế độ Play trong Unity Editor
    UnityEditor.EditorApplication.isPlaying = false;
#else
        // Thoát game trong bản build
        Application.Quit();
#endif
    }
}