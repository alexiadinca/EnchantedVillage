using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellRecipe", menuName = "Memory Weaving/Spell Recipe")]
public class SpellRecipe : ScriptableObject
{
    public string memoryName;
    public string npcName;
    public MemoryThread[] correctThreads;
}
