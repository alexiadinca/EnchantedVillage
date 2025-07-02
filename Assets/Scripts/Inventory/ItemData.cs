using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject furniturePrefab;
    public bool stackable;
    public int maxStack = 64;
    public bool isSpecial; // Mark the Axe as special
}