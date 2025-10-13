using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Tham chiếu đến UI Text tổng
    private TMP_Text coinText;
    private TMP_Text gemText;

    // --- PHẦN THAM CHIẾU CHO UI KẾT THÚC MÀN CHƠI ---
    [Header("Level Complete UI")]
    [Tooltip("Panel UI sẽ hiện ra khi hoàn thành màn chơi")]
    [SerializeField] private GameObject levelCompletePanel;
    [Tooltip("Kéo 3 đối tượng Image của các ngôi sao vào đây theo thứ tự")]
    [SerializeField] private GameObject[] stars;

    [SerializeField] private PlayerController playerController;

    // --- BIẾN LƯU TRỮ DỮ LIỆU TỔNG ---
    // Các biến này lưu tổng số tiền tệ của người chơi qua tất cả các màn
    private int totalCoinCount = 0;
    private int totalGemCount = 0;

    // --- BIẾN ĐẾM TẠM THỜI CHO MÀN CHƠI HIỆN TẠI ---
    // Các biến này sẽ được reset mỗi khi bắt đầu màn mới
    private int coinsCollectedThisLevel = 0;
    private int totalCoinsInThisLevel = 0;

    public Vector3 playerStartPosition;

    private void Awake()
    {
        // --- KHỞI TẠO SINGLETON ---
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Đăng ký sự kiện sceneLoaded để xử lý logic khi màn mới được tải
        SceneManager.sceneLoaded += OnSceneLoaded;

        // Tải dữ liệu tổng đã lưu
        totalCoinCount = PlayerPrefs.GetInt("TotalCoins", 0);
        totalGemCount = PlayerPrefs.GetInt("TotalGems", 0);
    }

    private void OnDestroy()
    {
        // Hủy đăng ký sự kiện để tránh lỗi
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // --- HÀM ĐƯỢC GỌI MỖI KHI MỘT MÀN MỚI ĐƯỢC TẢI ---
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 1. Reset lại số coin đã thu thập trong màn
        coinsCollectedThisLevel = 0;

        // 2. Tìm và đếm tất cả các đối tượng có tag "Coin" trong màn mới
        totalCoinsInThisLevel = GameObject.FindGameObjectsWithTag("Coin").Length;
        Debug.Log($"Màn chơi '{scene.name}' có tổng cộng: {totalCoinsInThisLevel} coins.");

        // 3. Tìm lại các tham chiếu UI và Player trong màn mới
        FindReferencesAgain();

        // 4. Cập nhật hiển thị UI
        UpdateGUI();
    }

    // --- TÌM LẠI CÁC THAM CHIẾU TRONG MÀN MỚI ---
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

        // Tìm LevelCompletePanel bằng tên và đảm bảo nó được ẩn đi khi bắt đầu màn
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

    // --- HÀM ĐƯỢC GỌI BỞI SCRIPT `Coin.cs` KHI NHẶT ---
    public void CollectCoin()
    {
        coinsCollectedThisLevel++;
        UpdateGUI(); // GỌI HÀM CẬP NHẬT UI NGAY LẬP TỨC
    }

    // --- HÀM THÊM COIN VÀO TỔNG SỐ VÀ LƯU LẠI ---
    public void AddToTotalCoins(int amount)
    {
        totalCoinCount += amount;
        PlayerPrefs.SetInt("TotalCoins", totalCoinCount);
        PlayerPrefs.Save();
        UpdateGUI(); // Cập nhật lại UI sau khi cộng dồn
    }

    // --- HÀM XỬ LÝ GEM
    public void IncrementGemCount()
    {
        totalGemCount++;
        PlayerPrefs.SetInt("TotalGems", totalGemCount);
        PlayerPrefs.Save();
        UpdateGUI();
    }

    
    // --- CẬP NHẬT GIAO DIỆN NGƯỜI DÙNG ---
    private void UpdateGUI()
    {
        // Hiển thị TỔNG SỐ coin (thường ở menu hoặc shop)
        if (coinText != null)
            coinText.text = "Tổng Coins: " + totalCoinCount;

        // HIỂN THỊ SỐ COIN TRONG MÀN HIỆN TẠI
        // Giả sử bạn có một đối tượng Text khác tên là levelCoinText
        // Nếu không có, bạn có thể dùng tạm coinText để hiển thị
        if (coinText != null)
            coinText.text = "Coins: " + coinsCollectedThisLevel; // THAY ĐỔI Ở ĐÂY

        if (gemText != null)
            gemText.text = "Gems: " + totalGemCount;
    }

    // --- LOGIC CHÍNH KHI HOÀN THÀNH MÀN CHƠI ---
    public void LevelComplete()
    {


        if (levelCompleteTitle != null)
            levelCompleteTitle.text = "LEVEL COMPLETE";

        if (levelCompleteCoins != null)

        // Tùy chọn: Dừng người chơi lại để tránh di chuyển sau khi về đích
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        // Cộng số coin vừa thu thập được vào tổng số coin của người chơi
        AddToTotalCoins(coinsCollectedThisLevel);

        // Hiển thị panel hoàn thành màn chơi
        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        // --- TÍNH TOÁN VÀ HIỂN THỊ SAO ---
        // Ẩn tất cả các sao đi trước khi tính toán
        foreach (var star in stars)
        {
            if (star != null) star.SetActive(false);
        }

        int starsEarned = 0;
        float coinPercentage = 0f;

        // Tránh lỗi chia cho 0 nếu màn không có coin nào
        if (totalCoinsInThisLevel > 0)
        {
            coinPercentage = (float)coinsCollectedThisLevel / totalCoinsInThisLevel;
        }

        // Xác định số sao dựa trên tỷ lệ %
        if (coinPercentage >= 1.0f) // Điều kiện 3 sao (100%)
        {
            starsEarned = 3;
        }
        else if (coinPercentage >= 0.5f) // Điều kiện 2 sao (từ 50% đến dưới 100%)
        {
            starsEarned = 2;
        }
        else // Điều kiện 1 sao (dưới 50%, có thể là 0)
        {
            // Nếu bạn muốn phải nhặt ít nhất 1 coin mới được 1 sao, thêm điều kiện: if (coinsCollectedThisLevel > 0)
            starsEarned = 1;
        }

        Debug.Log($"Tỷ lệ coin: {coinPercentage * 100}%. Số sao đạt được: {starsEarned}");

        // Hiển thị đúng số lượng sao đã kiếm được
        for (int i = 0; i < starsEarned; i++)
        {
            if (i < stars.Length && stars[i] != null)
            {
                stars[i].SetActive(true);
            }
        }
    }

    // --- CÁC HÀM ĐIỀU HƯỚNG (SẼ ĐƯỢC GỌI TỪ CÁC NÚT TRÊN UI) ---
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
            SceneManager.LoadScene("Menu"); // Về màn hình chính nếu đã hết level
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


    // --- HÀM MỚI CHO NÚT REPLAY ---
    public void ReloadLevel()
    {
        // Tải lại màn chơi hiện tại bằng cách lấy build index của scene đang hoạt động
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // --- HÀM MỚI CHO NÚT MENU ---
    public void BackToMenu()
    {
        // Tải màn hình Menu (Hãy chắc chắn bạn có một scene tên là "Menu" trong Build Settings)
        SceneManager.LoadScene("Menu");
    }
}
