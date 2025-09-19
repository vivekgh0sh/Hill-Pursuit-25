using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;

    [Header("Gameplay UI")]
    public TextMeshProUGUI gameplayDistanceText;
    public TextMeshProUGUI gameplayCoinText;

    [Header("Game Over UI")]
    public TextMeshProUGUI finalDistanceText;
    public TextMeshProUGUI finalCoinsText;
    public TextMeshProUGUI highscoreText;
    public TextMeshProUGUI totalCoinsText;
    public Button restartButton;
    public Button menuButton;

    void Start()
    {
        gameOverPanel.SetActive(false);
        gameplayPanel.SetActive(true);

        restartButton.onClick.AddListener(OnRestartButtonPressed);
        menuButton.onClick.AddListener(OnMenuButtonPressed);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            gameplayDistanceText.text = GameManager.Instance.GetCurrentDistance().ToString();
            gameplayCoinText.text = "Coins: " + GameManager.Instance.GetRunCoins().ToString();
        }
    }

    public void ShowGameOverScreen()
    {
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            finalDistanceText.text = "Distance: " + GameManager.Instance.GetCurrentDistance().ToString();
            finalCoinsText.text = "Coins This Run: " + GameManager.Instance.GetRunCoins().ToString();
            highscoreText.text = "Highscore: " + GameManager.Instance.highscore.ToString();
            totalCoinsText.text = "Total Coins: " + GameManager.Instance.totalCoins.ToString();
        }
    }

    private void OnRestartButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnMenuButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMenu();
        }
    }
}