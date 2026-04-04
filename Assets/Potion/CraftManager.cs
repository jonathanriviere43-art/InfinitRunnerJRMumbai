using UnityEngine;
using System.Collections.Generic;

public class CraftManager : MonoBehaviour
{
    public static CraftManager Instance;

    public PotionRecipe[] recipes;

    private void Awake()
    {
        Instance = this;
    }

    public bool CraftPotion(PotionType type)
    {
        PotionRecipe recipe = GetRecipe(type);
        if (recipe == null) return false;

        Dictionary<CollectibleType, int> costDict = new Dictionary<CollectibleType, int>();

        foreach (var item in recipe.cost)
        {
            costDict[item.type] = item.amount;
        }

        if (!PlayerInventoryCollectible.Instance.HasCollectibles(costDict))
            return false;

        PlayerInventoryCollectible.Instance.ConsumeCollectibles(costDict);
        PlayerInventoryCollectible.Instance.AddPotion(type);

        return true;
    }

    private PotionRecipe GetRecipe(PotionType type)
    {
        foreach (var r in recipes)
        {
            if (r.potionType == type)
                return r;
        }
        return null;
    }
}
