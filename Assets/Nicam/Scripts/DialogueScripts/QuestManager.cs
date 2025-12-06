using System.Collections.Generic;
using UnityEngine;

// ============================================================================
// QUEST MANAGER
// ============================================================================
// Singleton manager that tracks all active and completed quests
// Handles quest starting, progress tracking, and completion
// ============================================================================

public class QuestManager : MonoBehaviour
{
    // ========================================================================
    // SINGLETON
    // ========================================================================
    public static QuestManager Instance { get; private set; }

    // ========================================================================
    // PRIVATE VARIABLES
    // ========================================================================
    private Dictionary<string, Quest> activeQuests = new Dictionary<string, Quest>();
    private Dictionary<string, Quest> completedQuests = new Dictionary<string, Quest>();

    // ========================================================================
    // UNITY LIFECYCLE
    // ========================================================================

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========================================================================
    // QUEST MANAGEMENT
    // ========================================================================

    public void StartQuest(Quest quest)
    {
        if (quest == null || activeQuests.ContainsKey(quest.questID))
            return;

        quest.isActive = true;
        quest.isCompleted = false;
        activeQuests.Add(quest.questID, quest);

        Debug.Log($"Quest Started: {quest.questName}");

        // Notify UI
        if (QuestUIManager.Instance != null)
        {
            QuestUIManager.Instance.OnQuestStarted(quest);
        }
    }

    public void UpdateQuestProgress(string questID, string targetID, RequirementType type)
    {
        if (!activeQuests.ContainsKey(questID))
            return;

        Quest quest = activeQuests[questID];

        foreach (QuestRequirement req in quest.requirements)
        {
            if (req.type == type && req.targetID == targetID)
            {
                req.currentAmount++;
                Debug.Log($"Quest Progress: {req.targetID} - {req.currentAmount}/{req.requiredAmount}");

                // Notify UI
                if (QuestUIManager.Instance != null)
                {
                    QuestUIManager.Instance.OnQuestProgressUpdated(quest);
                }

                // Check completion
                if (CheckQuestCompletion(quest))
                {
                    CompleteQuest(questID);
                }
                break;
            }
        }
    }

    public void ForceCompleteQuest(string questID)
    {
        if (activeQuests.ContainsKey(questID))
        {
            CompleteQuest(questID);
        }
        else
        {
            Debug.LogWarning($"Cannot complete quest {questID} - Quest is not active!");
        }
    }

    private bool CheckQuestCompletion(Quest quest)
    {
        foreach (QuestRequirement req in quest.requirements)
        {
            if (!req.IsCompleted())
                return false;
        }
        return true;
    }

    private void CompleteQuest(string questID)
    {
        if (!activeQuests.ContainsKey(questID))
            return;

        Quest quest = activeQuests[questID];
        quest.isCompleted = true;
        quest.isActive = false;

        activeQuests.Remove(questID);
        completedQuests.Add(questID, quest);

        Debug.Log($"Quest Completed: {quest.questName}");

        // Notify UI
        if (QuestUIManager.Instance != null)
        {
            QuestUIManager.Instance.OnQuestCompleted(quest);
        }
    }

    // ========================================================================
    // QUERY METHODS
    // ========================================================================

    public bool IsQuestCompleted(string questID)
    {
        return completedQuests.ContainsKey(questID);
    }

    public bool IsQuestActive(string questID)
    {
        return activeQuests.ContainsKey(questID);
    }

    public Quest GetQuest(string questID)
    {
        if (activeQuests.ContainsKey(questID))
            return activeQuests[questID];
        if (completedQuests.ContainsKey(questID))
            return completedQuests[questID];
        return null;
    }
}