using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryCollectible : MonoBehaviour
{
    private Dictionary<CollectibleType, int> collectibles = new Dictionary<CollectibleType, int>();

    private void Awake()
    {
        foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType)))
        {
            collectibles[type] = 0;
        }

        LoadCollectibles();
    }

    public void AddCollectible(Collectible collectible)
    {
        if (collectible == null) return;

        collectibles[collectible.type]++;

        Debug.Log($"🧪 {collectible.type} = {collectibles[collectible.type]}");

        SaveCollectibles();
    }

    public int GetCount(CollectibleType type)
    {
        return collectibles.ContainsKey(type) ? collectibles[type] : 0;
    }

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
            int value = PlayerPrefs.GetInt(type.ToString(), 0);
            collectibles[type] = value;
        }
    }

    public void ResetCollectibles()
    {
        foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType)))
        {
            collectibles[type] = 0;
            PlayerPrefs.DeleteKey(type.ToString());
        }

        PlayerPrefs.Save();
    }
}
