using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    // We no longer assign the player in the Inspector. It will be found automatically.
    private Transform player;

    public GameObject[] levelChunkPrefabs;
    public int initialChunkCount = 3;
    public float generationLookahead = 100f;
    public float cleanupDistance = 200f;
    private List<GameObject> spawnedChunks = new List<GameObject>();
    private Vector3 lastEndPoint;

    // When this object is enabled, it subscribes to the event.
    void OnEnable()
    {
        PlayerSpawner.OnPlayerSpawned += HandlePlayerSpawned;
    }

    // When this object is disabled, it unsubscribes to prevent errors.
    void OnDisable()
    {
        PlayerSpawner.OnPlayerSpawned -= HandlePlayerSpawned;
    }

    // This method is called by the event when the player is spawned.
    private void HandlePlayerSpawned(Transform playerTransform)
    {
        player = playerTransform;
    }

    void Start()
    {
        GameObject firstChunk = Instantiate(levelChunkPrefabs[0], Vector3.zero, Quaternion.identity, this.transform);
        spawnedChunks.Add(firstChunk);

        PlatformData firstData = firstChunk.GetComponent<PlatformData>();
        lastEndPoint = firstData.endPoint.position;

        for (int i = 0; i < initialChunkCount - 1; i++)
        {
            GenerateChunk();
        }
    }

    void Update()
    {
        // If we don't have a player reference yet, don't do anything.
        if (player == null) return;

        if (player.position.z > lastEndPoint.z - generationLookahead)
        {
            GenerateChunk();
        }
        CleanUpChunks();
    }

    // ... (GenerateChunk and CleanUpChunks methods are unchanged) ...
    #region Unchanged Code
    void GenerateChunk()
    {
        GameObject prefabToSpawn = levelChunkPrefabs[Random.Range(0, levelChunkPrefabs.Length)];
        GameObject newChunk = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.identity, this.transform);
        PlatformData data = newChunk.GetComponent<PlatformData>();
        Vector3 startPoint = data.startPoint.position;
        Vector3 endPoint = data.endPoint.position;
        Vector3 moveVector = lastEndPoint - startPoint;
        newChunk.transform.position += moveVector;
        lastEndPoint = endPoint + moveVector;
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
    #endregion
}