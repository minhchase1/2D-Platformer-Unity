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

    // HÀM NÀY PHẢI NẰM Ở ĐÂY
    public void ContinueToNextLevel()
    {
        int nextLevelIndex = PlayerPrefs.GetInt("NextLevelIndex", 1);

        if (nextLevelIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextLevelIndex);
        }
        else
        {
            SceneManager.LoadScene("Menu");
        }
    }
}