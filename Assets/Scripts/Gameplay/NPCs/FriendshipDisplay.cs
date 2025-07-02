using UnityEngine;
using TMPro;

public class FriendshipDisplay : MonoBehaviour
{
    public NPCData npcData;

    private Camera cam;
    private TextMeshProUGUI text;

    void Start()
    {
        cam = Camera.main;
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        if (cam != null)
            transform.forward = cam.transform.forward;

        if (text != null && npcData != null && PlayerRelationshipTracker.instance != null)
        {
            int level = Mathf.Clamp(PlayerRelationshipTracker.instance.GetNPCStatus(npcData).relationshipLevel, 1, 5);
            text.text = $"Friendship Level: {level}";
        }
    }

    public void ForceRefresh()
    {
        if (text != null && npcData != null && PlayerRelationshipTracker.instance != null)
        {
            int level = Mathf.Clamp(PlayerRelationshipTracker.instance.GetNPCStatus(npcData).relationshipLevel, 1, 5);
            text.text = $"Friendship Level: {level}";
            Debug.Log($"Display for {npcData.npcName}: Level = {level}");
        }
    }
}
