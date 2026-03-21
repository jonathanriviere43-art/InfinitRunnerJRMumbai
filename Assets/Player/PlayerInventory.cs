using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public List<GameObject> collectedCapsules = new List<GameObject>();
    public int maxCapsules = 6;  // Maximum de capsules que le joueur peut collecter

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Carburant"))
        {
            // Vérifier si on a déjà le maximum
            if (collectedCapsules.Count >= maxCapsules)
            {
                Debug.Log("Inventaire plein !");
                return; // Ne pas collecter
            }

            // Ajouter la capsule à l'inventaire
            collectedCapsules.Add(other.gameObject);

            // Désactiver la capsule pour l'effet visuel
            other.gameObject.SetActive(false);

            // Debug
            Debug.Log("Capsule collectée ! Total : " + collectedCapsules.Count);
        }
    }
}
