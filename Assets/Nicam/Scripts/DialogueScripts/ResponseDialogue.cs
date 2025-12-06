using UnityEngine;
using XNode;
using static XNode.Node;

public class ResponseDialogue : DialogueNodeBase
{
    [Input(typeConstraint = TypeConstraint.Strict)] public int entry;
    [Output(connectionType = ConnectionType.Override)] public bool exit;

    [Header("Conditional Response")]
    public DialogueConditionType conditionType = DialogueConditionType.None;
    public string requiredQuestID;

    public override string GetDialogueType { get { return "Response"; } }

    public bool CanShowResponse()
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