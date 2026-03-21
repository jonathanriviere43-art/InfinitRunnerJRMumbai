using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs;
    public int activeChunks = 10;
    public float moveSpeed = 2f;

    [Header("Player Reference")]
    [SerializeField] private PlayerMouvement player;

    private Queue<GameObject> chunks = new Queue<GameObject>();
    private int lastChunkIndex = -1;

    void Start()
    {
        for (int i = 0; i < activeChunks; i++)
        {
            SpawnNextChunk();
        }
    }

    void Update()
    {
        UpdateSpeed();
        MoveChunks();
        RecycleChunks();
    }

    void UpdateSpeed()
    {
        if (player == null) return;

        moveSpeed = player.GetForwardSpeed();
    }

    void MoveChunks()
    {
        foreach (var chunk in chunks)
        {
            if (chunk == null) continue;
            chunk.transform.position += Vector3.back * moveSpeed * Time.deltaTime;
        }
    }

    void RecycleChunks()
    {
        if (chunks.Count == 0) return;

        GameObject firstChunk = chunks.Peek();
        if (firstChunk == null) return;

        if (firstChunk.transform.position.z < -20f)
        {
            chunks.Dequeue();
            Destroy(firstChunk);
            SpawnNextChunk();
        }
    }

    void SpawnNextChunk()
    {
        GameObject prefab = GetRandomPrefab();
        if (prefab == null) return;

        Vector3 spawnPos = Vector3.zero;

        if (chunks.Count > 0)
        {
            GameObject lastChunk = null;
            foreach (var c in chunks) lastChunk = c;

            if (lastChunk != null)
            {
                Transform end = lastChunk.transform.Find("EndPoint");
                if (end != null)
                    spawnPos = end.position;
                else
                    Debug.LogError("Dernier chunk n'a pas d'EndPoint !");
            }
        }

        GameObject chunk = Instantiate(prefab, spawnPos, Quaternion.identity);
        chunks.Enqueue(chunk);
    }

    GameObject GetRandomPrefab()
    {
        if (chunkPrefabs.Count == 0) return null;

        int index;
        do
        {
            index = Random.Range(0, chunkPrefabs.Count);
        } while (index == lastChunkIndex && chunkPrefabs.Count > 1);

        lastChunkIndex = index;
        return chunkPrefabs[index];
    }
}