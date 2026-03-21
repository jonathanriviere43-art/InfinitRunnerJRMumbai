using UnityEngine;

public class CapsuleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject capsulePrefab;
    public float spawnHeight = 1f;           // Y fixe pour la capsule
    public float[] allowedX = { -3f, 0f, 3f }; // positions X autorisées

    [Header("Chunk Settings")]
    public Transform chunkTransform;          // chunk dans lequel spawn

    [Header("Obstacle Tags")]
    public string[] obstacleTags = { "Obstacle", "ObstacleWood" };

    private void Start()
    {
        if (chunkTransform != null)
        {
            TrySpawnCapsule();
        }
    }

    private void TrySpawnCapsule()
    {
        // Choisir un X aléatoire autorisé
        float randomX = allowedX[Random.Range(0, allowedX.Length)];

        // Choisir Z aléatoire dans le chunk (en fonction de la taille du chunk)
        float minZ = chunkTransform.position.z;
        float maxZ = chunkTransform.position.z + chunkTransform.localScale.z;
        float randomZ = Random.Range(minZ, maxZ);

        Vector3 spawnPos = new Vector3(randomX, spawnHeight, randomZ);

        // Vérifier si un obstacle est présent à cet endroit
        Collider[] hitColliders = Physics.OverlapSphere(spawnPos, 0.5f);
        foreach (var hit in hitColliders)
        {
            foreach (string tag in obstacleTags)
            {
                if (hit.CompareTag(tag))
                {
                    Debug.Log("Spawn annulé : obstacle détecté");
                    return; // ne spawn pas ici
                }
            }
        }

        // Spawn la capsule
        Instantiate(capsulePrefab, spawnPos, Quaternion.identity);
        Debug.Log("Capsule spawnée à " + spawnPos);
    }
}
