using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class NPCInteraction : MonoBehaviour
{
    [Header("NPC Settings")]
    public NPCData npcData;
    public ItemData axeItem;
    public GameObject axeWorldPrefab;
    public Vector3 axeSpawnOffset = new Vector3(0, 0, 0.5f);

    [Header("Interaction Settings")]
    public float interactionRadius = 2f;

    [Header("UI Settings")]
    public GameObject pressEPrompt;

    [Header("Dialogue UI")]
    public GameObject dialoguePanel;
    public Button continueButton;

    [Header("Quest Items")]
    public ItemData requiredItem;        // Wood Plank
    public List<ItemData> rewardItems;  // Furniture

    [Header("Friendship UI")]
    public GameObject friendshipCanvas;

    bool hasTalked = false;
    Transform playerT;

    Text _uiText;
    // TMP_Text _uiTextTMP;

    void Awake()
    {
        if (pressEPrompt != null)
        {
            _uiText = pressEPrompt.GetComponent<Text>();
            if (_uiText == null)
                _uiText = pressEPrompt.GetComponentInChildren<Text>();
        }

        if (pressEPrompt != null) pressEPrompt.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (continueButton != null)
            continueButton.onClick.AddListener(OnContinueClicked);
    }

    void Update()
    {
        if (playerT == null)
        {
            var go = GameObject.FindWithTag("Player");
            if (go == null) return;
            playerT = go.transform;
        }

        float dist = Vector3.Distance(playerT.position, transform.position);
        bool inRange = dist <= interactionRadius;

        if (pressEPrompt != null)
        {
            if (inRange)
            {
                pressEPrompt.SetActive(true);
                if (_uiText != null)
                {
                    if (!hasTalked)
                        _uiText.text = "Talk to Griff (E)";
                    else if (InventoryManager.I.HasItem(requiredItem))
                        _uiText.text = "Give plank to Griff (E)";
                    else
                        _uiText.text = "Chop a tree (left-click)";
                }
            }
            else
            {
                pressEPrompt.SetActive(false);
            }
        }

        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            if (!hasTalked)
            {
                hasTalked = true;
                pressEPrompt.SetActive(false);
                StartConversation();
            }
            else if (requiredItem != null && InventoryManager.I.HasItem(requiredItem))
            {
                CompleteQuest();
            }
        }

        if (friendshipCanvas != null)
            friendshipCanvas.SetActive(inRange);
    }


    void StartConversation()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        if (DialogueUI.Instance != null && npcData != null)
            DialogueUI.Instance.Show($"{npcData.npcName}: Hello! Could you help me cut some trees with this axe? You can keep it.");
        else
            Debug.LogError("[NPCInteraction] npcData or DialogueUI.Instance is missing!");
    }


    void OnContinueClicked()
    {
        Debug.Log("OnContinueClicked: giving axe");

        if (axeItem != null)
            InventoryManager.I.AddItem(axeItem, 1);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (DialogueUI.Instance != null) DialogueUI.Instance.Hide();

        if (axeWorldPrefab != null && playerT != null)
        {
            Vector3 spawnPos =
                playerT.position +
                playerT.forward * axeSpawnOffset.z +
                Vector3.up * axeSpawnOffset.y +
                playerT.right * axeSpawnOffset.x;
            Instantiate(axeWorldPrefab, spawnPos, Quaternion.identity);
        }

        if (npcData != null && !string.IsNullOrEmpty(npcData.questId))
        {
            var q = QuestManager.Instance.Get(npcData.questId);
            if (q != null)
            {
                if (q.status == QuestStatus.Locked) QuestManager.Instance.Unlock(npcData.questId);
                if (q.status == QuestStatus.Available) QuestManager.Instance.StartQuest(npcData.questId);
            }
        }
        else
        {
            Debug.LogError("[NPCInteraction] Missing npcData or questId");
        }

        WoodcuttingSystem.Instance.canCut = true;
    }

    void CompleteQuest()
    {
        InventoryManager.I.RemoveItem(requiredItem, 1);

        int currentStep = 0;
        var q = QuestManager.Instance?.Get(npcData.questId);
        if (q != null)
            currentStep = q.currentStep;

        if (currentStep < rewardItems.Count)
        {
            ItemData reward = rewardItems[currentStep];
            InventoryManager.I.AddItem(reward, 1);

            if (DialogueUI.Instance != null)
                DialogueUI.Instance.Show($"{npcData.npcName}: Thanks! Here's a {reward.itemName} for your home.");
        }
        else
        {
            if (DialogueUI.Instance != null)
                DialogueUI.Instance.Show($"{npcData.npcName}: Thanks again!");
        }

        PlayerRelationshipTracker.instance.ChangeRelationship(npcData, +1);

        if (q != null)
        {
            QuestManager.Instance.AdvanceStep(npcData.questId);
        }

        Invoke(nameof(HidePrompt), 3f);
    }


    void HidePrompt()
    {
        if (pressEPrompt != null)
            pressEPrompt.SetActive(false);

        if (DialogueUI.Instance != null)
            DialogueUI.Instance.Hide();
    }


}
