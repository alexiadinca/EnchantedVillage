[System.Serializable]
public class InventoryItem
{
    public ItemData data;
    public int quantity;

    public InventoryItem(ItemData data, int qty = 1)
    {
        this.data = data;
        this.quantity = qty;
    }
}
