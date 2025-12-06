using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using static XNode.Node;

public class EntryNode : CoreNodeBase {

    [Output(connectionType = ConnectionType.Override)] public bool exit;

}