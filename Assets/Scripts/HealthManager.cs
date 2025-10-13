using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private TMP_Text coinText;
    private TMP_Text gemText;

    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject[] stars;
    [SerializeField] private PlayerController playerController;

    private int totalCoinCount = 0;
    private int totalGemCount = 0;

    private int coinsCollectedThisLevel = 0;
    private int totalCoinsInThisLevel = 0;

    public Vector3 playerStartPosition;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        totalCoinCount = PlayerPrefs.GetInt("TotalCoins", 0);
        totalGemCount = PlayerPrefs.GetInt("TotalGems", 0);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        coinsCollectedThisLevel = 0;

        totalCoinsInThisLevel = GameObject.FindGameObjectsWithTag("Coin").Length;
        Debug.Log($"Màn chơi '{scene.name}' có tổng cộng: {totalCoinsInThisLevel} coins.");

        FindReferencesAgain();
        UpdateGUI();
    }

    private void FindReferencesAgain()
    {
        UIReferences uiRefs = FindObjectOfType<UIReferences>();
        if (uiRefs != null)
        {
            coinText = uiRefs.coinText;
            gemText = uiRefs.gemText;
        }
        else
        {
            coinText = null;
            gemText = null;
        }

        levelCompletePanel = GameObject.Find("LevelCompletePanel");
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy 'LevelCompletePanel' trong màn này. Hãy chắc chắn tên đối tượng là chính xác.");
        }

        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerStartPosition = playerController.transform.position;
        }
    }

    public void CollectCoin()
    {
        coinsCollectedThisLevel++;
        UpdateGUI();
    }

    public void AddToTotalCoins(int amount)
    {
        totalCoinCount += amount;
        PlayerPrefs.SetInt("TotalCoins", totalCoinCount);
        PlayerPrefs.Save();
        UpdateGUI();
    }

    public void IncrementGemCount()
    {
        totalGemCount++;
        PlayerPrefs.SetInt("TotalGems", totalGemCount);
        PlayerPrefs.Save();
        UpdateGUI();
    }

    private void UpdateGUI()
    {
        if (coinText != null)
            coinText.text = "Coins: " + coinsCollectedThisLevel;

        if (gemText != null)
            gemText.text = "Gems: " + totalGemCount;
    }

    public void LevelComplete()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        AddToTotalCoins(coinsCollectedThisLevel);

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        foreach (var star in stars)
        {
            if (star != null) star.SetActive(false);
        }

        int starsEarned = 0;
        float coinPercentage = 0f;

        if (totalCoinsInThisLevel > 0)
        {
            coinPercentage = (float)coinsCollectedThisLevel / totalCoinsInThisLevel;
        }

        if (coinPercentage >= 1.0f)
        {
            starsEarned = 3;
        }
        else if (coinPercentage >= 0.5f)
        {
            starsEarned = 2;
        }
        else
        {
            starsEarned = 1;
        }

        Debug.Log($"Tỷ lệ coin: {coinPercentage * 100}%. Số sao đạt được: {starsEarned}");

        for (int i = 0; i < starsEarned; i++)
        {
            if (i < stars.Length && stars[i] != null)
            {
                stars[i].SetActive(true);
            }
        }
    }

    public void GoToNextLevel()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            PlayerPrefs.SetInt("LastUnlockedLevel", nextIndex);
            PlayerPrefs.Save();
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            PlayerPrefs.DeleteKey("LastUnlockedLevel");
            SceneManager.LoadScene("Menu");
        }
    }

    public void GoToShop()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int nextLevelIndex = currentIndex + 1;

        PlayerPrefs.SetInt("NextLevelIndex", nextLevelIndex);
        PlayerPrefs.SetInt("LastUnlockedLevel", nextLevelIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("Shop");
    }

    public void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
