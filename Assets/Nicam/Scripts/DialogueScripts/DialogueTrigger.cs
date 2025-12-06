using UnityEngine;
using System.Collections;
using TMPro;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private NPCConversation npcConversation;

    [Header("Interaction Settings")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TextMeshProUGUI interactionPromptText;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;

    [Header("Prompt Messages")]
    [SerializeField] private string canTalkMessage = "Press E to Talk";
    [SerializeField] private string conditionsNotMetMessage = "Come back later...";
    [SerializeField] private string noDialogueMessage = "...";

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip triggerAudioClip;

    [Header("Player Control")]
    [SerializeField] private PlayerMove playerController;

    private bool isDialogueActive = false;
    private bool playerInRange = false;

    [System.Obsolete]
    void Start()
    {
        // Auto-find components if not assigned
        if (dialogueCanvas == null)
            dialogueCanvas = FindObjectOfType<Canvas>()?.gameObject;

        if (npcConversation == null)
            npcConversation = FindObjectOfType<NPCConversation>();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerMove>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (interactionPromptText == null && interactionPrompt != null)
        {
            interactionPromptText = interactionPrompt.GetComponentInChildren<TextMeshProUGUI>();
        }

        // Ensure dialogue canvas starts closed and disabled
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
            Debug.Log("Dialogue canvas initialized as inactive");
        }
        else
        {
            Debug.LogError("DialogueCanvas is null! Please assign it in the inspector.");
        }

        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Only check for input if player is in range and dialogue is not active
        if (playerInRange && !isDialogueActive)
        {
            UpdatePromptMessage();

            if (Input.GetKeyDown(interactionKey) && CanStartDialogue())
            {
                StartDialogue();
            }
        }
    }

    private bool CanStartDialogue()
    {
        if (npcConversation == null)
            return false;

        return npcConversation.HasAvailableDialogue();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            if (!isDialogueActive)
            {
                UpdatePromptMessage();

                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(true);
                    Debug.Log("Player entered range");
                }
            }
        }
    }

    private void UpdatePromptMessage()
    {
        if (interactionPromptText == null || npcConversation == null)
            return;

        if (!npcConversation.HasAnyDialogue())
        {
            interactionPromptText.text = noDialogueMessage;
        }
        else if (npcConversation.HasAvailableDialogue())
        {
            interactionPromptText.text = canTalkMessage;
        }
        else
        {
            interactionPromptText.text = conditionsNotMetMessage;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }

            Debug.Log("Player left range");
        }
    }

    private void StartDialogue()
    {
        if (isDialogueActive)
        {
            Debug.LogWarning("Dialogue already active, ignoring trigger");
            return;
        }

        Debug.Log("Starting dialogue");
        isDialogueActive = true;

        // Hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }

        // Unlock and show cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Debug.Log("Cursor unlocked");

        // Disable player movement
        if (playerController != null)
        {
            playerController.enabled = false;
        }
        else
        {
            Debug.LogWarning("PlayerController is null in StartDialogue!");
        }

        // Open dialogue canvas
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(true);
            Debug.Log("Dialogue canvas opened");
        }
        else
        {
            Debug.LogWarning("DialogueCanvas is null in StartDialogue!");
        }

        // Play audio if available
        if (audioSource != null && triggerAudioClip != null)
        {
            audioSource.PlayOneShot(triggerAudioClip);
        }

        // Start the conversation
        if (npcConversation != null)
        {
            npcConversation.StartConversation();
        }
        else
        {
            Debug.LogWarning("NPCConversation is null in StartDialogue!");
        }
    }

    public void EndDialogue()
    {
        Debug.Log("EndDialogue called");
        StartCoroutine(EndDialogueCoroutine());
    }

    private IEnumerator EndDialogueCoroutine()
    {
        isDialogueActive = false;

        // Close dialogue canvas
        if (dialogueCanvas != null)
        {
            dialogueCanvas.SetActive(false);
            Debug.Log("Dialogue canvas closed");
        }
        else
        {
            Debug.LogWarning("DialogueCanvas is null!");
        }

        // Lock and hide cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Cursor locked");

        // Wait a frame to ensure canvas is closed
        yield return null;

        // Re-enable player movement
        if (playerController != null)
        {
            playerController.enabled = true;
            Debug.Log("Player movement re-enabled");
        }
        else
        {
            Debug.LogWarning("PlayerController is null!");
        }

        // Check if player is still in range
        Collider[] colliders = Physics.OverlapSphere(transform.position, GetComponent<Collider>().bounds.extents.magnitude);
        bool playerStillInRange = false;

        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Player"))
            {
                playerStillInRange = true;
                break;
            }
        }

        if (playerStillInRange)
        {
            UpdatePromptMessage();

            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(true);
                Debug.Log("Player still in range - showing prompt");
            }
        }

        Debug.Log($"Dialogue system ready. Player in range: {playerStillInRange}");
    }

    public bool IsDialogueActive => isDialogueActive;
}