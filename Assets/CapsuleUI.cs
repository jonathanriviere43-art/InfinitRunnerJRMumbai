using UnityEngine;
using UnityEngine.UI;

public class CapsuleUI : MonoBehaviour
{
    public PlayerInventory playerInventory; // référence au script du joueur
    public Image fillBar;                   // Image de remplissage

    private int maxCapsules;

    void Start()
    {
        if (playerInventory != null)
            maxCapsules = playerInventory.maxCapsules;

        if (fillBar != null)
            fillBar.fillAmount = 0f; // vide au début
    }

    void Update()
    {
        if (playerInventory != null && fillBar != null)
        {
            // calculer le ratio
            float ratio = (float)playerInventory.collectedCapsules.Count / maxCapsules;
            fillBar.fillAmount = ratio;
        }
    }
}
