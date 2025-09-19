using UnityEngine;
public class PlayerRegistration : MonoBehaviour
{
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterPlayer(transform);
        }
    }
}