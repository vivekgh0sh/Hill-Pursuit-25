using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VehicleSelectionManager : MonoBehaviour
{
    [Header("Showroom References")]
    public Transform showroomCarAnchor;
    public float rotationSpeed = 20f;

    [Header("UI Elements")]
    public Button leftButton;
    public Button rightButton;
    public Button unlockButton;
    public Button startButton;
    public Button stageButton;
    public TextMeshProUGUI totalCoinsText;
    public TextMeshProUGUI carNameText;
    public TextMeshProUGUI unlockCostText;

    private int currentCarIndex;
    private GameObject currentCarInstance;
    private CarData currentCarData;

    void Start()
    {
        if (GameManager.Instance == null) return;

        currentCarIndex = GameManager.Instance.selectedCarIndex;

        leftButton.onClick.AddListener(PreviousCar);
        rightButton.onClick.AddListener(NextCar);
        unlockButton.onClick.AddListener(UnlockCurrentCar);
        startButton.onClick.AddListener(StartGame);
        stageButton.onClick.AddListener(GoToLevelSelect);


        DisplayCar();
    }

    void Update()
    {
        if (currentCarInstance != null)
        {
            currentCarInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }
        totalCoinsText.text = GameManager.Instance.totalCoins.ToString();
    }

    void DisplayCar()
    {
        currentCarData = GameManager.Instance.allCars[currentCarIndex];

        if (currentCarInstance != null) { Destroy(currentCarInstance); }

        currentCarInstance = Instantiate(currentCarData.carPrefab, showroomCarAnchor);
        currentCarInstance.transform.localPosition = currentCarData.displayPositionOffset;
        currentCarInstance.transform.localRotation = Quaternion.Euler(currentCarData.displayRotation);
        currentCarInstance.transform.localScale = Vector3.one * currentCarData.displayScale;

        // Disable physics components for showroom display
        if (currentCarInstance.GetComponent<Rigidbody>() != null) { currentCarInstance.GetComponent<Rigidbody>().isKinematic = true; }
        if (currentCarInstance.GetComponent<CarController>() != null) { currentCarInstance.GetComponent<CarController>().enabled = false; }

        carNameText.text = currentCarData.carName;
        bool isUnlocked = GameManager.Instance.IsCarUnlocked(currentCarData.carID);

        unlockButton.gameObject.SetActive(!isUnlocked);
        startButton.interactable = isUnlocked;

        if (!isUnlocked)
        {
            unlockCostText.text = currentCarData.unlockCost.ToString();
        }
    }

    public void NextCar()
    {
        currentCarIndex++;
        if (currentCarIndex >= GameManager.Instance.allCars.Count) { currentCarIndex = 0; }
        DisplayCar();
    }

    public void PreviousCar()
    {
        currentCarIndex--;
        if (currentCarIndex < 0) { currentCarIndex = GameManager.Instance.allCars.Count - 1; }
        DisplayCar();
    }

    void UnlockCurrentCar()
    {
        if (GameManager.Instance.CanAfford(currentCarData.unlockCost))
        {
            GameManager.Instance.SpendCoins(currentCarData.unlockCost);
            GameManager.Instance.UnlockCar(currentCarData.carID);
            DisplayCar(); // Refresh UI to show the unlocked state
        }
    }

    public void StartGame()
    {
        GameManager.Instance.selectedCarIndex = currentCarIndex;
        GameManager.Instance.SaveGameData();
        GameManager.Instance.StartEndlessMode();
    }

    public void GoToLevelSelect()
    {
        GameManager.Instance.selectedCarIndex = currentCarIndex;
        GameManager.Instance.SaveGameData();
        GameManager.Instance.GoToLevelSelect();
    }
}