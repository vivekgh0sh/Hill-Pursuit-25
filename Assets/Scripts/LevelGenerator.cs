using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    // --- (All your public variables are the same) ---
    private Transform player;
    public GameObject[] levelChunkPrefabs;
    public int initialChunkCount = 3;
    public float generationLookahead = 100f;
    public float cleanupDistance = 200f;
    private List<GameObject> spawnedChunks = new List<GameObject>();
    private Vector3 lastEndPoint;

    void OnEnable() { PlayerSpawner.OnPlayerSpawned += HandlePlayerSpawned; }
    void OnDisable() { PlayerSpawner.OnPlayerSpawned -= HandlePlayerSpawned; }

    private void HandlePlayerSpawned(Transform playerTransform) { player = playerTransform; }

    void Start()
    {
        // Spawn the very first chunk using our new, safe method.
        SpawnChunkForEndless(true);

        // Spawn the rest of the initial chunks.
        for (int i = 0; i < initialChunkCount - 1; i++)
        {
            SpawnChunkForEndless(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.z > lastEndPoint.z - generationLookahead)
        {
            // Spawn subsequent chunks using the same safe method.
            SpawnChunkForEndless(false);
        }
        CleanUpChunks();
    }

    // --- THIS IS THE NEW HELPER METHOD ---
    // This is now the ONLY way chunks are created in endless mode.
    private void SpawnChunkForEndless(bool isFirstChunk)
    {
        GameObject prefabToSpawn = isFirstChunk ? levelChunkPrefabs[0] : levelChunkPrefabs[Random.Range(0, levelChunkPrefabs.Length)];

        // Use Vector3.zero for the first chunk, otherwise calculate position.
        Vector3 spawnPos = isFirstChunk ? Vector3.zero : new Vector3(0, 0, 10000); // Temp position
        GameObject newChunk = Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, this.transform);

        // CRITICAL: Immediately find and destroy any finish trigger.
        LevelFinishTrigger finishTrigger = newChunk.GetComponentInChildren<LevelFinishTrigger>();
        if (finishTrigger != null)
        {
            Destroy(finishTrigger.gameObject);
        }

        // Now, correctly position the chunk.
        PlatformData data = newChunk.GetComponent<PlatformData>();
        Vector3 startPoint = data.startPoint.position;
        Vector3 endPoint = data.endPoint.position;

        if (!isFirstChunk)
        {
            Vector3 moveVector = lastEndPoint - startPoint;
            newChunk.transform.position += moveVector;
            // Update the end point based on the moved position
            lastEndPoint = endPoint + moveVector;
        }
        else
        {
            // For the first chunk, just record its natural end point.
            lastEndPoint = endPoint;
        }

        spawnedChunks.Add(newChunk);
    }

    void CleanUpChunks()
    {
        for (int i = spawnedChunks.Count - 1; i >= 0; i--)
        {
            if (spawnedChunks[i] == null) { spawnedChunks.RemoveAt(i); continue; }
            float chunkPositionZ = spawnedChunks[i].transform.position.z;
            if (player.position.z - chunkPositionZ > cleanupDistance) { Destroy(spawnedChunks[i]); spawnedChunks.RemoveAt(i); }
        }
    }
}