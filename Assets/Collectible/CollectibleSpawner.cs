using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectibleEntry
{
    public GameObject prefab;
    public float weight;
}

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Ground Collectibles (Y = 0)")]
    public List<CollectibleEntry> groundCollectibles;

    [Header("Air Collectibles (Y = 3)")]
    public List<CollectibleEntry> airCollectibles;

    [Header("Spawn Lanes")]
    public float[] lanes = new float[] { -3f, 0f, 3f };

    [Header("Y Positions")]
    public float groundY = 0f;
    public float airY = 3f;

    [Header("Z Spawn Range (relative to chunk)")]
    public float minZOffset = -5f;
    public float maxZOffset = 5f;

    [Header("Spawn Amount Control")]
    public int minCollectiblesPerChunk = 0;
    public int maxCollectiblesPerChunk = 2;

    [Header("Spacing")]
    public float minDistanceBetweenCollectibles = 1.5f;
    public int maxAttempts = 10;

    public void SpawnCollectibleInChunk(Transform chunk)
    {
        if (chunk == null)
        {
            Debug.LogWarning("Chunk null !");
            return;
        }

        List<Vector3> spawnedPositions = new List<Vector3>();

        int amount = Random.Range(minCollectiblesPerChunk, maxCollectiblesPerChunk + 1);

        Debug.Log($"🎯 Spawn {amount} collectibles in chunk {chunk.name}");

        for (int i = 0; i < amount; i++)
        {
            bool isAir = Random.value > 0.5f;

            List<CollectibleEntry> pool = isAir ? airCollectibles : groundCollectibles;

            GameObject prefab = GetWeightedRandom(pool);
            if (prefab == null) continue;

            Vector3 spawnPos = Vector3.zero;
            bool valid = false;

            float y = isAir ? airY : groundY;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                float x = lanes[Random.Range(0, lanes.Length)];
                float z = Random.Range(chunk.position.z + minZOffset, chunk.position.z + maxZOffset);

                spawnPos = new Vector3(x, y, z);

                valid = true;

                foreach (Vector3 pos in spawnedPositions)
                {
                    if (Vector3.Distance(spawnPos, pos) < minDistanceBetweenCollectibles)
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                    break;
            }

            if (!valid)
            {
                Debug.LogWarning("⚠️ Impossible de placer un collectible");
                continue;
            }

            Instantiate(prefab, spawnPos, Quaternion.identity, chunk);
            spawnedPositions.Add(spawnPos);

            Debug.Log($"✅ Spawn {prefab.name} at {spawnPos} (Air: {isAir})");
        }
    }

    GameObject GetWeightedRandom(List<CollectibleEntry> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("⚠️ Liste de collectibles vide !");
            return null;
        }

        float totalWeight = 0f;

        foreach (var item in list)
            totalWeight += item.weight;

        float randomValue = Random.Range(0, totalWeight);

        float cumulative = 0f;

        foreach (var item in list)
        {
            cumulative += item.weight;

            if (randomValue <= cumulative)
                return item.prefab;
        }

        return list[list.Count - 1].prefab;
    }
}
