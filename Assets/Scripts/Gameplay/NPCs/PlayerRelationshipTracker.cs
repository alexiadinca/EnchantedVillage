using System.Collections.Generic;
using UnityEngine;

public class PlayerRelationshipTracker : MonoBehaviour
{
    public static PlayerRelationshipTracker instance;

    [System.Serializable]
    public class NPCStatus
    {
        public NPCData npc;
        public bool hasMet = false;
        public bool skillLearned = false;
        public bool memoryRestored = false;

        [Range(1, 5)]
        public int relationshipLevel = 1; // starts cold
    }

    public List<NPCStatus> npcStatuses = new List<NPCStatus>();

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        // ← replaced FindObjectsOfType with FindObjectsByType<…>(None)
        foreach (var npc in Object.FindObjectsByType<NPCInteraction>(FindObjectsSortMode.None))
        {
            GetNPCStatus(npc.npcData);
        }
    }

    public NPCStatus GetNPCStatus(NPCData npc)
    {
        foreach (var status in npcStatuses)
        {
            if (status.npc == npc)
                return status;
        }

        // If the NPC doesn't exist in the list, add a new one
        NPCStatus newStatus = new NPCStatus { npc = npc };
        npcStatuses.Add(newStatus);
        return newStatus;
    }

    public void ChangeRelationship(NPCData npc, int amount)
    {
        var status = GetNPCStatus(npc);
        int oldLevel = status.relationshipLevel;
        status.relationshipLevel = Mathf.Clamp(status.relationshipLevel + amount, 1, 5);

        if (amount > 0)
        {
            Debug.Log($"Your relationship with {npc.npcName} increased to Level {status.relationshipLevel}!");
        }
        else if (amount < 0)
        {
            Debug.Log($"Your relationship with {npc.npcName} decreased to Level {status.relationshipLevel}.");
        }

        if (oldLevel != status.relationshipLevel)
        {
            //Refresh the displays—also using the new API
            foreach (var display in Object.FindObjectsByType<FriendshipDisplay>(FindObjectsSortMode.None))
            {
                display.ForceRefresh();
            }
        }
    }
}
