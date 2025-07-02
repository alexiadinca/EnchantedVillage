using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNPC", menuName = "NPC")]
public class NPCData : ScriptableObject
{
    public string npcName;
    public string questId;
    public string personality;
    public string skillToTeach;

    [TextArea] public string forgottenMemory;
    public List<NPCData> forgottenRelationships;
    public int friendshipLevel;

    public List<string> likes;
    public List<string> dislikes;
    public List<string> hobbies;

    [Header("Quest Info")]
    [TextArea] public string questIntro;
    public List<string> questSteps;
    [TextArea] public string rewardDescription;

    public void RestoreMemory(string memory)
    {
        forgottenMemory = "";
        friendshipLevel += 10;
    }

    public void FailMemoryRestore(string memory)
    {
        friendshipLevel -= 5;
    }

}
