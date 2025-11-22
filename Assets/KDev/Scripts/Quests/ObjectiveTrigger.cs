using NUnit.Framework.Constraints;
using UnityEngine;

public class ObjectiveTrigger : MonoBehaviour
{
    private Objective ButtonObjective;

    private void OnTriggerEnter(Collider other)
    {
        //blankObjective.AddProgress(1);
        //GameManager.Instance.AddProgress("collectApples", 1);
    }

    public void ButtonQuestInit()
    {
        ButtonObjective = new Objective("ButtonPress", "You have pressed {0} / {1} buttons", 5);
        GameManager.Instance.AddObjective(ButtonObjective);
    }

    public void ButtonQuestProgress()
    {
        ButtonObjective?.AddProgress(1);
    }
}
