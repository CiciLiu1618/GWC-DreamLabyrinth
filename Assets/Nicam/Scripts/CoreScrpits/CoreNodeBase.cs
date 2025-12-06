using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

public class CoreNodeBase : DialogueNodeBase
{
    [SerializeField] protected string nodeType = "Core";

    public override string GetDialogueType => nodeType;
}