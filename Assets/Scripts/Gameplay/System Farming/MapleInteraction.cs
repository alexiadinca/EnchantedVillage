using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class MapleInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [Tooltip("How close (in world units) you must be to talk")]
    public float interactionRadius = 3f;
    [Tooltip("World-space UI element that says “Press E”")]
    public GameObject pressEPrompt;

    [Header("Dialogue & Quest")]
    [Tooltip("Name as displayed in dialogue")]
    public string mapleName = "Maple";
    [TextArea, Tooltip("What Maple says when you press E")]
    public string greetingLine = "Hello there! Here, take this tomato seed.";
    [Tooltip("Must exactly match your QuestManager quest.Id")]
    public string tomatoQuestId = "TomatoQuest";
    [Tooltip("The ItemData asset for the seed Maple gives you")]
    public ItemData tomatoSeedItem;

    // internal state
    bool hasGiven = false;
    Transform playerT;

    void Awake()
    {
        // ensure a SphereCollider trigger with the correct radius
        var sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = interactionRadius;
        sc.center = Vector3.zero;

        // hide the prompt at start
        if (pressEPrompt)
            pressEPrompt.SetActive(false);
    }

    void Update()
    {
        // 1) find the Player once by tag
        if (playerT == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go != null) playerT = go.transform;
            else return; // no Player yet
        }

        // 2) distance check
        float dist = Vector3.Distance(playerT.position, transform.position);
        bool inRange = dist <= interactionRadius;

        // 3) show/hide Press E prompt
        if (pressEPrompt)
            pressEPrompt.SetActive(inRange && !hasGiven);

        // 4) on E press, if in range and not already given
        if (inRange && !hasGiven && Input.GetKeyDown(KeyCode.E))
        {
            hasGiven = true;
            if (pressEPrompt)
                pressEPrompt.SetActive(false);

            // show dialogue
            DialogueUI.Instance?.Show($"{mapleName}: {greetingLine}");

            // give the seed
            InventoryManager.I.AddItem(tomatoSeedItem, 1);

            // unlock & start quest
            var q = QuestManager.Instance.Get(tomatoQuestId);
            if (q != null)
            {
                QuestManager.Instance.Unlock(tomatoQuestId);
                QuestManager.Instance.StartQuest(tomatoQuestId);
            }
        }
    }

    // visualize radius in the Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}
