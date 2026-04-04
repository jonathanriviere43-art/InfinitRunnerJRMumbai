using UnityEngine;

public class PotionCraftButton : MonoBehaviour
{
    public PotionType potionType;

    public void OnClick()
    {
        bool success = CraftManager.Instance.CraftPotion(potionType);

        if (success)
        {
            Debug.Log("Potion craftée !");
        }
        else
        {
            Debug.Log("Pas assez de ressources !");
        }
    }
}
