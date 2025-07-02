using UnityEngine;

public class LogPickup : MonoBehaviour
{
    public ItemData itemData;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            InventoryManager.I.AddItem(itemData, 1);
            Destroy(gameObject);
        }
    }
}
