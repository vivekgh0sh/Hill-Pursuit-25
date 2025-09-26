using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameplayUIController : MonoBehaviour
{
    private CarController carController;

    [Header("UI Panels")]
    public GameObject gameplayPanel;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public GameObject pausePanel; // New

    [Header("Gameplay UI")]
    public TextMeshProUGUI gameplayDistanceText;
    public TextMeshProUGUI gameplayCoinText;
    public Button pauseButton; // New

    [Header("Control Buttons")]
    public Button jumpButton;
    public Image jumpButtonImage;
    public Button boostButton;
    public Image boostButtonImage;

    [Header("Pause Panel UI")]
    public Button resumeButton;
    public Button pauseMenuButton; 


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

    void OnEnable() { PlayerSpawner.OnPlayerSpawned += OnPlayerSpawned; }
    void OnDisable() { PlayerSpawner.OnPlayerSpawned -= OnPlayerSpawned; }

    void OnPlayerSpawned(Transform playerTransform)
    {
        carController = playerTransform.GetComponent<CarController>();
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        pausePanel.SetActive(false);
        gameplayPanel.SetActive(true);

        // Standard Button Listeners
        restartButton.onClick.AddListener(OnRestartButtonPressed);
        menuButton.onClick.AddListener(OnMenuButtonPressed);
        nextLevelButton.onClick.AddListener(OnNextLevelButtonPressed);
        menuButtonLevelComplete.onClick.AddListener(OnMenuButtonPressed);

        // New Gameplay Button Listeners
        pauseButton.onClick.AddListener(OnPauseButtonPressed);
        resumeButton.onClick.AddListener(OnResumeButtonPressed);
        pauseMenuButton.onClick.AddListener(OnMenuButtonPressed);
        jumpButton.onClick.AddListener(OnJumpButtonPressed);
        boostButton.onClick.AddListener(OnBoostButtonPressed);

    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameManager.GameState.Playing)
        {
            gameplayDistanceText.text = GameManager.Instance.GetCurrentDistance().ToString();
            gameplayCoinText.text = GameManager.Instance.GetRunCoins().ToString();

            if (carController != null)
            {
                UpdateJumpButtonVisual();
                UpdateBoostButtonVisual();
            }
        }
    }

    void UpdateJumpButtonVisual()
    {
        bool canJump = carController.GetJumpsLeft() > 0;
        float alpha = canJump ? 1f : 0.5f;
        jumpButtonImage.color = new Color(jumpButtonImage.color.r, jumpButtonImage.color.g, jumpButtonImage.color.b, alpha);
    }

    void UpdateBoostButtonVisual()
    {
        float progress = carController.GetBoostCooldownProgress();
        bool canBoost = (progress >= 1f);
        float alpha = canBoost ? 1f : 0.5f;
        boostButtonImage.color = new Color(boostButtonImage.color.r, boostButtonImage.color.g, boostButtonImage.color.b, alpha);
        boostButton.interactable = canBoost;
    }

    // --- Button Actions ---
    void OnJumpButtonPressed()
    {

    }

    void OnBoostButtonPressed()
    {
    }
    void OnPauseButtonPressed() { GameManager.Instance.PauseGame(); pausePanel.SetActive(true); }
    void OnResumeButtonPressed() { GameManager.Instance.ResumeGame(); pausePanel.SetActive(false); }

    public void ShowGameOverScreen() { gameplayPanel.SetActive(false); gameOverPanel.SetActive(true); }
    public void ShowLevelCompleteScreen() { gameplayPanel.SetActive(false); levelCompletePanel.SetActive(true); if (GameManager.Instance != null) { lc_coinsCollectedText.text = "Coins Collected: " + GameManager.Instance.GetRunCoins().ToString(); lc_totalCoinsText.text = "Total Coins: " + GameManager.Instance.totalCoins.ToString(); } }
    public void OnRestartButtonPressed()
    {
        Time.timeScale = 1f; 
        if (GameManager.Instance != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
    public void OnMenuButtonPressed()
    {
        Time.timeScale = 1f; 
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GoToMenu();
        }
    }
    public void OnNextLevelButtonPressed()
    {
        Time.timeScale = 1f;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.LoadNextLevel();
        }
    }

    public void ShowGameOverScreen(int finalDistance, int runCoins, int newHighscore, int newTotalCoins)
    {
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);

        // We no longer need to ask the GameManager, we use the values we were given.
        finalDistanceText.text = "Distance: " + finalDistance.ToString();
        finalCoinsText.text = "Coins This Run: " + runCoins.ToString();
        highscoreText.text = "Highscore: " + newHighscore.ToString();
        totalCoinsText.text = "Total Coins: " + newTotalCoins.ToString();
    }
}