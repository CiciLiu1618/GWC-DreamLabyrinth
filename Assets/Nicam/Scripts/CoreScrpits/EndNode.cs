using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using static XNode.Node;

public class EndNode : CoreNodeBase {

    [Input] public string entry;

    public override string GetDialogueType => "End";


}