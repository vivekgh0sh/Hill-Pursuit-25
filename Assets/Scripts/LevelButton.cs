using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Button button;

    public void Setup(int levelIndex, bool isUnlocked)
    {
        levelText.text = (levelIndex + 1).ToString();
        button.interactable = isUnlocked;

        if (isUnlocked)
        {
            button.onClick.AddListener(() => GameManager.Instance.StartLevel(levelIndex));
        }
    }
}