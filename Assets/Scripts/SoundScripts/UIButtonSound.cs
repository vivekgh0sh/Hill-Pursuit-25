using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UIButtonSound : MonoBehaviour
{
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(PlayClickSound);
    }

    void PlayClickSound()
    {
        // Call the AudioManager to play the click sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX("Click");
        }
    }
}
