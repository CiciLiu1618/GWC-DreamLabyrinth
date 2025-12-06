using UnityEngine;

public class NPCDialogue : DialogueNodeBase
{
    [Input(typeConstraint = TypeConstraint.Strict)] public bool entry;
    [Output(dynamicPortList = true, connectionType = ConnectionType.Override)] public int exit;

    [Header("Speaker Info")]
    public string speakerName = "NPC";

    [Header("Quest Settings")]
    public bool startsQuest = false;
    public Quest questToStart;

    // *** ADDED: Quest completion settings ***
    public bool completesQuest = false;
    public string questToCompleteID;
    // *** END ADDITION ***

    [Header("Conditional Dialogue")]
    public DialogueConditionType conditionType = DialogueConditionType.None;
    public string requiredQuestID;

    public override string GetDialogueType { get { return "NPC"; } }

    public bool CanShowDialogue()
    {
        if (conditionType == DialogueConditionType.None)
            return true;

        if (QuestManager.Instance == null)
            return false;

        switch (conditionType)
        {
            case DialogueConditionType.QuestCompleted:
                return QuestManager.Instance.IsQuestCompleted(requiredQuestID);

            case DialogueConditionType.QuestActive:
                return QuestManager.Instance.IsQuestActive(requiredQuestID);

            case DialogueConditionType.QuestNotActive:
                return !QuestManager.Instance.IsQuestActive(requiredQuestID) &&
                       !QuestManager.Instance.IsQuestCompleted(requiredQuestID);

            default:
                return true;
        }
    }
}

public enum DialogueConditionType
{
    None,
    QuestCompleted,
    QuestActive,
    QuestNotActive
}