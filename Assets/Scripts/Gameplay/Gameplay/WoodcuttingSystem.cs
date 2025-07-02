using UnityEngine;
using System.Collections;

public class WoodcuttingSystem : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData woodItem;

    public static WoodcuttingSystem Instance { get; private set; }
    [Tooltip("Only true once you’ve talked to Griff")]
    public bool canCut = false;

    public LayerMask treeLayer;    // Trees layer
    public float chopRange = 5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!canCut) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("WoodcuttingSystem: canCut is TRUE and mouse clicked");

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * chopRange, Color.red, 2f); // Optional visual ray

            // TEMP DEBUG: check if ray hits anything, regardless of layer
            if (Physics.Raycast(ray, out var testHit, chopRange, ~0))
            {
                Debug.Log($"[DEBUG] Raycast HIT something: {testHit.collider.name} on layer {LayerMask.LayerToName(testHit.collider.gameObject.layer)}");
            }
            else
            {
                Debug.Log("[DEBUG] Raycast did NOT hit anything at all.");
            }

            // ACTUAL TREE LAYER CHECK
            if (Physics.Raycast(ray, out var hit, chopRange, treeLayer))
            {
                Debug.Log($"Raycast hit: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                ChoppableTree t = hit.collider.GetComponentInParent<ChoppableTree>();
                if (t != null)
                {
                    Debug.Log("Tree component found, calling Chop()");
                    t.Chop();
                    InventoryManager.I.AddItem(woodItem, t.woodAmount);
                    QuestManager.Instance.AdvanceStep("Griff");
                }
                else
                {
                    Debug.LogWarning("Hit object has no ChoppableTree component.");
                }
            }
            else
            {
                Debug.Log("Raycast did NOT hit any object in the treeLayer.");
            }
        }
    }
}




/*
using UnityEngine;
using System.Collections;

public class WoodcuttingSystem : MonoBehaviour
{
    [Header("Item Settings")]
    public ItemData woodItem;

    public static WoodcuttingSystem Instance { get; private set; }
    [Tooltip("Only true once you’ve talked to Griff")]
    public bool canCut = false;

    public LayerMask treeLayer;    // Trees layer
    public float chopRange = 5f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!canCut) return;

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("WoodcuttingSystem: canCut is TRUE and mouse clicked");
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, chopRange, treeLayer))
            {
                Debug.Log($"Raycast hit: {hit.collider.name} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");

                ChoppableTree t = hit.collider.GetComponentInParent<ChoppableTree>();
                if (t != null)
                {
                    Debug.Log("Tree component found, calling Chop()");
                    t.Chop();
                    InventoryManager.I.AddItem(woodItem, t.woodAmount);
                    QuestManager.Instance.AdvanceStep("Griff");
                }
                else
                {
                    Debug.LogWarning("Hit object has no ChoppableTree component.");
                }
            }
            else
            {
                Debug.Log("Raycast did NOT hit any object in the treeLayer.");
            }

        }
    }
}
*/