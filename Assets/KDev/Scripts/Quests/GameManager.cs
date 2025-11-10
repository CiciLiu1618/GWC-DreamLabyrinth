using UnityEngine;
using System;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        } else if (Instance != this) {
            Destroy(this);
        }
    }
    #region ObjectiveStuffMeThinks
    public Action<Objective> OnObjectiveAdded;

    public List<Objective> Objectives { get; } = new List<Objective>();
    private readonly Dictionary<string, List<Objective>> _objectiveMap = new();

    public void AddObjective(Objective objective)
    {
        Objectives.Add(objective);
        if(!string.IsNullOrEmpty(objective.EventTrigger))
        {
            if(!_objectiveMap.ContainsKey(objective.EventTrigger)) {
                _objectiveMap.Add(objective.EventTrigger, new List<Objective>());
            }
            _objectiveMap[objective.EventTrigger].Add(objective);
        }

        OnObjectiveAdded?.Invoke(objective);
        Debug.Log("OBJECTIVE ADDED RAHHH");
    }

    public void AddProgress(string eventTrigger, int value)
    {
        if (!_objectiveMap.ContainsKey(eventTrigger)) return;

        foreach (var objective in _objectiveMap[eventTrigger])
        {
            objective.AddProgress(value);
        }
        Debug.Log("we added the progress :3");
    }
    #endregion
}
