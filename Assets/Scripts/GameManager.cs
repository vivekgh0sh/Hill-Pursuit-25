using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI Reference")]
    public GameObject gameOverPanel;
    public bool isGameOver = false;
    public static GameManager instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        isGameOver = false;
        if(gameOverPanel != null)
        {
            gameObject.SetActive(false);
        }
        Time.timeScale = 1f;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver=true;
        Debug.Log("GameOver");

        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void Restart()
    {
        Time.timeScale=1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
