using TMPro;
using UnityEngine;

public class RelationshipUIManager : MonoBehaviour
{
    public static RelationshipUIManager instance;

    public GameObject uiPanel;
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI relationshipText;

    void Awake()
    {
        instance = this;
        uiPanel.SetActive(false);
    }

    public void Show(NPCData npc, int level)
    {
        npcNameText.text = npc.npcName;
        relationshipText.text = $"Relationship Level: {level}/5";
        uiPanel.SetActive(true);
    }

    public void Hide()
    {
        uiPanel.SetActive(false);
    }
}
