using System;
using UnityEngine;

public class Objective
{
    public Action OnComplete;
    public Action OnValueChange;

    public string EventTrigger { get; }
    public bool IsComplete {  get; private set; }
    public int MaxValue { get; }
    public int CurrentValue { get; private set; }

    private readonly string _statusText;

    public Objective(string eventTrigger, string statusText, int maxValue)
    {
        EventTrigger = eventTrigger;
        _statusText = statusText;
        MaxValue = maxValue;
    }

    public void CheckCompletion()
    {
        if (CurrentValue >= MaxValue)
        {
            IsComplete = true;
            OnComplete?.Invoke();
        }
    }

    public void AddProgress(int value)
    {
        if (IsComplete) return;

        CurrentValue += value;
        if (CurrentValue > MaxValue)
        {
            CurrentValue = MaxValue;
        }
        OnValueChange?.Invoke();
        CheckCompletion();
    }

    public string GetStatusText()
    {
        return string.Format(_statusText, CurrentValue, MaxValue);
    }
}
