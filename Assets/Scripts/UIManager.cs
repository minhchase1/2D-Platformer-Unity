using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Fade Settings")]
    public Image fadeScreen;              // Ảnh phủ mờ (Image màu đen)
    public float fadeSpeed = 2f;          // Tốc độ mờ sáng/tối
    public bool fadeToBlack;              // Trạng thái chuyển sang đen
    public bool fadeFromBlack;            // Trạng thái sáng dần

    [Header("Mobile Controls (Optional)")]
    [SerializeField] private GameObject mobileControls;

    private void Awake()
    {
        // Đảm bảo chỉ có 1 instance
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Khi bắt đầu game, fade từ đen sang sáng
        if (fadeScreen != null)
        {
            fadeScreen.color = new Color(0, 0, 0, 1); // Bắt đầu đen toàn màn
            StartCoroutine(FadeIn());
        }
        else
        {
            Debug.LogError("⚠️ fadeScreen chưa được gán trong UIManager!");
        }
    }

    private void Update()
    {
        if (fadeToBlack)
        {
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 1f, fadeSpeed * Time.deltaTime)
            );

            if (fadeScreen.color.a >= 1f)
            {
                fadeToBlack = false;
                Debug.Log("🌑 Fade To Black hoàn tất");
            }
        }

        if (fadeFromBlack)
        {
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime)
            );

            if (fadeScreen.color.a <= 0f)
            {
                fadeFromBlack = false;
                Debug.Log("🌅 Fade From Black hoàn tất");
            }
        }
    }

    // Gọi để mờ dần sang màu đen
    public void FadeToBlack()
    {
        fadeToBlack = true;
        fadeFromBlack = false;
    }

    // Gọi để sáng dần ra (từ đen sang trong suốt)
    public void FadeFromBlack()
    {
        fadeFromBlack = true;
        fadeToBlack = false;
    }

    // Coroutine tự fade in khi game bắt đầu
    public IEnumerator FadeIn()
    {
        Debug.Log("🌄 Bắt đầu FadeIn...");

        fadeFromBlack = true;

        while (fadeScreen.color.a > 0f)
        {
            fadeScreen.color = new Color(
                fadeScreen.color.r,
                fadeScreen.color.g,
                fadeScreen.color.b,
                Mathf.MoveTowards(fadeScreen.color.a, 0f, fadeSpeed * Time.deltaTime)
            );
            yield return null;
        }

        fadeFromBlack = false;
        Debug.Log("🌅 Fade From Black hoàn tất");

        // ✅ Thêm dòng này để ẩn luôn ảnh mờ khi fade xong
        fadeScreen.gameObject.SetActive(false);

        Debug.Log("✅ FadeIn hoàn tất");
    }


    public void DisableMobileControls()
    {
        if (mobileControls != null)
            mobileControls.SetActive(false);
    }
}
