using UnityEngine;

public class MenuInitializer : MonoBehaviour
{
    private void Start()
    {
        if (PotionEffectManager.Instance != null)
        {
            PotionEffectManager.Instance.ResetEffects();
            Debug.Log("✅ PotionEffectManager reset");
        }

        if (PlayerInventoryCollectible.Instance != null)
        {
            PlayerInventoryCollectible.Instance.ResetPotionRun();
            Debug.Log("✅ Inventory potion reset");
        }
    }
}
