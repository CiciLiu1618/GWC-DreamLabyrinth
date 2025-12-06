using UnityEngine;

public class GroundTrigger : MonoBehaviour
{
    private PlayerMove playerMove;
    private int groundContacts = 0;
    public LayerMask groundLayer; // Assign in Inspector

    void Start()
    {
        playerMove = GetComponentInParent<PlayerMove>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider is on the ground layer
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            groundContacts++;
            Debug.Log("Ground entered - contacts: " + groundContacts);
            playerMove.SetGrounded(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Check if the collider is on the ground layer
        if (((1 << other.gameObject.layer) & groundLayer) != 0)
        {
            groundContacts--;
            Debug.Log("Ground exited - contacts: " + groundContacts);

            if (groundContacts <= 0)
            {
                groundContacts = 0;
                playerMove.SetGrounded(false);
            }
        }
    }
}