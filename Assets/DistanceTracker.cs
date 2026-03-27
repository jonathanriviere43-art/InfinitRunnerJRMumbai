using UnityEngine;
using TMPro; // Utilise TextMeshPro

public class DistanceTracker : MonoBehaviour
{
    [Header("Références")]
    public ChunkManager chunkManager; // Glisse ton ChunkManager ici dans l'inspecteur
    public TextMeshProUGUI distanceText; // Glisse ton TextMeshProUGUI ici

    [Header("Options")]
    public float distanceMultiplier = 1f; // Permet d'ajuster la distance si nécessaire

    private float totalDistance = 0f;

    void Update()
    {
        if (chunkManager == null || distanceText == null)
            return;

        // Calcul de la distance parcourue
        totalDistance += chunkManager.moveSpeed * Time.deltaTime * distanceMultiplier;

        // Affichage en entier, style arcade, avec "Distance : " devant
        distanceText.text = "Distance : " + Mathf.FloorToInt(totalDistance) + " m";
    }
}
