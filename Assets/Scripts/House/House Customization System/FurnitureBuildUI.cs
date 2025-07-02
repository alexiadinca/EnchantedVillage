using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class FurnitureBuildUI : MonoBehaviour
{
    [Header("UI Prefab")]
    public GameObject buttonPrefab;

    [Header("UI Layout Container")]
    public Transform listContainer;

    void Start()
    {
        GenerateFurnitureButtons();
    }

    void GenerateFurnitureButtons()
    {
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        var ownedFurniture = InventoryManager.I.generalItems
            .Where(i => i.data != null && i.data.furniturePrefab != null)
            .GroupBy(i => i.data.itemName)
            .Select(g => g.First()) // show only one button per unique item
            .ToList();

        foreach (var item in ownedFurniture)
        {
            GameObject btn = Instantiate(buttonPrefab, listContainer);
            var label = btn.GetComponentInChildren<Text>();
            if (label != null)
                label.text = item.data.itemName;

            var button = btn.GetComponent<Button>();
            if (button != null)
            {
                GameObject prefab = item.data.furniturePrefab;
                button.onClick.AddListener(() =>
                {
                    Debug.Log($"Placing furniture: {item.data.itemName}");
                    FurniturePlacer.Instance.StartPlacing(prefab);
                });
            }
        }
    }
}
