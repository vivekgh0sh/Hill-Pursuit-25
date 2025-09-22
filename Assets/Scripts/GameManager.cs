using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState { MainMenu, Playing, LevelComplete, GameOver }
    public GameState currentState;

    [Header("Player Score")]
    public Transform playerTransform;
    private int coinsCollectedThisRun = 0;
    private float distanceTraveled = 0f;
    private float startingZPosition;

    [Header("Saved Data")]
    public int totalCoins = 0;
    public int highscore = 0;
    public List<CarData> allCars;
    public int selectedCarIndex = 0;

    [Header("Level Data")]
    public List<GameObject> allLevelChunks; // Assign your level chunk prefabs here
    public int highestLevelUnlocked = 0; // Level 0 is unlocked by default
    private int currentLevelIndex = -1;

    void Awake() { if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); LoadGameData(); } else { Destroy(gameObject); } }
    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "EndlessScene" || scene.name == "LevelScene")
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;
            coinsCollectedThisRun = 0;
            distanceTraveled = 0f;
        }
        else { currentState = GameState.MainMenu; }
    }

    void Update() { if (currentState == GameState.Playing && playerTransform != null) { distanceTraveled = playerTransform.position.z - startingZPosition; } }
    public void RegisterPlayer(Transform player) { playerTransform = player; startingZPosition = player.position.z; }
    public void CollectCoin() { if (currentState != GameState.Playing) return; coinsCollectedThisRun++; }
    public int GetCurrentDistance() { return Mathf.Max(0, Mathf.FloorToInt(distanceTraveled)); }
    public int GetRunCoins() { return coinsCollectedThisRun; }
    public void EndGame() { if (currentState != GameState.Playing) return; currentState = GameState.GameOver; Time.timeScale = 0f; int finalDistance = GetCurrentDistance(); if (finalDistance > highscore) { highscore = finalDistance; } totalCoins += coinsCollectedThisRun; SaveGameData(); GameplayUIController uiController = FindFirstObjectByType<GameplayUIController>(); if (uiController != null) { uiController.ShowGameOverScreen(); } }
    public void GoToMenu() { Time.timeScale = 1f; SceneManager.LoadScene("VehicleSelectionUI"); }
    public void GoToLevelSelect() { Time.timeScale = 1f; SceneManager.LoadScene("LevelSelectUI"); }
    public void StartEndlessMode() { Time.timeScale = 1f; SceneManager.LoadScene("EndlessScene"); }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < allLevelChunks.Count)
        {
            currentLevelIndex = levelIndex;
            Time.timeScale = 1f;
            SceneManager.LoadScene("LevelScene"); // The universal gameplay scene
        }
    }

    public GameObject GetCurrentLevelPrefab()
    {
        if (currentLevelIndex >= 0 && currentLevelIndex < allLevelChunks.Count)
        {
            return allLevelChunks[currentLevelIndex];
        }
        return null;
    }

    public void LevelCompleted()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.LevelComplete;
        Time.timeScale = 0f;

        if (currentLevelIndex + 1 > highestLevelUnlocked && currentLevelIndex + 1 < allLevelChunks.Count)
        {
            highestLevelUnlocked = currentLevelIndex + 1;
        }

        totalCoins += coinsCollectedThisRun;
        SaveGameData();

        // Find the UI controller and tell it to show the level complete screen
        GameplayUIController uiController = FindFirstObjectByType<GameplayUIController>();
        if (uiController != null)
        {
            uiController.ShowLevelCompleteScreen();
        }
    }

    // Add this new method
    public void LoadNextLevel()
    {
        int nextLevel = currentLevelIndex + 1;
        // Check if there is a next level
        if (nextLevel < allLevelChunks.Count)
        {
            StartLevel(nextLevel);
        }
        else
        {
            // If that was the last level, just go to the level select screen
            Debug.Log("Last level completed!");
            GoToLevelSelect();
        }
    }

    public void SaveGameData() { PlayerPrefs.SetInt("TotalCoins", totalCoins); PlayerPrefs.SetInt("Highscore", highscore); PlayerPrefs.SetInt("SelectedCarIndex", selectedCarIndex); PlayerPrefs.SetInt("HighestLevelUnlocked", highestLevelUnlocked); PlayerPrefs.Save(); }
    public void LoadGameData() { totalCoins = PlayerPrefs.GetInt("TotalCoins", 0); highscore = PlayerPrefs.GetInt("Highscore", 0); selectedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0); highestLevelUnlocked = PlayerPrefs.GetInt("HighestLevelUnlocked", 0); }
    public bool CanAfford(int cost) { return totalCoins >= cost; }
    public void SpendCoins(int amount) { totalCoins -= amount; }
    public void UnlockCar(string carID) { PlayerPrefs.SetInt("CarUnlocked_" + carID, 1); }
    public bool IsCarUnlocked(string carID) { CarData car = allCars.Find(c => c.carID == carID); if (car != null && car.unlockCost == 0) return true; return PlayerPrefs.GetInt("CarUnlocked_" + carID, 0) == 1; }
}