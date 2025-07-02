using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    [HideInInspector] public InventoryItem item;
    public Image iconImage;
    public TMP_Text qtyText;

    public void Setup(InventoryItem newItem)
    {
        Debug.Log($"Setup ran for {newItem.data.itemName}");

        item = newItem;
        iconImage.sprite = item.data.icon;
        iconImage.gameObject.SetActive(true);

        if (item.data.stackable && item.quantity > 1)
        {
            qtyText.text = item.quantity.ToString();
            qtyText.gameObject.SetActive(true);
        }
        else
        {
            qtyText.gameObject.SetActive(false);
        }
    }

    public void Clear()
    {
        item = null;
        iconImage.sprite = null;
        iconImage.gameObject.SetActive(false);
        qtyText.gameObject.SetActive(false);
    }
}
