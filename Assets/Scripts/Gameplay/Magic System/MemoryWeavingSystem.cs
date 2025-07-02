using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Linq;

public class MemoryWeavingSystem : MonoBehaviour
{
    public GameObject memoryUI;
    public List<MemoryThread> selectedThreads = new List<MemoryThread>();
    public List<SpellRecipe> allRecipes;

    public TMP_Dropdown memoryDropdown;
    public TMP_Text feedbackText;

    public TMP_Dropdown npcDropdown;
    public List<NPCData> npcDatabase;

    private bool isActive = false;
    private NPCData currentTarget;

    public Image[] threadSlots; // Slot1, Slot2, Slot3 into this array
    public Sprite goldIcon, blueIcon, redIcon, silverIcon;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMemoryWeaving();
        }
    }

    void ToggleMemoryWeaving()
    {
        isActive = !isActive;
        memoryUI.SetActive(isActive);
        Time.timeScale = isActive ? 0 : 1;
    }

    public void AddThread(MemoryThread thread)
    {
        if (selectedThreads.Count < 3)
            selectedThreads.Add(thread);
    }

    public void SelectNPCFromDropdown()
    {
        if (npcDropdown == null || npcDatabase == null || npcDropdown.value >= npcDatabase.Count)
        {
            Debug.LogWarning("Dropdown or database not set correctly.");
            return;
        }

        SetTargetNPC(npcDatabase[npcDropdown.value]);
        FilterMemoriesForNPC();
    }

    void Start()
    {
        if (npcDropdown != null)
        {
            npcDropdown.ClearOptions();
            List<string> npcNames = npcDatabase.Select(n => n.npcName).ToList();
            npcDropdown.AddOptions(npcNames);
        }
    }

    public void AttemptSpellFromDropdown()
    {
        if (memoryDropdown == null || memoryDropdown.options.Count == 0)
            return;

        string selectedMemory = memoryDropdown.options[memoryDropdown.value].text;
        AttemptSpell(selectedMemory);
    }

    public void AttemptSpell(string memoryName)
    {
        SpellRecipe recipe = allRecipes.Find(r => r.memoryName == memoryName && r.npcName == currentTarget.npcName);
        if (recipe != null && MatchThreads(recipe.correctThreads, selectedThreads))
        {
            currentTarget.RestoreMemory(memoryName);
            feedbackText.text = $"Memory restored for {currentTarget.npcName}!";
            PlayerRelationshipTracker.instance.ChangeRelationship(currentTarget, +1);
        }
        else
        {
            currentTarget.FailMemoryRestore(memoryName);
            feedbackText.text = $"Incorrect spell! Memory not restored.";
            PlayerRelationshipTracker.instance.ChangeRelationship(currentTarget, -1);
        }

        selectedThreads.Clear();
        ToggleMemoryWeaving();
    }

    public void SelectThread(int threadIndex)
    {
        if (selectedThreads.Count >= 3) return;

        MemoryThread thread = (MemoryThread)threadIndex;
        selectedThreads.Add(thread);

        Sprite icon = null;
        switch (thread)
        {
            case MemoryThread.Gold: icon = goldIcon; break;
            case MemoryThread.Blue: icon = blueIcon; break;
            case MemoryThread.Red: icon = redIcon; break;
            case MemoryThread.Silver: icon = silverIcon; break;
        }

        threadSlots[selectedThreads.Count - 1].sprite = icon;
    }


    bool MatchThreads(MemoryThread[] correct, List<MemoryThread> selected)
    {
        if (correct.Length != selected.Count)
            return false;

        var tempList = new List<MemoryThread>(selected);
        foreach (var thread in correct)
        {
            if (!tempList.Remove(thread))
                return false;
        }
        return true;
    }

    public void SetTargetNPC(NPCData npc)
    {
        currentTarget = npc;
        Debug.Log($"Target set to: {npc.npcName}");
    }

    public void FilterMemoriesForNPC()
    {
        if (currentTarget == null || memoryDropdown == null) return;

        memoryDropdown.ClearOptions();

        List<string> filteredNames = allRecipes
            .Where(r => r.npcName == currentTarget.npcName)
            .Select(r => r.memoryName)
            .ToList();

        memoryDropdown.AddOptions(filteredNames);
    }

    public void ClearSelectedThreads()
    {
        selectedThreads.Clear();

        for (int i = 0; i < threadSlots.Length; i++)
        {
            threadSlots[i].sprite = null; // or a default question mark sprite
        }

        if (feedbackText != null)
            feedbackText.text = "AttemptSpell called!";
        else
            Debug.LogWarning("FeedbackText is not assigned!");
    }

}
