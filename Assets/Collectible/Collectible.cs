using UnityEngine;

public class Collectible : MonoBehaviour
{
    public CollectibleType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("✅ Collectible ramassé : " + type);

            if (PlayerInventoryCollectible.Instance == null)
            {
                Debug.LogError("❌ Inventory non trouvé !");
                return;
            }

            int multiplier = 1;

            if (PotionEffectManager.Instance != null)
            {
                multiplier = PotionEffectManager.Instance.GetLootMultiplier();
            }

            for (int i = 0; i < multiplier; i++)
            {
                PlayerInventoryCollectible.Instance.AddCollectible(this);
            }

            Destroy(gameObject);
        }
    }
}
