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
    public float jumpHoldCutoff = 0.5f;
    public float jumpCooldown = 1.5f;

    [Header("Ground Check")]
    public LayerMask groundLayer;
    public Transform[] groundCheckPoints;
    public float groundCheckDistance = 0.2f;

    [Header("Boost Settings")]
    public float boostDuration = 0.5f;
    public float boostCooldown = 2f;

    [Header("Death Settings")]
    public float deathDistance = 20f;
    private float lastGroundedY;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isBoosting = false;
    private bool canBoost = true;
    private bool jumpRequested = false;
    private int jumpsLeft;
    private bool isJumping = false;
    private bool isJumpOnCooldown = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpsLeft = maxJumps;
        lastGroundedY = transform.position.y;
    }

    void Update()
    {
        HandleKeyboardInput();
        HandlePointerInput();
    }

    void FixedUpdate()
    {
        if (transform.position.y < lastGroundedY - deathDistance)
        {
            GameManager.instance.GameOver();
            return;
        }

        CheckGrounded();

        if (jumpRequested)
        {
            Jump();
            jumpRequested = false;
        }

        if (isJumping && rb.linearVelocity.y < 0)
        {
            isJumping = false;
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
                isJumping = false;
                isGrounded = true;
                lastGroundedY = transform.position.y;
                return;
            }
        }
        isGrounded = false;
    }

    void HandleKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RequestJump();
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            EndJump();
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Boost();
        }
    }

    void HandlePointerInput()
    {
#if UNITY_EDITOR || UNITY_IOS || UNITY_ANDROID
        foreach (Touch touch in Input.touches)
        {
            if (touch.position.x < Screen.width / 2)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    RequestJump();
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    EndJump();
                }
            }
            else
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Boost();
                }
            }
        }
#endif

#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width / 2) { RequestJump(); }
            else if (Input.mousePosition.x >= Screen.width / 2) { Boost(); }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (Input.mousePosition.x < Screen.width / 2) { EndJump(); }
        }
#endif
    }

    private void RequestJump()
    {
        if (jumpsLeft > 0 && !isJumpOnCooldown)
        {
            jumpRequested = true;
        }
    }

    private void EndJump()
    {
        if (isJumping && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * jumpHoldCutoff, rb.linearVelocity.z);
        }
        isJumping = false;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        jumpsLeft--;
        isJumping = true;

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

    void Boost()
    {
        if (canBoost)
        {
            StartCoroutine(BoostCoroutine());
        }
    }

    IEnumerator BoostCoroutine()
    {
        canBoost = false;
        isBoosting = true;
        yield return new WaitForSeconds(boostDuration);
        isBoosting = false;
        yield return new WaitForSeconds(boostCooldown);
        canBoost = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            ContactPoint contact = collision.contacts[0];
            float yNormal = contact.normal.y;

            if (yNormal < 0.5f)
            {
                GameManager.instance.GameOver();
                rb.isKinematic = true;
            }
        }
    }
}