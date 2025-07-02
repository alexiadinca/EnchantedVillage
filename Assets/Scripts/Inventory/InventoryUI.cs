using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Containers")]
    public Transform specialGrid;    
    public Transform generalGrid; 

    private List<InventorySlotUI> specialSlots = new List<InventorySlotUI>();
    private List<InventorySlotUI> generalSlots = new List<InventorySlotUI>();

    void Start()
    {
        // Pull in the slots that InventoryManager has already instantiated
        specialSlots.Clear();
        foreach (Transform child in specialGrid)
        {
            var slotUI = child.GetComponent<InventorySlotUI>();
            if (slotUI != null) specialSlots.Add(slotUI);
        }

        generalSlots.Clear();
        foreach (Transform child in generalGrid)
        {
            var slotUI = child.GetComponent<InventorySlotUI>();
            if (slotUI != null) generalSlots.Add(slotUI);
        }
    }

    void OnEnable()
    {
        InventoryManager.I.OnInventoryChanged += RefreshUI;
        RefreshUI();
    }

    void OnDisable()
    {
        InventoryManager.I.OnInventoryChanged -= RefreshUI;
    }

    public void RefreshUI()
    {
        // Clear all slots first
        foreach (var s in specialSlots) s.Clear();
        foreach (var g in generalSlots) g.Clear();

        // Fill special items
        var sItems = InventoryManager.I.specialItems;
        for (int i = 0; i < sItems.Count && i < specialSlots.Count; i++)
            specialSlots[i].Setup(sItems[i]);

        // Fill general items
        var gItems = InventoryManager.I.generalItems;
        for (int i = 0; i < gItems.Count && i < generalSlots.Count; i++)
            generalSlots[i].Setup(gItems[i]);
    }
}
