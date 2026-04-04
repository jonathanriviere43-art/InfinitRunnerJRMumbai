using UnityEngine;

public class PotionEffectManager : MonoBehaviour
{
    public static PotionEffectManager Instance;

    private PotionType selectedPotion = PotionType.None;

    [Header("Multiplicateurs Agilité")]
    [SerializeField] private float agilityMultiplier = 1.5f;

    [Header("Loot")]
    [SerializeField] private int lootMultiplier = 2;
    [SerializeField] private int maxLootUses = 3;

    private int lootUsesRemaining = 0;

    [Header("Crowd Speed (Potion Vitesse)")]
    [SerializeField] private float crowdSpeedMultiplierWhenPotion = 0.01f;

    [Header("Second Life")]
    private bool hasSecondLife = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SelectPotion(PotionType type)
    {
        selectedPotion = type;

        // Loot reset
        if (type == PotionType.Loot)
        {
            lootUsesRemaining = maxLootUses;
            Debug.Log("💰 Potion Loot activée (" + lootUsesRemaining + " utilisations)");
        }

        // Second Life activation
        if (type == PotionType.Secondelife)
        {
            hasSecondLife = true;
            Debug.Log("❤️ Potion Second Life activée");
        }
    }

    public PotionType GetSelectedPotion()
    {
        return selectedPotion;
    }

    // ================= AGILITÉ =================

    public float GetSpeedMultiplier()
    {
        if (selectedPotion == PotionType.Agilite)
            return agilityMultiplier;

        return 1f;
    }

    // ================= LOOT =================

    public int GetLootMultiplier()
    {
        if (selectedPotion == PotionType.Loot && lootUsesRemaining > 0)
        {
            lootUsesRemaining--;
            Debug.Log("💰 Loot x2 utilisé, reste : " + lootUsesRemaining);
            return lootMultiplier;
        }

        return 1;
    }

    // ================= CROWD SPEED (VITESSE) =================

    public float GetCrowdSpeedMultiplier()
    {
        if (selectedPotion == PotionType.Vitesse)
            return crowdSpeedMultiplierWhenPotion;

        return 1f;
    }

    // ================= SECOND LIFE =================

    public bool UseSecondLife()
    {
        if (hasSecondLife)
        {
            hasSecondLife = false;
            Debug.Log("❤️ Second Life utilisée !");
            return true;
        }

        return false;
    }

    public void ResetEffects()
    {
        selectedPotion = PotionType.None;
        lootUsesRemaining = 0;
        hasSecondLife = false;

        Debug.Log("🔄 PotionEffectManager reset");
    }
}
