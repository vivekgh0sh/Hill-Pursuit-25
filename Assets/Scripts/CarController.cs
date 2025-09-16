using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float boostSpeed = 25f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public int maxJumps = 2;
    [Range(0.1f, 1f)]
    public float jumpHoldCutoff = 0.5f; // Multiplier to reduce upward velocity when jump is released early
    public float jumpCooldown = 1.5f; // Cooldown after the second jump

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform[] groundCheckPoints;
    public float groundCheckDistance = 0.2f;

    [Header("Boost Settings")]
    public float boostDuration = 0.5f;
    public float boostCooldown = 2f;

    [Header("Death Settings")]
    public float deathYLevel = -20f;

    // Private state variables
    private Rigidbody rb;
    private bool isGrounded;
    private bool isBoosting = false;
    private bool canBoost = true;
    private bool jumpRequested = false;
    private int jumpsLeft;
    private bool isJumping = false; // Are we currently in the upward motion of a jump?
    private bool isJumpOnCooldown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        // All input is read in Update() for maximum responsiveness
        HandleKeyboardInput();
        HandlePointerInput();
    }

    void FixedUpdate()
    {

        if (transform.position.y < deathYLevel)
        {
            RestartLevel();
        }

        CheckGrounded();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        // If the car has reached the apex of its jump and starts falling, it's no longer "actively jumping"
        if (isJumping && rb.linearVelocity.y < 0)
        {
            isJumping = false;
        }

        // Forward movement logic
        float currentSpeed = isBoosting ? boostSpeed : forwardSpeed;
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void RestartLevel()
    {
        Debug.Log("Player has fallen!");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CheckGrounded()
    {
        foreach (Transform point in groundCheckPoints)
        {
            if (Physics.Raycast(point.position, Vector3.down, groundCheckDistance, groundLayer))
            {
                // If we are grounded, reset jump counter, end the "isJumping" state, and set grounded to true
                if (!isGrounded) // Only reset if we just landed
                {
                    jumpsLeft = maxJumps;
                }
                isJumping = false;
                isGrounded = true;
                return; // Exit early since we know we are grounded
            }
        }
        // If we loop through all points and none hit, we are in the air
        isGrounded = false;
    }

    // --- Input Handling ---

    void HandleKeyboardInput()
    {
        // On key press
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestJump();
        }
        // On key release
        if (Input.GetKeyUp(KeyCode.Space))
        {
            EndJump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift)) { Boost(); }
    }

    void HandlePointerInput()
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        // Loop through all touches to handle multi-touch scenarios gracefully
        foreach (Touch touch in Input.touches)
        {
            // Check if the touch is on the left side of the screen
            if (touch.position.x < Screen.width / 2)
            {
                if (touch.phase == TouchPhase.Began) // On press
                {
                    RequestJump();
                }
                if (touch.phase == TouchPhase.Ended) // On release
                {
                    EndJump();
                }
            }
            // Check if the touch is on the right side
            else
            {
                if (touch.phase == TouchPhase.Began) // On press
                {
                    Boost();
                }
            }
        }
#endif

#if UNITY_EDITOR
        // Also handle mouse input for easy testing in the editor
        if (Input.GetMouseButtonDown(0)) // On press
        {
            if (Input.mousePosition.x < Screen.width / 2) { RequestJump(); }
            else if (Input.mousePosition.x >= Screen.width / 2) { Boost(); }
        }
        if (Input.GetMouseButtonUp(0)) // On release
        {
            if (Input.mousePosition.x < Screen.width / 2) { EndJump(); }
        }
#endif
    }

    // --- Jump Logic ---

    private void RequestJump()
    {
        // We can only request a jump if we have jumps left AND the ability isn't on cooldown
        if (jumpsLeft > 0 && !isJumpOnCooldown)
        {
            jumpRequested = true;
        }
    }

    private void EndJump()
    {
        // If we release the button while moving upwards, cut the jump short
        if (isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * jumpHoldCutoff, rb.linearVelocity.z);
        }
        isJumping = false; // We are no longer actively holding the jump
    }

    void Jump()
    {
        // Reset vertical velocity to ensure consistent jump height, especially for the second jump
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpsLeft--;
        isJumping = true; // We have started the upward motion of a jump

        // If that was our last jump, start the cooldown
        if (jumpsLeft == 0)
        {
            StartCoroutine(JumpCooldownCoroutine());
        }
    }

    IEnumerator JumpCooldownCoroutine()
    {
        isJumpOnCooldown = true;
        yield return new WaitForSeconds(jumpCooldown);
        isJumpOnCooldown = false;
    }

    // --- Other Mechanics ---

    void Boost() { if (canBoost) StartCoroutine(BoostCoroutine()); }

    IEnumerator BoostCoroutine()
    {
        canBoost = false; isBoosting = true;
        yield return new WaitForSeconds(boostDuration);
        isBoosting = false;
        yield return new WaitForSeconds(boostCooldown);
        canBoost = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone")) { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    }
}