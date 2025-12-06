using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XNode;
using UnityEngine.UI;

public class NPCConversation : MonoBehaviour
{
    [Header("Dialogue Management")]
    [SerializeField] private List<DialogueGraph> dialogueList = new List<DialogueGraph>();
    private Dictionary<string, DialogueGraph> dialogueDictionary = new Dictionary<string, DialogueGraph>();
    private DialogueGraph currentDialogue;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI spokenLine;
    [SerializeField] private Transform responseButtonPanel;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Quest Settings")]
    [SerializeField] private string npcID;

    private DialogueTrigger dialogueTrigger;

    [Obsolete]
    void Start()
    {
        dialogueTrigger = GetComponent<DialogueTrigger>();
        if (dialogueTrigger == null)
            dialogueTrigger = FindObjectOfType<DialogueTrigger>();

        InitializeDialogues();
    }

    private void InitializeDialogues()
    {
        dialogueDictionary.Clear();

        for (int i = 0; i < dialogueList.Count; i++)
        {
            if (dialogueList[i] != null)
            {
                string key = $"dialogue_{i}";
                dialogueDictionary.Add(key, dialogueList[i]);
            }
        }

        Debug.Log($"Initialized {dialogueDictionary.Count} dialogues for {npcID}");
    }

    public void StartConversation()
    {
        // ADDED: Re-enable UI GameObjects at the start of conversation
        if (spokenLine != null)
        {
            spokenLine.gameObject.SetActive(true);
            spokenLine.text = "";
        }

        if (speakerNameText != null)
        {
            speakerNameText.gameObject.SetActive(true);
            speakerNameText.text = "";
        }

        if (responseButtonPanel != null)
        {
            responseButtonPanel.gameObject.SetActive(true);
        }

        ClearButtons();

        currentDialogue = GetNextAvailableDialogue();

        if (currentDialogue == null)
        {
            Debug.Log("No more dialogues available for this NPC");
            EndConversation();
            return;
        }

        Debug.Log($"Starting dialogue. Remaining dialogues: {dialogueDictionary.Count}");

        foreach (Node item in currentDialogue.nodes)
        {
            if (item is EntryNode)
            {
                currentDialogue.current = item.GetPort("exit").Connection.node as DialogueNodeBase;
                break;
            }
        }
        ParseNode();
    }

    public bool HasAnyDialogue()
    {
        return dialogueDictionary.Count > 0;
    }

    public bool HasAvailableDialogue()
    {
        if (dialogueDictionary.Count == 0)
            return false;

        foreach (var kvp in dialogueDictionary)
        {
            DialogueGraph graph = kvp.Value;

            foreach (Node item in graph.nodes)
            {
                if (item is EntryNode)
                {
                    NodePort exitPort = item.GetPort("exit");
                    if (exitPort != null && exitPort.Connection != null)
                    {
                        DialogueNodeBase firstNode = exitPort.Connection.node as DialogueNodeBase;

                        if (firstNode is NPCDialogue)
                        {
                            NPCDialogue npcNode = firstNode as NPCDialogue;
                            if (npcNode.CanShowDialogue())
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    break;
                }
            }
        }

        return false;
    }

    private DialogueGraph GetNextAvailableDialogue()
    {
        if (dialogueDictionary.Count > 0)
        {
            foreach (var kvp in dialogueDictionary)
            {
                Debug.Log($"Getting dialogue: {kvp.Key}");
                return kvp.Value;
            }
        }

        Debug.Log("No more dialogues in dictionary");
        return null;
    }

    private void RemoveCurrentDialogue()
    {
        if (currentDialogue == null)
            return;

        string keyToRemove = null;
        foreach (var kvp in dialogueDictionary)
        {
            if (kvp.Value == currentDialogue)
            {
                keyToRemove = kvp.Key;
                break;
            }
        }

        if (keyToRemove != null)
        {
            dialogueDictionary.Remove(keyToRemove);
            Debug.Log($"Removed dialogue: {keyToRemove}. Remaining dialogues: {dialogueDictionary.Count}");
        }
    }

    private void ParseNode()
    {
        if (currentDialogue == null || currentDialogue.current == null || spokenLine == null)
        {
            EndConversation();
            return;
        }

        switch (currentDialogue.current.GetDialogueType)
        {
            case "NPC":
                NPCDialogue npcNode = currentDialogue.current as NPCDialogue;

                if (npcNode != null && !npcNode.CanShowDialogue())
                {
                    NextNode("exit");
                    return;
                }

                if (npcNode != null && speakerNameText != null)
                {
                    speakerNameText.text = npcNode.speakerName;
                }

                spokenLine.text = currentDialogue.current.dialogueSpoken;

                if (npcNode != null && npcNode.startsQuest && npcNode.questToStart != null)
                {
                    if (QuestManager.Instance != null)
                    {
                        QuestManager.Instance.StartQuest(npcNode.questToStart);
                    }
                }

                if (npcNode != null && npcNode.completesQuest && !string.IsNullOrEmpty(npcNode.questToCompleteID))
                {
                    if (QuestManager.Instance != null)
                    {
                        QuestManager.Instance.ForceCompleteQuest(npcNode.questToCompleteID);
                        Debug.Log($"Quest completed via dialogue: {npcNode.questToCompleteID}");
                    }
                }

                bool hasResponses = HasResponseConnections();

                if (hasResponses)
                {
                    SpawnResponseButtons();
                }
                else
                {
                    CreateContinueButton();
                }
                break;

            case "Response":
                ClearButtons();
                NextNode("exit");

                if (!string.IsNullOrEmpty(npcID) && QuestManager.Instance != null)
                {
                    UpdateTalkQuestProgress();
                }
                break;

            case "End":
                RemoveCurrentDialogue();

                if (!string.IsNullOrEmpty(currentDialogue.current.dialogueSpoken))
                {
                    spokenLine.text = currentDialogue.current.dialogueSpoken;
                    CreateCloseButton();
                }
                else
                {
                    EndConversation();
                }
                break;

            default:
                Debug.LogWarning($"Unknown dialogue node type: {currentDialogue.current.GetDialogueType}");
                EndConversation();
                break;
        }
    }

    private bool HasResponseConnections()
    {
        foreach (NodePort port in currentDialogue.current.Ports)
        {
            if (port.Connection != null && !port.IsInput && port.Connection.node is ResponseDialogue)
            {
                return true;
            }
        }
        return false;
    }

    private void UpdateTalkQuestProgress()
    {
        // Quest progress tracking placeholder
    }

    public void NextNode(string fieldName)
    {
        if (currentDialogue == null || currentDialogue.current == null)
        {
            EndConversation();
            return;
        }

        foreach (NodePort port in currentDialogue.current.Ports)
        {
            if (port.fieldName == fieldName)
            {
                if (port.Connection != null)
                {
                    currentDialogue.current = port.Connection.node as DialogueNodeBase;
                    ParseNode();
                    return;
                }
                else
                {
                    EndConversation();
                    return;
                }
            }
        }

        Debug.LogWarning($"No port found with fieldName: {fieldName}");
        EndConversation();
    }

    private void SpawnResponseButtons()
    {
        foreach (NodePort port in currentDialogue.current.Ports)
        {
            if (port.Connection == null || port.IsInput)
            {
                continue;
            }

            if (port.Connection.node is ResponseDialogue)
            {
                ResponseDialogue rd = port.Connection.node as ResponseDialogue;

                if (!rd.CanShowResponse())
                {
                    continue;
                }

                Button b = Instantiate(buttonPrefab, responseButtonPanel).GetComponent<Button>();
                b.onClick.AddListener(() => NextNode(port.fieldName));
                b.GetComponentInChildren<TextMeshProUGUI>().text = rd.dialogueSpoken.ToString();
            }
        }

        if (responseButtonPanel.childCount == 0)
        {
            CreateContinueButton();
        }
    }

    private void ClearButtons()
    {
        Transform[] children = responseButtonPanel.GetComponentsInChildren<Transform>();
        for (int i = children.Length - 1; i >= 0; i--)
        {
            if (children[i] != responseButtonPanel)
            {
                Destroy(children[i].gameObject);
            }
        }
    }

    private void CreateCloseButton()
    {
        if (buttonPrefab == null || responseButtonPanel == null)
        {
            EndConversation();
            return;
        }

        Button closeButton = Instantiate(buttonPrefab, responseButtonPanel).GetComponent<Button>();
        closeButton.onClick.AddListener(() => EndConversation());
        closeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Close";
    }

    private void CreateContinueButton()
    {
        if (buttonPrefab == null || responseButtonPanel == null)
        {
            EndConversation();
            return;
        }

        Button continueButton = Instantiate(buttonPrefab, responseButtonPanel).GetComponent<Button>();
        continueButton.onClick.AddListener(() => {
            ClearButtons();
            NextNode("exit");
        });
        continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Continue";
    }

    private void EndConversation()
    {
        ClearButtons();

        // Clear all text fields
        if (spokenLine != null)
            spokenLine.text = "";

        if (speakerNameText != null)
            speakerNameText.text = "";

        // Reset current dialogue
        if (currentDialogue != null)
            currentDialogue.current = null;

        if (dialogueTrigger != null)
        {
            dialogueTrigger.EndDialogue();
        }
        else
        {
            Debug.LogWarning("DialogueTrigger not found! Dialogue may not end properly.");
        }
    }
}