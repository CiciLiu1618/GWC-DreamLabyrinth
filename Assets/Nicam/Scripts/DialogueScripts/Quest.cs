using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Dialogue/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea] public string questDescription;

    public QuestRequirementType requirementType;
    public List<QuestRequirement> requirements = new List<QuestRequirement>();

    public bool isCompleted = false;
    public bool isActive = false;
}

[System.Serializable]
public class QuestRequirement
{
    public RequirementType type;
    public string targetID; // NPC name or item ID
    public int requiredAmount = 1;
    public int currentAmount = 0;

    public bool IsCompleted()
    {
        return currentAmount >= requiredAmount;
    }
}

public enum QuestRequirementType
{
    TalkToNPC,
    CollectItems,
    Mixed
}

public enum RequirementType
{
    TalkToNPC,
    CollectItem
}