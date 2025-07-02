using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FinnFishingInteraction : MonoBehaviour
{
    [Header("NPC & Quest")]
    public NPCData npcData;
    public string questId;

    [Header("Interaction")]
    [Tooltip("How close the player needs to be to talk/give fish")]
    public float interactionRadius = 8f;

    [Header("Items & Prefabs")]
    public ItemData fishingRodItem;
    public GameObject fishingRodWorldPrefab;
    public ItemData fishItem;
    public GameObject fishWorldPrefab;

    [Header("UI")]
    public GameObject pressEPrompt;
    public GameObject dialoguePanel;
    public Button continueButton;

    // Internal reference to the text component
    private TMP_Text promptText;

    bool hasTalked = false;
    Transform playerT;

    void Awake()
    {
        // Cache the text component inside the prompt
        if (pressEPrompt != null)
        {
            promptText = pressEPrompt.GetComponentInChildren<TMP_Text>();
            pressEPrompt.SetActive(false);
        }
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnGiveRod);
    }

    void Update()
    {
        if (playerT == null)
            playerT = GameObject.FindWithTag("Player")?.transform;
        if (playerT == null) return;

        float dist = Vector3.Distance(playerT.position, transform.position);
        bool inRange = dist <= interactionRadius;

        // Prompt logic
        if (pressEPrompt != null)
        {
            if (inRange)
            {
                pressEPrompt.SetActive(true);
                if (promptText != null)
                {
                    if (!hasTalked)
                        promptText.text = "Talk to Finn (E)";
                    else if (InventoryManager.I.HasItem(fishItem))
                        promptText.text = "Give fish to Finn (E)";
                }
            }
            else
            {
                pressEPrompt.SetActive(false);
            }
        }

        // Interaction
        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Player pressed E near Finn");

            if (!hasTalked)
            {
                Debug.Log("Starting dialogue with Finn");
                hasTalked = true;
                StartDialogue();
            }
            else
            {
                int count = InventoryManager.I.GetItemCount(fishItem);
                bool hasFish = InventoryManager.I.HasItem(fishItem);

                Debug.Log($"[DEBUG] fishItem.name = {fishItem.name}, GetItemCount = {count}, HasItem = {hasFish}");

                if (hasFish)
                {
                    Debug.Log("Player has fish. Completing quest...");
                    CompleteFishingQuest();
                }
                else
                {
                    Debug.Log("Player talked to Finn but has no fish.");
                }
            }
        }
    }



    void StartDialogue()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);
        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Show($"{npcData.npcName}: Here, take my rod—catch me a fish!");
    }

    void OnGiveRod()
    {
        // 1) Make sure QuestManager is actually there
        if (QuestManager.Instance == null)
        {
            Debug.LogError($"[FinnFishingInteraction] QuestManager.Instance is null! " +
                           "Make sure a QuestManager exists in the scene.");
            return;
        }

        // 2) Give the player the rod
        InventoryManager.I.AddItem(fishingRodItem, 1);

        // 3) Spawn the world-space rod prefab
        if (playerT != null && fishingRodWorldPrefab != null)
        {
            Vector3 spawnPos = playerT.position + playerT.forward * 0.5f + Vector3.up;
            Instantiate(fishingRodWorldPrefab, spawnPos, Quaternion.identity);
        }

        // 4) Look up the quest by ID—and null-check it!
        var q = QuestManager.Instance.Get(questId);
        if (q == null)
        {
            Debug.LogError($"[FinnFishingInteraction] no quest found with ID '{questId}'. " +
                           "Did you set the Quest Id in the Inspector exactly?");
            return;
        }

        // 5) Unlock / start as appropriate
        if (q.status == QuestStatus.Locked)
            QuestManager.Instance.Unlock(questId);
        if (q.status == QuestStatus.Available)
            QuestManager.Instance.StartQuest(questId);

        // 6) Turn on fishing
        if (FishingSystem.Instance != null)
            FishingSystem.Instance.canFish = true;
        else
            Debug.LogError("[FinnFishingInteraction] FishingSystem.Instance is null!");

        // 7) Hide dialogue
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Hide();
    }


    void CompleteFishingQuest()
    {
        var q = QuestManager.Instance.Get(questId);
        if (q == null)
        {
            Debug.LogError($"[FinnFishingInteraction] No quest found with ID '{questId}' when trying to complete quest.");
            return;
        }

        InventoryManager.I.RemoveItem(fishItem, 1);
        q.status = QuestStatus.Completed;
        PlayerRelationshipTracker.instance.ChangeRelationship(npcData, +1);

        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Show($"{npcData.npcName}: Thanks! You’re a lifesaver. (Friendship ↑)");

        Invoke(nameof(HidePrompt), 2f);
    }


    void HidePrompt()
    {
        if (pressEPrompt != null)
            pressEPrompt.SetActive(false);
        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Hide();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
#endif
}