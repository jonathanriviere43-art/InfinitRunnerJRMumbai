using UnityEngine;
using UnityEngine.UI;

public class CrowdDistanceUI : MonoBehaviour
{
    [Header("References")]
    public Transform player;       // joueur à Z = 0
    public Transform crowd;        // foule à Z = -3 au départ

    [Header("UI Settings")]
    public Image distanceBar;      // Image type Filled
    public Color farColor = Color.green;
    public Color midColor = Color.yellow;
    public Color nearColor = Color.red;

    private float startZ;          // Z initial de la foule
    private float endZ = 3f;       // Z cible pour que la barre soit pleine

    private void Start()
    {
        if (crowd != null)
            startZ = crowd.position.z;  // mémoriser la position initiale
    }

    private void Update()
    {
        if (player == null || crowd == null || distanceBar == null) return;

        // Distance parcourue par la foule depuis son départ
        float distanceTraveled = crowd.position.z - startZ;

        // Distance totale à parcourir pour que la barre soit pleine
        float totalDistance = endZ - startZ;

        // Remplissage proportionnel
        float fill = Mathf.Clamp01(distanceTraveled / totalDistance);
        distanceBar.fillAmount = fill;

        // Interpoler la couleur
        if (fill < 0.5f)
        {
            // Vert → Jaune
            distanceBar.color = Color.Lerp(farColor, midColor, fill * 2f);
        }
        else
        {
            // Jaune → Rouge
            distanceBar.color = Color.Lerp(midColor, nearColor, (fill - 0.5f) * 2f);
        }
    }
}