using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelSelectManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject levelButtonPrefab;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private Button backButton;
    [SerializeField] private Image background;

    [Header("Pagination")]
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button prevPageButton;
    [SerializeField] private TextMeshProUGUI pageTitleText;
    [SerializeField] private int levelsPerPage = 25;

    [Header("Phase Data")]
    [Tooltip("Assign your PhaseData assets here in order (Phase 1, Phase 2, etc.).")]
    [SerializeField] private List<PhaseData> phases;

    private int currentPage = 0;
    private int totalPages = 0;

    void Start()
    {
        if (GameManager.Instance == null) { Debug.LogError("GameManager not found!"); return; }

        int totalLevels = GameManager.Instance.allLevelChunks.Count;
        totalPages = Mathf.CeilToInt((float)totalLevels / levelsPerPage);

        nextPageButton.onClick.AddListener(NextPage);
        prevPageButton.onClick.AddListener(PreviousPage);
        backButton.onClick.AddListener(() => GameManager.Instance.GoToMenu());

        DisplayCurrentPage();
    }

    private void DisplayCurrentPage()
    {
        foreach (Transform child in buttonContainer) { Destroy(child.gameObject); }

        int startLevelIndex = currentPage * levelsPerPage;
        int endLevelIndex = Mathf.Min(startLevelIndex + levelsPerPage - 1, GameManager.Instance.allLevelChunks.Count - 1);
        int highestUnlocked = GameManager.Instance.highestLevelUnlocked;

        for (int i = startLevelIndex; i <= endLevelIndex; i++)
        {
            GameObject buttonGO = Instantiate(levelButtonPrefab, buttonContainer);
            LevelButton levelButton = buttonGO.GetComponent<LevelButton>();
            bool isUnlocked = (i <= highestUnlocked);
            levelButton.Setup(i, isUnlocked);
        }

        UpdatePageVisuals();
    }

    private void UpdatePageVisuals()
    {
        if (phases != null && currentPage < phases.Count)
        {
            PhaseData currentPhaseData = phases[currentPage];
            if (pageTitleText != null) { pageTitleText.text = currentPhaseData.phaseName; }
            if (background != null && currentPhaseData.backgroundImage != null)
            {
                background.sprite = currentPhaseData.backgroundImage;
            }
        }
        else
        {
            if (pageTitleText != null) { pageTitleText.text = $"Page {currentPage + 1}"; }
        }

        prevPageButton.gameObject.SetActive(currentPage > 0);
        nextPageButton.gameObject.SetActive(currentPage < totalPages - 1);
    }

    public void NextPage() { if (currentPage < totalPages - 1) { currentPage++; DisplayCurrentPage(); } }
    public void PreviousPage() { if (currentPage > 0) { currentPage--; DisplayCurrentPage(); } }
}