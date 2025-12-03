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
        PressE.SetActive(false);
        Message.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = true;
            PressE.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = false;
            PressE.SetActive(false);
            Message.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inTrigger = true;
        }
    }

    public void OnPressE(InputAction.CallbackContext context)
    {
        if (context.performed && inTrigger)
        {
            Message.SetActive(true);
            PressE.SetActive(false);
        }
    }
}
