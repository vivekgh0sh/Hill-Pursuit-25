using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayUIController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;

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

    [Header("Level Complete UI")]
    public TextMeshProUGUI lc_coinsCollectedText;
    public TextMeshProUGUI lc_totalCoinsText;
    public Button nextLevelButton;
    public Button menuButtonLevelComplete;

    void Start()
    {
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        gameplayPanel.SetActive(true);

        restartButton.onClick.AddListener(OnRestartButtonPressed);
        menuButton.onClick.AddListener(OnMenuButtonPressed);
        nextLevelButton.onClick.AddListener(OnNextLevelButtonPressed);
        menuButtonLevelComplete.onClick.AddListener(OnMenuButtonPressed);
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            gameplayDistanceText.text =GameManager.Instance.GetCurrentDistance().ToString() + "m";
            gameplayDistanceText.text =GameManager.Instance.GetCurrentDistance().ToString() + "m";
            gameplayCoinText.text = GameManager.Instance.GetRunCoins().ToString();
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

    public void ShowLevelCompleteScreen()
    {
        gameplayPanel.SetActive(false);
        levelCompletePanel.SetActive(true);

        if (GameManager.Instance != null)
        {
            lc_coinsCollectedText.text = "Coins Collected: " + GameManager.Instance.GetRunCoins().ToString();
            lc_totalCoinsText.text = "Total Coins: " + GameManager.Instance.totalCoins.ToString();
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

    private void OnNextLevelButtonPressed()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
    }
}