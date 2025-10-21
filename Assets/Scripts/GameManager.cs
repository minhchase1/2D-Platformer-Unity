using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private TMP_Text coinText;
    private TMP_Text gemText;

    [Header("Level Complete UI")]
    [Tooltip("Panel UI sẽ hiện ra khi hoàn thành màn chơi")]
    [SerializeField] private GameObject levelCompletePanel;
    [Tooltip("Kéo 3 đối tượng Image của các ngôi sao vào đây theo thứ tự")]
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
            Transform starsContainer = levelCompletePanel.transform.Find("StarsContainer");
            if (starsContainer != null && starsContainer.childCount >= 3)
            {
                stars = new GameObject[starsContainer.childCount];
                for (int i = 0; i < starsContainer.childCount; i++)
                {
                    stars[i] = starsContainer.GetChild(i).gameObject;
                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy 'StarsContainer' hoặc không đủ sao bên trong.");
            }

            Button nextLevelButton = levelCompletePanel.transform.Find("ButtonsContainer/NextLevelButton").GetComponent<Button>();
            if (nextLevelButton != null)
            {
                nextLevelButton.onClick.RemoveAllListeners();

                string currentSceneName = SceneManager.GetActiveScene().name;

                if (currentSceneName == "Level")
                {
                    Debug.Log($"[GameManager] Đang ở màn '{currentSceneName}', nút Next sẽ dẫn đến Shop.");
                    nextLevelButton.onClick.AddListener(GoToShop);
                }
                else
                {
                    Debug.Log($"[GameManager] Đang ở màn '{currentSceneName}', nút Next sẽ dẫn đến màn tiếp theo.");
                    nextLevelButton.onClick.AddListener(GoToNextLevel);
                }
            }

            Button replayButton = levelCompletePanel.transform.Find("ButtonsContainer/ReplayButton").GetComponent<Button>();
            if (replayButton != null)
            {
                replayButton.onClick.RemoveAllListeners();
                replayButton.onClick.AddListener(ReloadLevel);
            }

            Button menuButton = levelCompletePanel.transform.Find("ButtonsContainer/MenuButton").GetComponent<Button>();
            if (menuButton != null)
            {
                menuButton.onClick.RemoveAllListeners();
                menuButton.onClick.AddListener(BackToMenu);
            }

            levelCompletePanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy 'LevelCompletePanel' trong màn này.");
        }

        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerStartPosition = playerController.transform.position;
            playerController.enabled = true;
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

    public void StartLevelCompleteSequence(float fadeTime)
    {
        StartCoroutine(LevelCompleteCoroutine(fadeTime));
    }

    private IEnumerator LevelCompleteCoroutine(float fadeDuration)
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.FadeToBlack();
            yield return new WaitForSeconds(fadeDuration);
        }

        LevelComplete();
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
        int nextLevelIndex = currentIndex + 2;

        Debug.Log($"[GameManager] Chuẩn bị vào Shop. Sẽ lưu NextLevelIndex là: {nextLevelIndex}.");
        PlayerPrefs.SetInt("NextLevelIndex", nextLevelIndex);
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