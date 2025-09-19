using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { MainMenu, Playing, GameOver }
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

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() { SceneManager.sceneLoaded += OnSceneLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnSceneLoaded; }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene")
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;
            coinsCollectedThisRun = 0;
            distanceTraveled = 0f;
        }
        else
        {
            currentState = GameState.MainMenu;
        }
    }

    void Update()
    {
        if (currentState == GameState.Playing && playerTransform != null)
        {
            distanceTraveled = playerTransform.position.z - startingZPosition;
        }
    }

    public void RegisterPlayer(Transform player)
    {
        playerTransform = player;
        startingZPosition = player.position.z;
    }

    public void CollectCoin()
    {
        if (currentState != GameState.Playing) return;
        coinsCollectedThisRun++;
    }

    public int GetCurrentDistance() { return Mathf.Max(0, Mathf.FloorToInt(distanceTraveled)); }
    public int GetRunCoins() { return coinsCollectedThisRun; }

    public void EndGame()
    {
        if (currentState != GameState.Playing) return;
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        int finalDistance = GetCurrentDistance();
        if (finalDistance > highscore) { highscore = finalDistance; }
        totalCoins += coinsCollectedThisRun;
        SaveGameData();
        GameplayUIController uiController = FindObjectOfType<GameplayUIController>();
        if (uiController != null) { uiController.ShowGameOverScreen(); }
    }

    public void GoToMenu() { Time.timeScale = 1f; SceneManager.LoadScene("VehicleSelectionUI"); }
    public void StartEndlessMode() { Time.timeScale = 1f; SceneManager.LoadScene("SampleScene"); }

    // --- NEW METHODS FOR THE SHOWROOM ---
    public bool CanAfford(int cost) { return totalCoins >= cost; }
    public void SpendCoins(int amount) { totalCoins -= amount; }
    public void UnlockCar(string carID) { PlayerPrefs.SetInt("CarUnlocked_" + carID, 1); }
    public bool IsCarUnlocked(string carID)
    {
        CarData car = allCars.Find(c => c.carID == carID);
        if (car != null && car.unlockCost == 0) return true; // Cars that cost 0 are unlocked by default
        return PlayerPrefs.GetInt("CarUnlocked_" + carID, 0) == 1;
    }
    // --- END OF NEW METHODS ---

    public void SaveGameData()
    {
        PlayerPrefs.SetInt("TotalCoins", totalCoins);
        PlayerPrefs.SetInt("Highscore", highscore);
        PlayerPrefs.SetInt("SelectedCarIndex", selectedCarIndex);
        PlayerPrefs.Save();
    }

    public void LoadGameData()
    {
        totalCoins = PlayerPrefs.GetInt("TotalCoins", 0);
        highscore = PlayerPrefs.GetInt("Highscore", 0);
        selectedCarIndex = PlayerPrefs.GetInt("SelectedCarIndex", 0);
    }
}