using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private PlayerMove playerMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMove = GetComponentInParent<PlayerMove>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playerMove.gameObject)
        {
            return;
        }
        Debug.Log("entered");
        playerMove.SetGrounded(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playerMove.gameObject)
        {
            return;
        }
        Debug.Log("exited");
        playerMove.SetGrounded(false);
    }
}
