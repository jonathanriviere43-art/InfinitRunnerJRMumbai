using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryCollectible : MonoBehaviour
{
    public static PlayerInventoryCollectible Instance;

    private Dictionary<CollectibleType, int> collectibles = new Dictionary<CollectibleType, int>();
    private Dictionary<PotionType, int> potions = new Dictionary<PotionType, int>();

    private bool potionUsed = false;
    private PotionType? activePotion = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType)))
        {
            collectibles[type] = 0;
        }

        foreach (PotionType type in Enum.GetValues(typeof(PotionType)))
        {
            potions[type] = 0;
        }

        LoadCollectibles();
        LoadPotions();
    }

    // ================= COLLECTIBLES =================

    public void AddCollectible(Collectible collectible)
    {
        if (collectible == null) return;

        collectibles[collectible.type]++;
        SaveCollectibles();
    }

    public int GetCount(CollectibleType type)
    {
        return collectibles.ContainsKey(type) ? collectibles[type] : 0;
    }

    public bool HasCollectibles(Dictionary<CollectibleType, int> required)
    {
        foreach (var item in required)
        {
            if (GetCount(item.Key) < item.Value)
                return false;
        }
        return true;
    }

    public void ConsumeCollectibles(Dictionary<CollectibleType, int> required)
    {
        foreach (var item in required)
        {
            collectibles[item.Key] -= item.Value;
        }

        SaveCollectibles();
    }

    // ================= POTIONS =================

    public void AddPotion(PotionType type)
    {
        if (!potions.ContainsKey(type))
            potions[type] = 0;

        potions[type]++;
        SavePotions();

        Debug.Log("🧪 Potion créée : " + type + " = " + potions[type]);
    }

    public int GetPotionCount(PotionType type)
    {
        return potions.ContainsKey(type) ? potions[type] : 0;
    }

    public bool ConsumePotion(PotionType type)
    {
        if (potionUsed)
        {
            Debug.Log("❌ Potion déjà utilisée");
            return false;
        }

        if (GetPotionCount(type) <= 0)
        {
            Debug.Log("❌ Pas de potion");
            return false;
        }

        potions[type]--;
        potionUsed = true;
        activePotion = type;

        SavePotions();

        Debug.Log("🧪 Potion consommée : " + type);
        return true;
    }

    public PotionType? GetActivePotion()
    {
        return activePotion;
    }

    public void ResetPotionRun()
    {
        potionUsed = false;
        activePotion = null;
    }

    // ================= SAVE =================

    private void SaveCollectibles()
    {
        foreach (var pair in collectibles)
        {
            PlayerPrefs.SetInt(pair.Key.ToString(), pair.Value);
        }
        PlayerPrefs.Save();
    }

    private void LoadCollectibles()
    {
        foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType)))
        {
            collectibles[type] = PlayerPrefs.GetInt(type.ToString(), 0);
        }
    }

    private void SavePotions()
    {
        foreach (var pair in potions)
        {
            PlayerPrefs.SetInt("Potion_" + pair.Key.ToString(), pair.Value);
        }
        PlayerPrefs.Save();
    }

    private void LoadPotions()
    {
        foreach (PotionType type in Enum.GetValues(typeof(PotionType)))
        {
            potions[type] = PlayerPrefs.GetInt("Potion_" + type.ToString(), 0);
        }
    }
}
