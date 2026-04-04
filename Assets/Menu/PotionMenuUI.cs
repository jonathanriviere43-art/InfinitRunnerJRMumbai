using TMPro;
using UnityEngine;

public class PotionMenuUI : MonoBehaviour
{
    public TMP_Text agiliteText;
    public TMP_Text vitesseText;
    public TMP_Text resistanceText;
    public TMP_Text lootText;
    public TMP_Text secondelifeText;

    private void Update()
    {
        if (PlayerInventoryCollectible.Instance == null) return;

        var inv = PlayerInventoryCollectible.Instance;

        agiliteText.text = inv.GetPotionCount(PotionType.Agilite).ToString();
        vitesseText.text = inv.GetPotionCount(PotionType.Vitesse).ToString();
        resistanceText.text = inv.GetPotionCount(PotionType.Resistance).ToString();
        lootText.text = inv.GetPotionCount(PotionType.Loot).ToString();
        secondelifeText.text = inv.GetPotionCount(PotionType.Secondelife).ToString();
    }
}
