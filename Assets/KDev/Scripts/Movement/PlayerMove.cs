using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed, sensitivity, maxForce;
    public float jumpForce;
    private float lookRot;
    private Vector2 move, look;
    private Rigidbody rb;
    private Camera cam;
    private bool isGrounded = false;
    private bool hasJumped = false; // Track if player has jumped

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        // Only jump if grounded AND hasn't jumped yet
        if (context.performed && isGrounded && !hasJumped)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            hasJumped = true; // Lock jumping until touching ground
            isGrounded = false; // Immediately set not grounded
            Debug.Log("Jumped!");
        }
    }

    public void SetGrounded(bool grounded)
    {
        isGrounded = grounded;

        // Reset jump lock when touching ground
        if (grounded)
        {
            hasJumped = false;
            Debug.Log("Can jump again");
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cam = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Vector3 currVelocity = rb.linearVelocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y) * speed;
        targetVelocity = transform.TransformDirection(targetVelocity);
        Vector3 VelocityChange = targetVelocity - currVelocity;
        VelocityChange = Vector3.ClampMagnitude(VelocityChange, maxForce);
        rb.AddForce(new Vector3(VelocityChange.x, 0, VelocityChange.z), ForceMode.VelocityChange);
    }

    private void LateUpdate()
    {
        transform.Rotate(Vector3.up * look.x * sensitivity);
        lookRot += (-look.y * sensitivity);
        lookRot = Mathf.Clamp(lookRot, -90, 90);
        cam.transform.eulerAngles = new Vector3(lookRot, cam.transform.eulerAngles.y, cam.transform.eulerAngles.z);
    }
}