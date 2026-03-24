using UnityEngine;
using UnityEngine.UI;

public class CapsuleUI : MonoBehaviour
{
    public PlayerInventory playerInventory;
    public Image fillBar;

    void Start()
    {
        if (fillBar != null)
            fillBar.fillAmount = 0f;
    }

    void Update()
    {
        if (playerInventory != null && fillBar != null)
        {
            float ratio = playerInventory.currentFuel / playerInventory.maxFuel;
            fillBar.fillAmount = ratio;
        }
    }
}