using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<GameObject> chunkPrefabs;
    public int activeChunks = 10;
    public float moveSpeed = 2f;

    [Header("Player Reference")]
    [SerializeField] private PlayerMouvement player;

    [Header("Capsule Settings")]
    public GameObject capsulePrefab;
    public float capsuleSpawnHeight = 1f;
    public float[] allowedX = { -3f, 0f, 3f };
    public string[] obstacleTags = { "Obstacle", "ObstacleWood" };

    [Header("Speed Progression")]
    [SerializeField] private float increaseInterval = 30f;
    [SerializeField] private float speedMultiplierStep = 0.1f;

    private float timer = 0f;
    private float speedMultiplier = 1f;

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
        HandleSpeedProgression();
        MoveChunks();
        RecycleChunks();
    }

    void UpdateSpeed()
    {
        if (player == null) return;

        moveSpeed = player.GetForwardSpeed() * speedMultiplier;
    }

    void HandleSpeedProgression()
    {
        timer += Time.deltaTime;

        if (timer >= increaseInterval)
        {
            timer = 0f;
            speedMultiplier += speedMultiplierStep;

            Debug.Log("🔥 Speed multiplier: " + speedMultiplier);
        }
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

        TrySpawnCapsule(chunk);
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

    void TrySpawnCapsule(GameObject chunk)
    {
        if (capsulePrefab == null || chunk == null) return;

        float randomX = allowedX[Random.Range(0, allowedX.Length)];

        float minZ = chunk.transform.position.z;
        float maxZ = chunk.transform.position.z + chunk.transform.localScale.z;
        float randomZ = Random.Range(minZ, maxZ);

        Vector3 spawnPos = new Vector3(randomX, capsuleSpawnHeight, randomZ);

        Collider[] hitColliders = Physics.OverlapSphere(spawnPos, 0.5f);
        foreach (var hit in hitColliders)
        {
            foreach (string tag in obstacleTags)
            {
                if (hit.CompareTag(tag))
                {
                    return;
                }
            }
        }

        Instantiate(capsulePrefab, spawnPos, Quaternion.identity, chunk.transform);
    }

    // ✅ Accès au multiplier (pour UI)
    public float GetSpeedMultiplier()
    {
        return speedMultiplier;
    }

    // ✅ Reset de la vitesse (appelé par GameManager)
    public void ResetSpeed()
    {
        speedMultiplier = 1f;
        timer = 0f;

        Debug.Log("🔄 Speed reset");
    }
}