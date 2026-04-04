using UnityEngine;

public class PotionButton : MonoBehaviour
{
    public PotionType type;

    public void OnClick()
    {
        if (PlayerInventoryCollectible.Instance == null) return;

        bool success = PlayerInventoryCollectible.Instance.ConsumePotion(type);

        if (success && PotionEffectManager.Instance != null)
        {
            PotionEffectManager.Instance.SelectPotion(type);
            Debug.Log("🔥 Potion sélectionnée : " + type);
        }
    }
}
