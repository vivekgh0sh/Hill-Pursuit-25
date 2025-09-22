using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    void Start()
    {
        // 1. Get the correct level chunk prefab from the GameManager
        GameObject levelPrefab = GameManager.Instance.GetCurrentLevelPrefab();
        if (levelPrefab == null)
        {
            Debug.LogError("Could not find level prefab to spawn!");
            return;
        }

        // 2. Spawn the level chunk at the origin
        GameObject levelInstance = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);

        // 3. Get the selected car data from the GameManager
        CarData selectedCarData = GameManager.Instance.allCars[GameManager.Instance.selectedCarIndex];

        // 4. Find the level's start point and spawn the player there
        Transform startPoint = levelInstance.GetComponent<PlatformData>().startPoint;
        GameObject playerInstance = Instantiate(selectedCarData.carPrefab, startPoint.position + Vector3.up * 2f, startPoint.rotation);

        // 5. Tell the camera to follow the new player
        FindFirstObjectByType<CameraManager>()?.AssignFollowTarget(playerInstance.transform);
    }
}