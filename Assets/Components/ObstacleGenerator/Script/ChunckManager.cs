using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs;
    public int activeChunks = 10;
    public float moveSpeed = 5f;

    private Queue<GameObject> chunks = new Queue<GameObject>();
    private int lastChunkIndex = -1;

    void Start()
    {
        // Spawn initial : 10 chunks consécutifs
        for (int i = 0; i < activeChunks; i++)
        {
            SpawnNextChunk();
        }
    }

    void Update()
    {
        MoveChunks();
        RecycleChunks();
    }

    void MoveChunks()
    {
        foreach (var chunk in chunks)
        {
            chunk.transform.position += Vector3.back * moveSpeed * Time.deltaTime; // adapte selon ton axe
        }
    }

    void RecycleChunks()
    {
        if (chunks.Count == 0) return;

        GameObject firstChunk = chunks.Peek();

        if (firstChunk.transform.position.z < -20f) // sortie de l'écran
        {
            chunks.Dequeue();           // retire chunk
            Destroy(firstChunk);        // ou recycler si tu veux

            SpawnNextChunk();           // spawn après le dernier chunk
        }
    }

    void SpawnNextChunk()
    {
        GameObject prefab = GetRandomPrefab();
        Vector3 spawnPos = Vector3.zero;

        // Si on a déjà des chunks, spawn après le dernier
        if (chunks.Count > 0)
        {
            GameObject lastChunk = null;
            foreach (var c in chunks) lastChunk = c; // dernier dans la queue

            Transform end = lastChunk.transform.Find("EndPoint");
            if (end != null)
                spawnPos = end.position;
            else
                Debug.LogError("Dernier chunk n'a pas d'EndPoint !");
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