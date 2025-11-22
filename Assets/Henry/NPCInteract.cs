using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteract : MonoBehaviour
{
    public GameObject PressE;
    public GameObject Message;

    public bool inTrigger;

    private void Start()
    {
        inTrigger = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PressE.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        inTrigger = false;
        PressE.SetActive(false);
        Message.SetActive(false);
    }

    private void OnTriggerStay(Collider other)
    {
        inTrigger = true;
    }

    public void OnPressE(InputAction.CallbackContext context)
    {
        if(inTrigger == true)
        {
            Message.SetActive(true);
            PressE.SetActive(false);
        }
    }
}
