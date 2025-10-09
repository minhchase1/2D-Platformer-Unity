using UnityEngine;
using System.Collections;

public class ExitTrigger : MonoBehaviour
{
    [Header("Fade Duration (optional)")]
    public float fadeDuration = 0.5f; // thời gian fade mượt

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(LevelExit());
        }
    }

    private IEnumerator LevelExit()
    {
        // Fade mượt nếu UIManager có
        if (UIManager.instance != null)
        {
            UIManager.instance.FadeToBlack();
            yield return new WaitForSeconds(fadeDuration); // chờ fade xong
        }

        // Gọi GameManager load level mới
        if (GameManager.instance != null)
        {
            GameManager.instance.LevelComplete();
        }

        // Tắt trigger để không bị gọi lại
        GetComponent<Collider2D>().enabled = false;
    }
}
