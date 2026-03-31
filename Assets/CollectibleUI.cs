using UnityEngine;
using TMPro;

public class CollectibleUI : MonoBehaviour
{
    public PlayerInventoryCollectible inventory;

    public TMP_Text crapaudText;
    public TMP_Text scarabeeText;
    public TMP_Text oeufText;
    public TMP_Text chauveSourisText;
    public TMP_Text corbeauText;
    public TMP_Text roseText;

    void Update()
    {
        if (inventory == null) return;

        crapaudText.text = inventory.GetCount(CollectibleType.BaseCrapaud).ToString();
        scarabeeText.text = inventory.GetCount(CollectibleType.CarapaceScarabe).ToString();
        oeufText.text = inventory.GetCount(CollectibleType.CoquilleOeuf).ToString();
        chauveSourisText.text = inventory.GetCount(CollectibleType.AileChauveSouris).ToString();
        corbeauText.text = inventory.GetCount(CollectibleType.PlumeCorbeau).ToString();
        roseText.text = inventory.GetCount(CollectibleType.PetaleRoseNoir).ToString();
    }
}
