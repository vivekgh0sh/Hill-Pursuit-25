using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public GameObject[] levelChunkPrefabs;

    [Header("Generation Control")]
    public int initialChunkCount = 3;
    
    // --- THIS IS THE CORRECTED LINE ---
    public float generationLookahead = 100f; // Changed from int to float
    
    public float cleanupDistance = 200f;

    private List<GameObject> spawnedChunks = new List<GameObject>();
    private Vector3 lastEndPoint;

    void Start()
    {
        if (levelChunkPrefabs == null || levelChunkPrefabs.Length == 0)
        {
            Debug.LogError("Level Chunk Prefabs array is not assigned or is empty!");
            return;
        }

        // Use the first chunk in the array as the starting one.
        GameObject firstChunk = Instantiate(levelChunkPrefabs[0], Vector3.zero, Quaternion.identity, this.transform);
        spawnedChunks.Add(firstChunk);
        
        PlatformData firstData = firstChunk.GetComponent<PlatformData>();
        lastEndPoint = firstData.endPoint.position;

        // Place the player at the start of that chunk.
        player.position = firstData.startPoint.position + Vector3.up * 2f;
        player.rotation = Quaternion.identity;

        for (int i = 0; i < initialChunkCount - 1; i++)
        {
            GenerateChunk();
        }
    }

    void Update()
    {
        if (player.position.z > lastEndPoint.z - generationLookahead)
        {
            GenerateChunk();
        }
        CleanUpChunks();
    }

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
            if (spawnedChunks[i] == null)
            {
                spawnedChunks.RemoveAt(i);
                continue;
            }

            // Use the chunk's root position for a simpler check
            float chunkPositionZ = spawnedChunks[i].transform.position.z;
            if (player.position.z - chunkPositionZ > cleanupDistance)
            {
                Destroy(spawnedChunks[i]);
                spawnedChunks.RemoveAt(i);
            }
        }
    }
}