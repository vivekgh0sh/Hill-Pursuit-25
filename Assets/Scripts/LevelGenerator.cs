using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    // We now use an array of cube prefabs for variety
    public GameObject[] cubePrefabs;

    [Header("Platform Settings")]
    [Range(3, 20)]
    public int minPlatformLength = 5; // Min number of cubes in a platform
    [Range(5, 30)]
    public int maxPlatformLength = 15; // Max number of cubes

    [Header("Gap Settings")]
    [Range(1, 10)]
    public int minGapLength = 2; // Min empty space between platforms
    [Range(3, 15)]
    public int maxGapLength = 8; // Max empty space

    [Header("Generation Control")]
    public int initialPlatformCount = 3; // Number of platforms to spawn at the start
    public int generationLookahead = 40; // How far ahead of the player to spawn new platforms

    // We still need to track spawned objects for cleanup
    private List<GameObject> spawnedCubes = new List<GameObject>();
    private Vector3 nextSpawnPoint = Vector3.zero;
    private float lastCleanupZ = 0f;

    void Start()
    {
        if (cubePrefabs.Length == 0)
        {
            Debug.LogError("No cube prefabs assigned in the LevelGenerator!");
            return;
        }

        // Spawn the initial runway for the player
        for (int i = 0; i < initialPlatformCount; i++)
        {
            GenerateSegment();
        }
    }

    void Update()
    {
        // Check if the player has moved far enough to trigger new generation
        if (player.position.z > nextSpawnPoint.z - generationLookahead)
        {
            GenerateSegment();
        }

        // Clean up cubes that are far behind the player
        // We do this less frequently than every frame for performance
        if (player.position.z - lastCleanupZ > 20f)
        {
            CleanUpCubes();
            lastCleanupZ = player.position.z;
        }
    }

    void GenerateSegment()
    {
        // 1. Decide on the length of the next solid platform
        int platformLength = Random.Range(minPlatformLength, maxPlatformLength);

        // 2. Build the platform by spawning cubes one by one
        for (int i = 0; i < platformLength; i++)
        {
            // Pick a random cube prefab from our array for visual variety
            GameObject prefabToSpawn = cubePrefabs[Random.Range(0, cubePrefabs.Length)];

            // Instantiate the cube and add it to our list for cleanup
            GameObject newCube = Instantiate(prefabToSpawn, nextSpawnPoint, Quaternion.identity);
            spawnedCubes.Add(newCube);

            // Move the spawn point forward for the next cube
            nextSpawnPoint.z += 1; // Assumes cubes are 1 unit long
        }

        // 3. Decide on the length of the gap after the platform
        int gapLength = Random.Range(minGapLength, maxGapLength);

        // 4. Move the spawn point forward to create the empty space
        nextSpawnPoint.z += gapLength;
    }

    void CleanUpCubes()
    {
        // Use a for loop backwards because we are removing items from the list
        for (int i = spawnedCubes.Count - 1; i >= 0; i--)
        {
            if (spawnedCubes[i] == null)
            {
                spawnedCubes.RemoveAt(i);
                continue;
            }

            if (player.position.z - spawnedCubes[i].transform.position.z > 50f)
            {
                GameObject cubeToDestroy = spawnedCubes[i];
                spawnedCubes.RemoveAt(i);
                Destroy(cubeToDestroy);
            }
        }
    }
}