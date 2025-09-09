using UnityEngine;

public class CarController : MonoBehaviour
{
    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float jumpForce = 8f;
    // We'll add a boost speed for later
    public float boostSpeed = 25f;

    private Rigidbody rb;
    private bool isGrounded;

    // We'll use this to track if we are currently boosting
    private bool isBoosting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // --- Core Movement ---
        float currentSpeed = isBoosting ? boostSpeed : forwardSpeed;
        transform.position += Vector3.forward * currentSpeed * Time.deltaTime;

        // --- Input Handling ---
        HandleKeyboardInput();

        // Call the new, combined input function
        HandlePointerInput();
    }

    void HandleKeyboardInput()
    {
        // Check for jump input
        if (isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // Use Left Shift for boost testing in the editor
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Boost();
        }
    }

    void HandlePointerInput()
    {
        // This code will only run when on a mobile device (iOS or Android)
#if UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                // Check if the touch is on the left side of the screen
                if (touch.position.x < Screen.width / 2)
                {
                    if (isGrounded) Jump();
                }
                // Check if the touch is on the right side of the screen
                else if (touch.position.x >= Screen.width / 2)
                {
                    Boost();
                }
            }
        }
#endif

        // This code will only run when in the Unity Editor
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0)) // 0 is the left mouse button
        {
            // Check if the mouse click is on the left side of the screen
            if (Input.mousePosition.x < Screen.width / 2)
            {
                if (isGrounded) Jump();
            }
            // Check if the mouse click is on the right side of the screen
            else if (Input.mousePosition.x >= Screen.width / 2)
            {
                Boost();
            }
        }
#endif
    }

    void Jump()
    {
        // The jump logic is now in its own function
        isGrounded = false;
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        Debug.Log("Jump!");
    }

    void Boost()
    {
        // For now, our boost will just be a debug message.
        // We will add the actual logic (like a speed burst and effects) in a later step.
        Debug.Log("Boost!");
        // We'll add a simple timed boost later. For now, we can just log it.
        // isBoosting = true; 
        // Invoke("StopBoosting", 0.5f); // Example of how we might stop it
    }

    /*
    void StopBoosting()
    {
        isBoosting = false;
    }
    */

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}