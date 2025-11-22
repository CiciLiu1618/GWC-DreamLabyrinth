using UnityEngine;
using TMPro;
using System.Collections;

public class RespawnTrigger : MonoBehaviour
{
    [Header("Respawn Settings")]
    [Tooltip("The point where the player will respawn")]
    public Transform respawnPoint;

    [Tooltip("Tag of the player object (default: 'Player')")]
    public string playerTag = "Player";

    [Tooltip("Delay before respawning (in seconds)")]
    public float respawnDelay = 2f;

    [Header("UI Settings")]
    [Tooltip("The TextMeshPro component to display the message")]
    public TextMeshProUGUI messageText;

    [Tooltip("Message to display")]
    public string popupMessage = "You got lost in the fog...";

    [Tooltip("How long to display the message (in seconds)")]
    public float messageDuration = 3f;

    private bool isRespawning = false;

    private void Start()
    {
        // Hide the popup at start
        if (messageText != null)
        {
            messageText.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object that entered is the player
        if (other.CompareTag(playerTag) && !isRespawning)
        {
            // Start the respawn sequence
            StartCoroutine(RespawnSequence(other.gameObject));
        }
    }

    private IEnumerator RespawnSequence(GameObject player)
    {
        isRespawning = true;

        // Show popup message
        if (messageText != null)
        {
            messageText.text = popupMessage;
            messageText.enabled = true;
        }

        // Wait for delay
        yield return new WaitForSeconds(respawnDelay);

        // Respawn the player
        RespawnPlayer(player);

        // Keep message visible for remaining duration
        float remainingTime = messageDuration - respawnDelay;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // Hide message
        if (messageText != null)
        {
            messageText.enabled = false;
        }

        isRespawning = false;
    }

    private void RespawnPlayer(GameObject player)
    {
        if (respawnPoint != null)
        {
            // Move player to respawn point
            player.transform.position = respawnPoint.position;
            player.transform.rotation = respawnPoint.rotation;

            // Reset velocity if player has a Rigidbody
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogWarning("Respawn point not set in RespawnTrigger!");
        }
    }
}