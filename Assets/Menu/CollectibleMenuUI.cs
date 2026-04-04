using UnityEngine;
using TMPro;

public class CollectibleMenuUI : MonoBehaviour
{
    [System.Serializable]
    public class CollectibleDisplay
    {
        public CollectibleType type;
        public TMP_Text text;
    }

    public CollectibleDisplay[] displays;

    private void Update()
    {
        if (PlayerInventoryCollectible.Instance == null) return;

        foreach (var display in displays)
        {
            int count = PlayerInventoryCollectible.Instance.GetCount(display.type);
            display.text.text = display.type.ToString() + " : " + count;
        }
    }
}
