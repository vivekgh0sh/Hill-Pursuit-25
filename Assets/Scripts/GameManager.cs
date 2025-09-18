using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Game State")]
    public bool isGameOver = false;

    [Header("Player and Score")]
    public Transform player;
    private int coinsCollected = 0;
    private float distanceTraveled = 0f;
    private float startingZPosition; // This will store our starting point

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI distanceText;
    public TextMeshProUGUI coinText;

    public static GameManager instance;

    void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }
    }

    void Start()
    {
        isGameOver = false;
        if (gameOverPanel != null) { gameOverPanel.SetActive(false); }
        Time.timeScale = 1f;

        coinsCollected = 0;

        // Record where the player starts
        if (player != null)
        {
            startingZPosition = player.position.z;
        }

        UpdateUI();
    }

    void Update()
    {
        if (!isGameOver && player != null)
        {
            // Calculate distance relative to the starting point
            distanceTraveled = player.position.z - startingZPosition;
            UpdateUI();
        }
    }

    public void AddCoin()
    {
        if (isGameOver) return;
        coinsCollected++;
        UpdateUI();
    }

    private void UpdateUI()
    {
        // Use Mathf.Max to ensure the distance never shows a negative number (e.g., due to floating point quirks)
        if (distanceText != null) { distanceText.text = Mathf.Max(0, distanceTraveled).ToString("F0"); }
        if (coinText != null) { coinText.text = "Coins: " + coinsCollected; }
    }

    public void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        if (gameOverPanel != null) { gameOverPanel.SetActive(true); }
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}