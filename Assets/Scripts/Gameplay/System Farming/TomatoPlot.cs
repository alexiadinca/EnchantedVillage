using System.Collections;
using UnityEngine;

public class TomatoPlot : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("Radius around this plot where R will work")]
    public float interactionRadius = 2f;
    [Tooltip("Key to plant")]
    public KeyCode plantKey = KeyCode.R;

    [Header("Visuals (child objects)")]
    [Tooltip("Disabled seed model under this GameObject")]
    public GameObject seedVisual;
    [Tooltip("Disabled crop model under this GameObject")]
    public GameObject cropVisual;

    [Header("Gameplay")]
    public ItemData tomatoSeedItem;
    public ItemData tomatoCropItem;
    public string tomatoQuestId = "TomatoQuest";
    public NPCData mapleNPC;
    [Tooltip("Seconds until seed turns into crop")]
    public float growTime = 10f;

    bool hasPlanted = false;
    Transform player;

    void Awake()
    {
        Debug.Log("[TomatoPlot] Awake – hiding visuals");
        if (seedVisual) seedVisual.SetActive(false);
        if (cropVisual) cropVisual.SetActive(false);
    }

    void Update()
    {
        // 1) Find player once
        if (player == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null)
            {
                player = go.transform;
                Debug.Log("[TomatoPlot] Found Player: " + go.name);
            }
            else
            {
                Debug.LogWarning("[TomatoPlot] Cannot find GameObject tagged 'Player'");
                return;
            }
        }

        // 2) Log planted state
        Debug.Log($"[TomatoPlot] hasPlanted={hasPlanted}");

        // 3) Distance & input checks
        float d = Vector3.Distance(player.position, transform.position);
        bool inRange = d <= interactionRadius;
        bool pressed = Input.GetKeyDown(plantKey);

        Debug.Log($"[TomatoPlot] Dist={d:0.00} InRange={inRange} HasSeed={InventoryManager.I.HasItem(tomatoSeedItem)} Pressed{plantKey}={pressed}");

        // 4) If already planted, skip
        if (hasPlanted)
            return;

        // 5) If in range & pressed R, try to plant
        if (inRange && pressed)
            TryPlant();
    }

    void TryPlant()
    {
        Debug.Log("[TomatoPlot] TryPlant() called");

        if (!InventoryManager.I.HasItem(tomatoSeedItem))
        {
            Debug.LogWarning("[TomatoPlot] You have no tomato seeds!");
            return;
        }

        Debug.Log("[TomatoPlot] Consuming seed and showing seedVisual");
        InventoryManager.I.RemoveItem(tomatoSeedItem, 1);
        hasPlanted = true;

        if (seedVisual)
            seedVisual.SetActive(true);
        else
            Debug.LogError("[TomatoPlot] seedVisual is not assigned!");

        StartCoroutine(GrowRoutine());
    }

    IEnumerator GrowRoutine()
    {
        Debug.Log("[TomatoPlot] GrowRoutine started, waiting " + growTime + "s");
        yield return new WaitForSeconds(growTime);

        Debug.Log("[TomatoPlot] Grow time elapsed — swapping visuals");
        if (seedVisual) seedVisual.SetActive(false);
        if (cropVisual)
            cropVisual.SetActive(true);
        else
            Debug.LogError("[TomatoPlot] cropVisual is not assigned!");

        Debug.Log("[TomatoPlot] Granting crop, advancing quest & friendship");
        InventoryManager.I.AddItem(tomatoCropItem, 1);
        QuestManager.Instance.AdvanceStep(tomatoQuestId);
        PlayerRelationshipTracker.instance.ChangeRelationship(mapleNPC, +1);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
