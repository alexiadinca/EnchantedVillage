using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager I;

    [Header("Inventory Settings")]
    public Transform specialItemsHolder; // SpecialGridHolder
    public Transform generalItemsHolder; // GeneralGridHolder
    public GameObject slotPrefab;
    public int specialSlotsCount = 5;
    public int generalSlotsCount = 30;

    float cellW = 64f, cellH = 64f;

    public List<InventoryItem> specialItems = new();
    public List<InventoryItem> generalItems = new();

    public event Action OnInventoryChanged;

    void Awake()
    {
        if (I == null)
            I = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        InitializeSlots();
    }

    void InitializeSlots()
    {
        // 1) clear any old slots
        foreach (Transform child in generalItemsHolder)
            Destroy(child.gameObject);
        foreach (Transform child in specialItemsHolder)
            Destroy(child.gameObject);

        // 2) grid settings
        int genColumns = 10;
        int specColumns = specialSlotsCount;
        float spacingX = 8f;
        float spacingY = 8f;

        // 3) instantiate & position general slots
        for (int i = 0; i < generalSlotsCount; i++)
        {
            GameObject go = Instantiate(slotPrefab, generalItemsHolder);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;

            int col = i % genColumns;
            int row = i / genColumns;

            rt.anchoredPosition = new Vector2(
                col * (cellW + spacingX),
               -row * (cellH + spacingY)
            );
        }

        // 4) instantiate & position special slots
        for (int i = 0; i < specialSlotsCount; i++)
        {
            GameObject go = Instantiate(slotPrefab, specialItemsHolder);
            RectTransform rt = go.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;

            int col = i % specColumns;
            int row = i / specColumns;

            rt.anchoredPosition = new Vector2(
                col * (cellW + spacingX),
               -row * (cellH + spacingY)
            );
        }

        // 5) finally, hook up the visuals
        UpdateInventoryUI();
    }

    public void AddItem(ItemData data, int quantity)
    {
        Debug.Log($"Adding item: {data.itemName}, quantity: {quantity}, isSpecial: {data.isSpecial}");

        var list = data.isSpecial ? specialItems : generalItems;
        var existing = list.Find(x => x.data.itemName == data.itemName);

        if (existing != null)
        {
            existing.quantity += quantity;
            Debug.Log("Stacked on existing item.");
        }
        else
        {
            list.Add(new InventoryItem(data, quantity));
            Debug.Log(data.isSpecial ? "Added to SPECIAL inventory list" : "Added to GENERAL inventory list");
        }

        OnInventoryChanged?.Invoke();
        UpdateInventoryUI();
    }


    void UpdateInventoryUI()
    {
        for (int i = 0; i < generalItemsHolder.childCount; i++)
        {
            InventorySlotUI slotUI = generalItemsHolder.GetChild(i).GetComponent<InventorySlotUI>();
            if (i < generalItems.Count)
                slotUI.Setup(generalItems[i]);
            else
                slotUI.Clear();
        }

        for (int i = 0; i < specialItemsHolder.childCount; i++)
        {
            InventorySlotUI slotUI = specialItemsHolder.GetChild(i).GetComponent<InventorySlotUI>();
            if (i < specialItems.Count)
                slotUI.Setup(specialItems[i]);
            else
                slotUI.Clear();
        }
    }

    public int GetItemCount(ItemData data)
    {
        if (data == null)
        {
            Debug.LogWarning("InventoryManager.GetItemCount() called with null ItemData!");
            return 0;
        }

        var list = data.isSpecial ? specialItems : generalItems;
        var entry = list.Find(x => x.data.itemName == data.itemName);
        return entry != null ? entry.quantity : 0;
    }


    public bool HasItem(ItemData data)
    {
        return GetItemCount(data) > 0;
    }

    public void RemoveItem(ItemData data, int quantity)
    {
        var list = data.isSpecial ? specialItems : generalItems;
        var entry = list.Find(x => x.data.itemName == data.itemName);
        if (entry != null)
        {
            entry.quantity -= quantity;
            if (entry.quantity <= 0)
                list.Remove(entry);

            OnInventoryChanged?.Invoke();
            UpdateInventoryUI();
        }
    }
}
