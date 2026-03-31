using UnityEngine;

public class Collectible : MonoBehaviour
{
    public CollectibleType type;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventoryCollectible inventory = other.GetComponent<PlayerInventoryCollectible>();

            if (inventory != null)
            {
                inventory.AddCollectible(this);
                gameObject.SetActive(false);
            }
        }
    }
}
