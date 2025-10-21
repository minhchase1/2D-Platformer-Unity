using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement; // <-- Dòng này rất quan trọng

public class ShopManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text totalCoinsText;
    [SerializeField] private Button buyExtraLifeButton;

    [Header("Item Costs")]
    [SerializeField] private int extraLifeCost = 100;

    private int totalCoins;

    void Start()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        UpdateUI();

        if (PlayerPrefs.GetInt("ExtraLifePurchased", 0) == 1)
        {
            DisableButton(buyExtraLifeButton, "Đã Mua");
        }
    }

    void UpdateUI()
    {
        if (totalCoinsText != null)
            totalCoinsText.text = "Vàng: " + totalCoins.ToString();
    }

    public void BuyExtraLife()
    {
        // ... (code mua bán của bạn ở đây)
        if (PlayerPrefs.GetInt("ExtraLifePurchased", 0) == 1) return;

        if (totalCoins >= extraLifeCost)
        {
            totalCoins -= extraLifeCost;
            PlayerPrefs.SetInt("TotalCoins", totalCoins);
            PlayerPrefs.SetInt("ExtraLifePurchased", 1);
            PlayerPrefs.Save();
            UpdateUI();
            DisableButton(buyExtraLifeButton, "Đã Mua");
        }
    }

    void DisableButton(Button button, string newText)
    {
        if (button != null)
        {
            button.interactable = false;
            button.GetComponentInChildren<TMP_Text>().text = newText;
        }
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    // Trong file ShopManager.cs
    public void ContinueToNextLevel()
    {
        // Lấy chỉ số màn chơi tiếp theo đã được GameManager lưu
        int nextLevelIndex = PlayerPrefs.GetInt("NextLevelIndex", 1);

        // --- THÊM DÒNG LOG NÀY ĐỂ KIỂM TRA ---
        Debug.Log($"[ShopManager] Đang cố gắng tải màn tiếp theo. Lấy được giá trị 'NextLevelIndex' từ PlayerPrefs là: {nextLevelIndex}.");

        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log($"[ShopManager] Sẽ tải màn chơi với build index: {nextLevelIndex}.");
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            Debug.LogWarning($"[ShopManager] Chỉ số {nextLevelIndex} vượt quá số màn trong Build Settings. Quay về Menu.");
            SceneManager.LoadScene("Menu");
        }
    }
}