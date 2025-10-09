using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public static HealthManager instance;

    public int MaxHealth = 6;
    public int currentHealth;

    [Header("Lives")]
    public int maxLives = 3;
    public int currentLives;

    [Header("Heart UI")]
    [SerializeField] private Image[] hearts;
    [SerializeField] private Sprite FullHeartSprite;
    [SerializeField] private Sprite HalfHeartSprite;
    [SerializeField] private Sprite EmptyHeartSprite;

    [Header("UI Panels")]
    [SerializeField] private GameObject GameOverPanel;

    private GameObject player;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerController>()?.gameObject;
        currentHealth = MaxHealth;
        currentLives = maxLives;
        DisplayHearts();

        if (GameOverPanel != null)
            GameOverPanel.SetActive(false);
    }

    // Dán đoạn code này để thay thế cho hàm TakeDamage() cũ của bạn

    public void TakeDamage(int dmg)
    {
        Debug.Log($"[HealthManager] TakeDamage được gọi, dmg = {dmg}, currentHealth = {currentHealth}");
        currentHealth -= dmg;
        DisplayHearts();

        // KIỂM TRA NẾU HẾT MÁU
        if (currentHealth <= 0)
        {
            currentLives--; // Trừ đi một mạng
            Debug.Log($"[HealthManager] Mất một mạng! Số mạng còn lại: {currentLives}");

            // KIỂM TRA XEM CÒN MẠNG ĐỂ HỒI SINH KHÔNG
            if (currentLives > 0)
            {
                // Nếu còn mạng -> Hồi sinh
                Debug.Log("[HealthManager] Còn mạng, tiến hành hồi sinh...");
                currentHealth = MaxHealth; // Hồi đầy máu
                DisplayHearts();           // Cập nhật lại UI tim
                RespawnPlayer();           // Đưa người chơi về vị trí ban đầu
            }
            else
            {
                // Nếu hết mạng -> Game Over
                Debug.Log("[HealthManager] Hết mạng → Game Over!");
                currentHealth = 0;
                ShowGameOver();
            }
        }
    }

    private void ShowGameOver()
    {
        if (GameOverPanel != null)
        {
            GameOverPanel.SetActive(true);
        }

        if (player != null)
        {
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            player.GetComponent<PlayerController>().enabled = false;
        }

        Time.timeScale = 0f;
        Debug.Log("[HealthManager] Hiển thị Game Over Panel!");
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, MaxHealth);
        DisplayHearts();
    }

    private void DisplayHearts()
    {
        int fullHearts = currentHealth / 2;
        bool halfHeart = currentHealth % 2 == 1;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < fullHearts) hearts[i].sprite = FullHeartSprite;
            else if (halfHeart && i == fullHearts) hearts[i].sprite = HalfHeartSprite;
            else hearts[i].sprite = EmptyHeartSprite;
        }
    }

    // Hàm này vẫn có thể được gọi từ nơi khác nếu cần, nhưng không còn dùng trong logic mất mạng
    public void RespawnPlayer()
    {
        if (player == null)
        {
            player = FindObjectOfType<PlayerController>()?.gameObject;
        }

        if (player != null)
        {
            player.transform.position = GameManager.instance.playerStartPosition;

            if (!player.activeSelf)
            {
                player.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("[HealthManager] Không tìm thấy Player để respawn!");
        }
    }
}