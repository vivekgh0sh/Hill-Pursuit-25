using UnityEngine;

public class LevelFinishTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player
        if (other.GetComponent<CarController>() != null)
        {
            Debug.Log("Player reached the finish line!");
            GameManager.Instance.LevelCompleted();
            // Disable the trigger to prevent it from firing multiple times
            this.gameObject.SetActive(false);
        }
    }
}