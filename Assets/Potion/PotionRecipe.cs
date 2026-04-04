using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PotionRecipe
{
    public PotionType potionType;
    public List<CostItem> cost;
}

[System.Serializable]
public class CostItem
{
    public CollectibleType type;
    public int amount;
}
