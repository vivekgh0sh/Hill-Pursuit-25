using UnityEngine;
using System;

public class PlayerSpawner : MonoBehaviour
{
    public static event Action<Transform> OnPlayerSpawned;

    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager not found! Cannot spawn player.");
            return;
        }

        var allCars = GameManager.Instance.allCars;
        int selectedCarIndex = GameManager.Instance.selectedCarIndex;

        GameObject carToSpawnPrefab = null;

        if (selectedCarIndex >= 0 && selectedCarIndex < allCars.Count)
        {
            carToSpawnPrefab = allCars[selectedCarIndex].carPrefab;
        }
        else
        {
            Debug.LogError("Selected Car Index is out of range! Spawning default car.");
            carToSpawnPrefab = allCars[0].carPrefab;
        }

        // Spawn the car prefab and get a reference to the new instance
        GameObject playerInstance = Instantiate(carToSpawnPrefab, transform.position, transform.rotation);

        // Announce that the player has been spawned, and pass along its Transform.
        if (OnPlayerSpawned != null)
        {
            OnPlayerSpawned(playerInstance.transform);
        }

        Destroy(gameObject);
    }
}