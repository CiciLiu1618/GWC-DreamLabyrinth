using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    // ========================================================================
    // SINGLETON
    // ========================================================================
    public static QuestUIManager Instance { get; private set; }

    // ========================================================================
    // UI REFERENCES
    // ========================================================================
    [Header("UI References")]
    [SerializeField] private GameObject questCanvas;              // Main quest UI canvas
    [SerializeField] private GameObject questPanel;               // Container for quest info
    [SerializeField] private GameObject questEntryPrefab;         // Prefab for each quest (multi-quest mode)
    [SerializeField] private Transform questListContainer;        // Parent for quest entries (multi-quest mode)

    // ========================================================================
    // SINGLE QUEST DISPLAY COMPONENTS
    // ========================================================================
    [Header("Single Quest Display")]
    [SerializeField] private GameObject singleQuestPanel;         // Panel for single quest display
    [SerializeField] private TextMeshProUGUI questTitleText;      // Quest name
    [SerializeField] private TextMeshProUGUI questDescriptionText;// Quest description
    [SerializeField] private Transform objectiveListContainer;    // Parent for objective entries
    [SerializeField] private GameObject objectivePrefab;          // Prefab for each objective line

    // ========================================================================
    // SETTINGS
    // ========================================================================
    [Header("Settings")]
    [SerializeField] private bool useMultipleQuestDisplay = false; // Toggle between single/multiple quest display

    // ========================================================================
    // PRIVATE VARIABLES
    // ========================================================================
    // Track displayed quests (for multiple quest mode)
    private Dictionary<string, GameObject> activeQuestEntries = new Dictionary<string, GameObject>();

    // ========================================================================
    // UNITY LIFECYCLE
    // ========================================================================

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize UI - start hidden
        if (questCanvas != null)
            questCanvas.SetActive(false);
    }

    // ========================================================================
    // PUBLIC QUEST EVENT HANDLERS
    // ========================================================================

    /// <summary>
    /// Called when a quest is started - displays quest UI
    /// </summary>
    public void OnQuestStarted(Quest quest)
    {
        if (quest == null)
            return;

        Debug.Log($"QuestUI: Displaying quest {quest.questName}");

        // Show quest canvas
        if (questCanvas != null)
            questCanvas.SetActive(true);

        // Display based on mode
        if (useMultipleQuestDisplay)
        {
            CreateQuestEntry(quest);
        }
        else
        {
            DisplaySingleQuest(quest);
        }
    }

    /// <summary>
    /// Called when quest progress is updated - refreshes UI
    /// </summary>
    public void OnQuestProgressUpdated(Quest quest)
    {
        if (quest == null)
            return;

        Debug.Log($"QuestUI: Updating quest {quest.questName}");

        // Update based on mode
        if (useMultipleQuestDisplay)
        {
            UpdateQuestEntry(quest);
        }
        else
        {
            DisplaySingleQuest(quest);
        }
    }

    /// <summary>
    /// Called when a quest is completed - removes from UI
    /// </summary>
    public void OnQuestCompleted(Quest quest)
    {
        if (quest == null)
            return;

        Debug.Log($"QuestUI: Quest completed {quest.questName}");

        // Remove based on mode
        if (useMultipleQuestDisplay)
        {
            RemoveQuestEntry(quest.questID);
        }
        else
        {
            HideSingleQuest();
        }

        // Hide canvas if no active quests remain
        if (QuestManager.Instance != null && !HasActiveQuests())
        {
            if (questCanvas != null)
                questCanvas.SetActive(false);
        }
    }

    // ========================================================================
    // MULTIPLE QUEST DISPLAY METHODS
    // ========================================================================

    /// <summary>
    /// Creates a new quest entry in the quest list
    /// </summary>
    private void CreateQuestEntry(Quest quest)
    {
        if (questEntryPrefab == null || questListContainer == null)
            return;

        // Don't create duplicate entries
        if (activeQuestEntries.ContainsKey(quest.questID))
        {
            UpdateQuestEntry(quest);
            return;
        }

        // Instantiate quest entry
        GameObject entry = Instantiate(questEntryPrefab, questListContainer);
        activeQuestEntries.Add(quest.questID, entry);

        // Populate entry with quest data
        UpdateQuestEntryContent(entry, quest);
    }

    /// <summary>
    /// Updates an existing quest entry with new progress
    /// </summary>
    private void UpdateQuestEntry(Quest quest)
    {
        if (!activeQuestEntries.ContainsKey(quest.questID))
            return;

        GameObject entry = activeQuestEntries[quest.questID];
        UpdateQuestEntryContent(entry, quest);
    }

    /// <summary>
    /// Populates quest entry UI with quest data
    /// </summary>
    private void UpdateQuestEntryContent(GameObject entry, Quest quest)
    {
        // Find text components in the entry
        TextMeshProUGUI[] texts = entry.GetComponentsInChildren<TextMeshProUGUI>();

        // Set title
        if (texts.Length > 0)
            texts[0].text = quest.questName;

        // Set objectives
        if (texts.Length > 1)
        {
            // Build objectives text with checkmarks
            string objectivesText = "";
            foreach (QuestRequirement req in quest.requirements)
            {
                string checkmark = req.IsCompleted() ? "✓" : "○";
                objectivesText += $"{checkmark} {GetRequirementDescription(req)}\n";
            }
            texts[1].text = objectivesText;
        }
    }

    /// <summary>
    /// Removes a quest entry from the list
    /// </summary>
    private void RemoveQuestEntry(string questID)
    {
        if (activeQuestEntries.ContainsKey(questID))
        {
            Destroy(activeQuestEntries[questID]);
            activeQuestEntries.Remove(questID);
        }
    }

    // ========================================================================
    // SINGLE QUEST DISPLAY METHODS
    // ========================================================================

    /// <summary>
    /// Displays a single quest with full details
    /// </summary>
    private void DisplaySingleQuest(Quest quest)
    {
        if (singleQuestPanel == null)
            return;

        singleQuestPanel.SetActive(true);

        // Set title
        if (questTitleText != null)
            questTitleText.text = quest.questName;

        // Set description
        if (questDescriptionText != null)
            questDescriptionText.text = quest.questDescription;

        // Clear and rebuild objective list
        ClearObjectiveList();

        // Create objective entries
        if (objectiveListContainer != null && objectivePrefab != null)
        {
            foreach (QuestRequirement req in quest.requirements)
            {
                CreateObjectiveEntry(req);
            }
        }
    }

    /// <summary>
    /// Creates a single objective entry in the list
    /// </summary>
    private void CreateObjectiveEntry(QuestRequirement requirement)
    {
        if (objectivePrefab == null || objectiveListContainer == null)
            return;

        // Instantiate objective
        GameObject objective = Instantiate(objectivePrefab, objectiveListContainer);

        // Set objective text
        TextMeshProUGUI text = objective.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            string checkmark = requirement.IsCompleted() ? "✓" : "○";
            text.text = $"{checkmark} {GetRequirementDescription(requirement)}";

            // Change color based on completion
            if (requirement.IsCompleted())
            {
                text.color = Color.green;
            }
        }

        // Toggle checkmark image if present
        Image checkImage = objective.GetComponentInChildren<Image>();
        if (checkImage != null)
        {
            checkImage.enabled = requirement.IsCompleted();
        }
    }

    /// <summary>
    /// Clears all objective entries from the list
    /// </summary>
    private void ClearObjectiveList()
    {
        if (objectiveListContainer == null)
            return;

        foreach (Transform child in objectiveListContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Hides the single quest panel
    /// </summary>
    private void HideSingleQuest()
    {
        if (singleQuestPanel != null)
            singleQuestPanel.SetActive(false);
    }

    // ========================================================================
    // HELPER METHODS
    // ========================================================================

    /// <summary>
    /// Generates human-readable description for a requirement
    /// </summary>
    private string GetRequirementDescription(QuestRequirement req)
    {
        switch (req.type)
        {
            case RequirementType.TalkToNPC:
                return $"Talk to {req.targetID}";

            case RequirementType.CollectItem:
                return $"Collect {req.targetID}: {req.currentAmount}/{req.requiredAmount}";

            default:
                return $"{req.targetID}: {req.currentAmount}/{req.requiredAmount}";
        }
    }

    /// <summary>
    /// Checks if there are any active quests being displayed
    /// </summary>
    private bool HasActiveQuests()
    {
        if (useMultipleQuestDisplay)
        {
            return activeQuestEntries.Count > 0;
        }
        else
        {
            return singleQuestPanel != null && singleQuestPanel.activeSelf;
        }
    }

    // ========================================================================
    // PUBLIC UTILITY METHODS
    // ========================================================================

    /// <summary>
    /// Manually refresh display (call if quest updated externally)
    /// </summary>
    public void RefreshQuestDisplay(Quest quest)
    {
        if (quest == null)
            return;

        if (quest.isActive)
        {
            OnQuestProgressUpdated(quest);
        }
    }

    /// <summary>
    /// Toggle quest UI visibility on/off
    /// </summary>
    public void ToggleQuestUI()
    {
        if (questCanvas != null)
        {
            questCanvas.SetActive(!questCanvas.activeSelf);
        }
    }
}