using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject levelButtonPrefab;
    public Transform buttonContainer;
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(() => GameManager.Instance.GoToMenu());
        PopulateLevels();
    }

    void PopulateLevels()
    {
        // Clear any old buttons
        foreach (Transform child in buttonContainer) { Destroy(child.gameObject); }

        int highestUnlocked = GameManager.Instance.highestLevelUnlocked;

        // Create a button for each level chunk defined in the GameManager
        for (int i = 0; i < GameManager.Instance.allLevelChunks.Count; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);

            // Set the button's text
            buttonGO.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();

            bool isUnlocked = (i <= highestUnlocked);
            buttonGO.GetComponent<Button>().interactable = isUnlocked;

            // Add a listener to the button
            int levelIndex = i; // Important to copy the value for the lambda
            buttonGO.GetComponent<Button>().onClick.AddListener(() => GameManager.Instance.StartLevel(levelIndex));
        }
    }
}