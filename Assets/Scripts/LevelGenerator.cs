using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    // This array will now hold your complete platform prefabs of different shapes
    public GameObject[] platformPrefabs;

    [Header("Platform Placement")]
    public float minYPosition = 0f;
    public float maxYPosition = 5f;
    public float maxHeighChange = 3f; // Max jump height between consecutive platforms

    [Header("Gap Settings")]
    [Range(2, 10)]
    public float minGapLength = 3;
    [Range(5, 15)]
    public float maxGapLength = 10;

    [Header("Generation Control")]
    public int initialPlatformCount = 5;
    public int generationLookahead = 50;

    private List<GameObject> spawnedPlatforms = new List<GameObject>();
    private Vector3 nextSpawnPoint = Vector3.zero;
    private float lastPlatformY = 0f;
    private float lastCleanupZ = 0f;

    void Start()
    {
        if (platformPrefabs.Length == 0)
        {
            Debug.LogError("No platform prefabs assigned in the LevelGenerator!");
            return;
        }

        // Set the initial Y position
        lastPlatformY = nextSpawnPoint.y;

        // Spawn the starting platforms
        for (int i = 0; i < initialPlatformCount; i++)
        {
            GenerateSegment();
        }
    }

    void Update()
    {
        if (player.position.z > nextSpawnPoint.z - generationLookahead)
        {
            GenerateSegment();
        }

        if (player.position.z - lastCleanupZ > 30f)
        {
            CleanUpPlatforms();
            lastCleanupZ = player.position.z;
        }
    }

    void GenerateSegment()
    {
        // 1. Pick a random platform prefab from our array
        GameObject prefabToSpawn = platformPrefabs[Random.Range(0, platformPrefabs.Length)];

        // Get the length of the chosen prefab from its collider bounds. This is KEY.
        float platformLength = prefabToSpawn.GetComponent<Collider>().bounds.size.z;

        // 2. Calculate a new Y position for this platform
        float randomYOffset = Random.Range(-maxHeighChange, maxHeighChange);
        float newY = Mathf.Clamp(lastPlatformY + randomYOffset, minYPosition, maxYPosition);

        // 3. Calculate the actual spawn position.
        // We add half the platform's length because its pivot is in the center.
        Vector3 spawnPosition = new Vector3(
            nextSpawnPoint.x,
            newY,
            nextSpawnPoint.z + (platformLength / 2)
        );

        // 4. Instantiate the platform and add it to our list
        GameObject newPlatform = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        spawnedPlatforms.Add(newPlatform);

        // 5. Update variables for the next segment
        lastPlatformY = newY; // Remember the height of this platform
        float gapLength = Random.Range(minGapLength, maxGapLength);

        // The next spawn point is at the end of this platform, plus the gap
        nextSpawnPoint.z += platformLength + gapLength;
    }

    void CleanUpPlatforms()
    {
        for (int i = spawnedPlatforms.Count - 1; i >= 0; i--)
        {
            if (spawnedPlatforms[i] == null)
            {
                spawnedPlatforms.RemoveAt(i);
                continue;
            }

            if (player.position.z - spawnedPlatforms[i].transform.position.z > 100f) // Increased cleanup distance
            {
                GameObject platformToDestroy = spawnedPlatforms[i];
                spawnedPlatforms.RemoveAt(i);
                Destroy(platformToDestroy);
            }
        }
    }
}