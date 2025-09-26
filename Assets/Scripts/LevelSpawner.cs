using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    void Start()
    {
        GameObject levelPrefab = GameManager.Instance.GetCurrentLevelPrefab();
        if (levelPrefab == null)
        {
            Debug.LogError("Could not find level prefab to spawn!");
            return;
        }

        GameObject levelInstance = Instantiate(levelPrefab, Vector3.zero, Quaternion.identity);

        CarData selectedCarData = GameManager.Instance.allCars[GameManager.Instance.selectedCarIndex];

        Transform startPoint = levelInstance.GetComponent<PlatformData>().startPoint;
        GameObject playerInstance = Instantiate(selectedCarData.carPrefab, startPoint.position + Vector3.up * 2f, startPoint.rotation);

        FindFirstObjectByType<CameraManager>()?.AssignFollowTarget(playerInstance.transform);

        PlayerSpawner.InvokeOnPlayerSpawned(playerInstance.transform);
    }
}