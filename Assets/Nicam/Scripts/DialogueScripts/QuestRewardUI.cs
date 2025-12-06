using UnityEngine;
using TMPro;
using UnityEngine.UI;

// ============================================================================
// QUEST REWARD UI
// ============================================================================
// Makes canvas transparent when quest incomplete, visible when complete
// Press J to toggle visibility (only works when quest is complete)
// ============================================================================

public class QuestRewardUI : MonoBehaviour
{
    // ========================================================================
    // INSPECTOR SETTINGS
    // ========================================================================

    [Header("Quest Requirement")]
    [SerializeField] private string requiredQuestID;              // Quest that must be completed

    [Header("UI Settings")]
    [SerializeField] private GameObject rewardCanvas;             // Canvas to show/hide
    [SerializeField] private KeyCode openKey = KeyCode.J;         // Key to toggle visibility
    [SerializeField] private KeyCode closeKey = KeyCode.Escape;   // Key to close canvas

    [Header("Transparency Settings")]
    [SerializeField] private float transparentAlpha = 0f;         // Alpha when hidden (0 = invisible)
    [SerializeField] private float visibleAlpha = 1f;             // Alpha when visible (1 = fully visible)
    [SerializeField] private bool hideWhenTransparent = true;     // Also disable raycast when transparent

    [Header("Feedback (Optional)")]
    [SerializeField] private string lockedMessage = "Complete the quest first!";
    [SerializeField] private GameObject lockedPrompt;             // UI element showing locked message
    [SerializeField] private float lockedPromptDuration = 2f;     // How long to show locked message

    [Header("Player Control (Optional)")]
    [SerializeField] private MonoBehaviour playerController;      // Disable player when canvas open

    // ========================================================================
    // PRIVATE VARIABLES
    // ========================================================================

    private bool isCanvasVisible = false;
    private float lockedPromptTimer = 0f;
    private CanvasGroup canvasGroup;

    // ========================================================================
    // UNITY LIFECYCLE
    // ========================================================================

    void Start()
    {
        Debug.Log($"=== QuestRewardUI Started ===");
        Debug.Log($"Required Quest ID: {requiredQuestID}");
        Debug.Log($"Open Key: {openKey}");
        Debug.Log($"Reward Canvas: {(rewardCanvas != null ? rewardCanvas.name : "NULL")}");

        // Setup canvas group for transparency control
        if (rewardCanvas != null)
        {
            canvasGroup = rewardCanvas.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = rewardCanvas.AddComponent<CanvasGroup>();
                Debug.Log("Added CanvasGroup component to reward canvas");
            }

            // Start transparent (invisible)
            SetCanvasTransparency(transparentAlpha);
            Debug.Log($"Set canvas to transparent (alpha: {transparentAlpha})");
        }
        else
        {
            Debug.LogError("REWARD CANVAS NOT ASSIGNED IN INSPECTOR!");
        }

        // Ensure locked prompt starts hidden
        if (lockedPrompt != null)
        {
            lockedPrompt.SetActive(false);
        }

        // Check initial quest state
        Debug.Log($"Initial quest state check:");
        IsQuestCompleted();
    }

    void Update()
    {
        // Handle locked prompt timer
        if (lockedPromptTimer > 0)
        {
            lockedPromptTimer -= Time.deltaTime;
            if (lockedPromptTimer <= 0 && lockedPrompt != null)
            {
                lockedPrompt.SetActive(false);
            }
        }

        // Check for open key
        if (Input.GetKeyDown(openKey))
        {
            Debug.Log($"=== J KEY PRESSED ===");
            Debug.Log($"Canvas Visible: {isCanvasVisible}");
            Debug.Log($"Required Quest: {requiredQuestID}");
            Debug.Log($"Quest Completed: {IsQuestCompleted()}");

            if (!isCanvasVisible)
            {
                TryShowCanvas();
            }
            else
            {
                HideCanvas();
            }
        }

        // Check for close key
        if (Input.GetKeyDown(closeKey))
        {
            Debug.Log($"=== CLOSE KEY PRESSED ===");
            if (isCanvasVisible)
            {
                HideCanvas();
            }
        }
    }

    // ========================================================================
    // CANVAS MANAGEMENT
    // ========================================================================

    /// <summary>
    /// Attempts to show the reward canvas (checks quest completion)
    /// </summary>
    private void TryShowCanvas()
    {
        Debug.Log($"TryShowCanvas called");

        // Check if quest is completed
        if (IsQuestCompleted())
        {
            Debug.Log($"Quest IS completed, showing canvas");
            ShowCanvas();
        }
        else
        {
            Debug.Log($"Quest NOT completed, showing locked message");
            ShowLockedMessage();
        }
    }

    /// <summary>
    /// Shows the reward canvas (makes it visible)
    /// </summary>
    private void ShowCanvas()
    {
        Debug.Log($"ShowCanvas called");

        if (rewardCanvas == null || canvasGroup == null)
        {
            Debug.LogError("Canvas or CanvasGroup is NULL!");
            return;
        }

        Debug.Log($"Making canvas visible: {rewardCanvas.name}");
        isCanvasVisible = true;
        SetCanvasTransparency(visibleAlpha);

        Debug.Log($"Canvas alpha set to: {canvasGroup.alpha}");

        // Disable player movement if controller is assigned
        if (playerController != null)
        {
            playerController.enabled = false;
            Debug.Log($"Disabled player controller");
        }

        Debug.Log($"Showed reward canvas for quest: {requiredQuestID}");
    }

    /// <summary>
    /// Hides the reward canvas (makes it transparent)
    /// </summary>
    public void HideCanvas()
    {
        Debug.Log($"HideCanvas called");

        if (rewardCanvas == null || canvasGroup == null)
            return;

        isCanvasVisible = false;
        SetCanvasTransparency(transparentAlpha);

        // Re-enable player movement
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log($"Re-enabled player controller");
        }

        Debug.Log("Canvas hidden (transparent)");
    }

    /// <summary>
    /// Sets the canvas transparency level
    /// </summary>
    private void SetCanvasTransparency(float alpha)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = alpha;

        // Optionally disable interaction when transparent
        if (hideWhenTransparent)
        {
            canvasGroup.interactable = alpha > 0.5f;
            canvasGroup.blocksRaycasts = alpha > 0.5f;
        }
    }

    /// <summary>
    /// Shows a message that the canvas is locked
    /// </summary>
    private void ShowLockedMessage()
    {
        Debug.Log($"Quest not completed: {requiredQuestID}");

        // Show locked prompt if available
        if (lockedPrompt != null)
        {
            lockedPrompt.SetActive(true);
            lockedPromptTimer = lockedPromptDuration;

            // Update text if there's a TextMeshProUGUI component
            TextMeshProUGUI text = lockedPrompt.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = lockedMessage;
            }
        }
    }

    // ========================================================================
    // QUEST CHECKING
    // ========================================================================

    /// <summary>
    /// Checks if the required quest is completed
    /// </summary>
    private bool IsQuestCompleted()
    {
        if (string.IsNullOrEmpty(requiredQuestID))
        {
            Debug.LogWarning("Required Quest ID is empty!");
            return false;
        }

        if (QuestManager.Instance == null)
        {
            Debug.LogWarning("QuestManager not found!");
            return false;
        }

        bool isCompleted = QuestManager.Instance.IsQuestCompleted(requiredQuestID);
        Debug.Log($"Checking quest '{requiredQuestID}': {(isCompleted ? "COMPLETED" : "NOT COMPLETED")}");

        return isCompleted;
    }

    // ========================================================================
    // PUBLIC UTILITY METHODS
    // ========================================================================

    /// <summary>
    /// Manually show canvas (call from button)
    /// </summary>
    public void ShowCanvasButton()
    {
        TryShowCanvas();
    }

    /// <summary>
    /// Check if canvas can be shown
    /// </summary>
    public bool CanShowCanvas()
    {
        return IsQuestCompleted();
    }

    /// <summary>
    /// Get the required quest ID
    /// </summary>
    public string GetRequiredQuestID()
    {
        return requiredQuestID;
    }
}

// ============================================================================
// SETUP INSTRUCTIONS
// ============================================================================
/*
1. CREATE REWARD CANVAS:
   - Create new Canvas GameObject
   - Add your reward content (items, skills, etc.)
   - Make it INACTIVE by default
   
2. CREATE LOCKED PROMPT (OPTIONAL):
   - Create UI Panel in main scene
   - Add TextMeshProUGUI for message
   - Make it INACTIVE by default
   
3. ATTACH SCRIPT:
   - Add QuestRewardUI to empty GameObject or canvas
   
4. CONFIGURE INSPECTOR:
   - Required Quest ID: "apple_quest"
   - Reward Canvas: [Your reward canvas]
   - Open Key: J
   - Close Key: Escape
   - Locked Prompt: [Your locked message panel]
   - Player Controller: [Your player controller script]
   
5. TEST:
   - Before quest: Press J ¡ú Shows locked message
   - After quest: Press J ¡ú Opens reward canvas
   - Press ESC ¡ú Closes canvas

MULTIPLE CANVASES:
- Add multiple QuestRewardUI components
- Each with different quest IDs and canvases
- Example: Quest 1 unlocks Shop, Quest 2 unlocks Skills
*/