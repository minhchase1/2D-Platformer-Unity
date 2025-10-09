using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    [Header("Thời gian hiệu ứng mờ dần (giây)")]
    public float fadeDuration = 1.5f;

    private Image fadeImage;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
        if (fadeImage == null)
        {
            Debug.LogError("⚠️ FadeScreen cần được gắn vào GameObject có Image component!");
            return;
        }

        // Bắt đầu với màu đen toàn màn hình
        fadeImage.color = new Color(0, 0, 0, 1);
    }

    IEnumerator Start()
    {
        // Đợi 1 frame để UI vẽ xong
        yield return null;

        // Bắt đầu hiệu ứng mờ dần sáng ra
        yield return StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        float t = 0f;
        Color startColor = new Color(0, 0, 0, 1);
        Color endColor = new Color(0, 0, 0, 0);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, t / fadeDuration);
            yield return null;
        }

        fadeImage.color = endColor;
    }

    public IEnumerator FadeOut()
    {
        float t = 0f;
        Color startColor = new Color(0, 0, 0, 0);
        Color endColor = new Color(0, 0, 0, 1);

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeImage.color = Color.Lerp(startColor, endColor, t / fadeDuration);
            yield return null;
        }

        fadeImage.color = endColor;
    }
}
