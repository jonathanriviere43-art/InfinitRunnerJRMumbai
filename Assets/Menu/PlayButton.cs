using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    public void StartGame()
    {
        // 🔒 Sécurité : récupère la potion active si déjà consommée
        if (PlayerInventoryCollectible.Instance != null &&
            PotionEffectManager.Instance != null)
        {
            var potion = PlayerInventoryCollectible.Instance.GetActivePotion();

            if (potion.HasValue)
            {
                PotionEffectManager.Instance.SelectPotion(potion.Value);
            }
        }

        SceneManager.LoadScene("SampleScene");
    }
}
