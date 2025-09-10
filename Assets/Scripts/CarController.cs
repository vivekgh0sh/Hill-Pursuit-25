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
    private int jumpsLeft;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform[] groundCheckPoints;
    public float groundCheckDistance = 0.2f;

    [Header("Boost Settings")]
    public float boostDuration = 0.5f;
    public float boostCooldown = 2f;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isBoosting = false;
    private bool canBoost = true;
    private bool jumpRequested = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpsLeft = maxJumps;
    }

    void Update()
    {
        HandleKeyboardInput();
        HandlePointerInput();
    }

    void FixedUpdate()
    {
        CheckGrounded();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        float currentSpeed = isBoosting ? boostSpeed : forwardSpeed;
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void CheckGrounded()
    {
        foreach (Transform point in groundCheckPoints)
        {
            if (Physics.Raycast(point.position, Vector3.down, groundCheckDistance, groundLayer))
            {
                if (!isGrounded)
                {
                    jumpsLeft = maxJumps;
                }
                isGrounded = true;
                return;
            }
        }
        isGrounded = false;
    }

    void HandleKeyboardInput()
    {
        if (jumpsLeft > 0 && Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Boost();
        }
    }

    void HandlePointerInput()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width / 2) { if (jumpsLeft > 0) jumpRequested = true; }
            else if (Input.mousePosition.x >= Screen.width / 2) { Boost(); }
        }
#endif
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpsLeft--;
    }

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