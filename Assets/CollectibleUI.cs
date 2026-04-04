using UnityEngine;
using TMPro;
using System.Collections;

public class CollectibleUI : MonoBehaviour
{
    public TMP_Text crapaudText;
    public TMP_Text scarabeeText;
    public TMP_Text oeufText;
    public TMP_Text chauveSourisText;
    public TMP_Text corbeauText;
    public TMP_Text roseText;

    private void Start()
    {
        // On attend que l'instance soit disponible
        StartCoroutine(WaitForInventory());
    }

    private IEnumerator WaitForInventory()
    {
        while (PlayerInventoryCollectible.Instance == null)
        {
            yield return null;
        }

        UpdateUI();
    }

    void Update()
    {
        if (PlayerInventoryCollectible.Instance == null) return;

        UpdateUI();
    }

    void UpdateUI()
    {
        var inventory = PlayerInventoryCollectible.Instance;

        crapaudText.text = inventory.GetCount(CollectibleType.BaseCrapaud).ToString();
        scarabeeText.text = inventory.GetCount(CollectibleType.CarapaceScarabe).ToString();
        oeufText.text = inventory.GetCount(CollectibleType.CoquilleOeuf).ToString();
        chauveSourisText.text = inventory.GetCount(CollectibleType.AileChauveSouris).ToString();
        corbeauText.text = inventory.GetCount(CollectibleType.PlumeCorbeau).ToString();
        roseText.text = inventory.GetCount(CollectibleType.PetaleRoseNoir).ToString();
    }
}
