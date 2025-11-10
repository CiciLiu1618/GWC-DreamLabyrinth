using System.Collections.Generic;
using UnityEngine;

public class ObjectivePanel : MonoBehaviour
{
    [SerializeField] private ObjectiveDisplay _ObjDisplayPrefab;

    [SerializeField] private Transform _ObjDisplayParent;
    private readonly List<ObjectiveDisplay> _ListDisplay = new();

    private void Start()
    {
        foreach(Objective objective in GameManager.Instance.Objectives)
        {
            AddObjective(objective);
        }
        GameManager.Instance.OnObjectiveAdded += AddObjective;
    }

    private void AddObjective(Objective obj)
    {
        ObjectiveDisplay display = Instantiate(_ObjDisplayPrefab, _ObjDisplayParent);
        display.Init(obj);
        _ListDisplay.Add(display);
        Debug.Log("added to display yippee");
    }

    public void LeReset()
    {
        for (int i = _ListDisplay.Count - 1; i >= 0; i--)
        {
            Destroy(_ListDisplay[i].gameObject);
        }
        _ListDisplay.Clear();
    }
}
