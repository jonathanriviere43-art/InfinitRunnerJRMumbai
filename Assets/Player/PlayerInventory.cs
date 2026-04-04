using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    public List<GameObject> collectedCapsules = new List<GameObject>();
    public int maxCapsules = 6;

    [Header("Fuel System")]
    public float currentFuel;
    public float maxFuel;
    internal int currentPotions;

    void Start()
    {
        maxFuel = maxCapsules;
        currentFuel = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Carburant"))
        {
            if (collectedCapsules.Count >= maxCapsules)
            {
                Debug.Log("Inventaire plein !");
                return;
            }

            collectedCapsules.Add(other.gameObject);
            other.gameObject.SetActive(false);

            // Ajout de fuel
            currentFuel += 1f;
            currentFuel = Mathf.Clamp(currentFuel, 0, maxFuel);

            Debug.Log("Capsule collectée ! Total : " + collectedCapsules.Count);
        }
    }

    // Méthode pour consommer du fuel
    public bool ConsumeFuel(float amount)
    {
        if (currentFuel <= 0f) return false;

        currentFuel -= amount;
        currentFuel = Mathf.Clamp(currentFuel, 0f, maxFuel);

        // Supprimer des capsules si nécessaire
        while (collectedCapsules.Count > Mathf.RoundToInt(currentFuel))
        {
            collectedCapsules.RemoveAt(collectedCapsules.Count - 1);
        }

        return true;
    }

    // ✅ RESET DU FUEL (corrigé)
    public void ResetFuel()
    {
        currentFuel = 0f;
        collectedCapsules.Clear();

        Debug.Log("⛽ Fuel reset");
    }

    internal void AddCollectible(Collectible collectible)
    {
        throw new NotImplementedException();
    }
}
