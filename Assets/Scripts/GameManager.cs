using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text coinText;      // Hiển thị coin HUD
    [SerializeField] private TMP_Text gemText;       // Hiển thị gem HUD
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private TMP_Text levelCompleteTitle;
    [SerializeField] private TMP_Text levelCompleteCoins; // Panel hiển thị coin/gem

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    // ============================
    // COINS & GEMS
    // ============================
    private int levelCoinCount = 0; // Vàng level hiện tại
    private int totalCoinCount = 0; // Vàng cộng dồn

    private int levelGemCount = 0;  // Gem level hiện tại
    private int totalGemCount = 0;  // Gem cộng dồn

    private bool isGameOver = false;
    public Vector3 playerStartPosition;
    private int totalLevelCoins = 0;
    private int totalLevelGems = 0;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        Application.targetFrameRate = 60;
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Load tổng vàng & gem trước khi bắt đầu
        totalCoinCount = PlayerPrefs.GetInt("TotalCoins", 0);
        totalGemCount = PlayerPrefs.GetInt("TotalGems", 0);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindReferencesAgain();

        isGameOver = false;
        levelCoinCount = 0;
        levelGemCount = 0;
        UpdateGUI();

        FindTotalPickups();

        if (UIManager.instance != null)
            UIManager.instance.FadeFromBlack();
    }

    private void FindReferencesAgain()
    {
        coinText = FindObjectOfType<TMP_Text>(); // Cần chắc chắn đúng Text hiển thị coin
        gemText = GameObject.Find("GemText")?.GetComponent<TMP_Text>(); // Gán gemText nếu có
        levelCompletePanel = GameObject.Find("LevelCompletePanel");
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);

        playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
            playerStartPosition = playerController.transform.position;
    }

    private void Start()
    {
        if (UIManager.instance != null)
            UIManager.instance.FadeFromBlack();

        if (playerController != null)
            playerStartPosition = playerController.transform.position;

        UpdateGUI();
        FindTotalPickups();
    }

    // ============================
    // COIN / GEM COLLECTION
    // ============================
    public void IncrementCoinCount()
    {
        levelCoinCount++;
        totalCoinCount++;

        PlayerPrefs.SetInt("TotalCoins", totalCoinCount);
        PlayerPrefs.Save();

        UpdateGUI();
    }

    public void IncrementGemCount()
    {
        levelGemCount++;
        totalGemCount++;

        PlayerPrefs.SetInt("TotalGems", totalGemCount);
        PlayerPrefs.Save();

        UpdateGUI();
    }

    private void UpdateGUI()
    {
        if (coinText != null)
            coinText.text = $"Coin: {levelCoinCount} | Total: {totalCoinCount}";

        if (gemText != null)
            gemText.text = $"Gem: {levelGemCount} | Total: {totalGemCount}";
    }

    // ============================
    // DEATH / RESPAWN
    // ============================
    public void Death()
    {
        if (isGameOver)
        {
            Debug.Log("[GameManager] Bỏ qua Death() vì isGameOver = true");
            return;
        }

        Debug.Log("[GameManager] Gọi Death() — Respawn bắt đầu");
        isGameOver = true;

        if (UIManager.instance != null)
        {
            Debug.Log("[GameManager] Gọi UIManager.DisableMobileControls() và FadeToBlack()");
            UIManager.instance.DisableMobileControls();
            UIManager.instance.FadeToBlack();
        }

        if (playerController != null)
        {
            Debug.Log("[GameManager] Ẩn player");
            playerController.gameObject.SetActive(false);
        }

        StartCoroutine(DeathCoroutine());
    }


    private IEnumerator DeathCoroutine()
    {
        Debug.Log("[GameManager] DeathCoroutine bắt đầu, đợi 1s...");
        yield return new WaitForSeconds(1f);

        if (playerController != null)
        {
            Debug.Log("[GameManager] Đặt lại vị trí player về spawn: " + playerStartPosition);
            playerController.transform.position = playerStartPosition;
            playerController.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);
        Debug.Log("[GameManager] Reload scene...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // ============================
    // PICKUPS
    // ============================
    private void FindTotalPickups()
    {
        pickup[] pickups = GameObject.FindObjectsOfType<pickup>();
        totalLevelCoins = 0;
        totalLevelGems = 0;

        foreach (pickup item in pickups)
        {
            if (item.pt == pickup.pickupType.coin)
                totalLevelCoins++;
            if (item.pt == pickup.pickupType.gem)
                totalLevelGems++;
        }
    }

    // ============================
    // LEVEL COMPLETE
    // ============================
    public void LevelComplete()
    {
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(true);

        if (levelCompleteTitle != null)
            levelCompleteTitle.text = "LEVEL COMPLETE";

        if (levelCompleteCoins != null)
        {
            levelCompleteCoins.text =
                $"LEVEL COINS: {levelCoinCount} / {totalLevelCoins}\n" +
                $"LEVEL GEMS: {levelGemCount} / {totalLevelGems}\n" +
                $"TOTAL COINS: {totalCoinCount}\n" +
                $"TOTAL GEMS: {totalGemCount}";
        }

        StartCoroutine(FadeAndLoadNextLevel());
    }

    private IEnumerator FadeAndLoadNextLevel()
    {
        float fadeDuration = 0.5f;

        if (UIManager.instance != null)
        {
            UIManager.instance.FadeToBlack();
            yield return new WaitForSeconds(fadeDuration);
        }

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
            SceneManager.LoadScene("Menu");
        }
    }

    // ============================
    // MOBILE CONTROLS
    // ============================
    public void DisableMobileControls()
    {
        if (UIManager.instance != null)
            UIManager.instance.DisableMobileControls();
    }
}
