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

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (var display in displays)
        {
            int count = PlayerPrefs.GetInt(display.type.ToString(), 0);
            display.text.text = display.type.ToString() + " : " + count;
        }
    }
}
