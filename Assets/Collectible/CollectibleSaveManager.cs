using System.Collections.Generic;
using UnityEngine;

public class CollectibleSaveManager : MonoBehaviour
{
    public static CollectibleSaveManager Instance;

    private HashSet<string> collectedItems = new HashSet<string>();

    private const string SAVE_KEY = "COLLECTIBLES_SAVE";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCollectible(string id)
    {
        if (collectedItems.Add(id))
        {
            Save();
        }
    }

    public bool HasCollectible(string id)
    {
        return collectedItems.Contains(id);
    }

    public List<string> GetAllCollectibles()
    {
        return new List<string>(collectedItems);
    }

    private void Save()
    {
        string data = string.Join(",", collectedItems);
        PlayerPrefs.SetString(SAVE_KEY, data);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        collectedItems.Clear();

        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string data = PlayerPrefs.GetString(SAVE_KEY);

            if (!string.IsNullOrEmpty(data))
            {
                string[] items = data.Split(',');

                foreach (string id in items)
                {
                    collectedItems.Add(id);
                }
            }
        }
    }

    public void ClearAll()
    {
        collectedItems.Clear();
        PlayerPrefs.DeleteKey(SAVE_KEY);
    }
}
