using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public enum PlayerState { Grounded, Jumping, Peaking, Falling }

public class CarController : MonoBehaviour
{
    public PlayerState CurrentState { get; private set; }

    [Header("Movement")]
    public float forwardSpeed = 10f;
    public float boostSpeed = 25f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public int maxJumps = 2;
    [Range(0.1f, 1f)]
    public float jumpHoldCutoff = 0.5f;
    public float jumpCooldown = 1.5f;
    public float peakVelocityThreshold = 0.5f;

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

    [Header("Effects")]
    public GameObject jumpParticlesPrefab;

    private Rigidbody rb;
    private bool isGrounded;
    private bool isBoosting = false;
    private bool canBoost = true;
    private bool jumpRequested = false;
    private int jumpsLeft;
    private bool isJumping = false;
    private bool isJumpOnCooldown = false;
    private float lastBoostTime = -100f; 

    void Start() 
    {
        rb = GetComponent<Rigidbody>();
        jumpsLeft = maxJumps;
        lastGroundedY = transform.position.y;
    }
    void FixedUpdate()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;
        if (transform.position.y < lastGroundedY - deathDistance) { GameManager.Instance.EndGame(); return; }
        CheckGrounded();
        UpdatePlayerState();
        if (jumpRequested) { Jump(); jumpRequested = false; }
        float currentSpeed = isBoosting ? boostSpeed : forwardSpeed;
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
    private void UpdatePlayerState() { if (isGrounded) { CurrentState = PlayerState.Grounded; } else { float yVelocity = rb.linearVelocity.y; if (yVelocity > peakVelocityThreshold) { CurrentState = PlayerState.Jumping; } else if (yVelocity < -peakVelocityThreshold) { CurrentState = PlayerState.Falling; } else { CurrentState = PlayerState.Peaking; } } }
    private void CheckGrounded() { bool wasGrounded = isGrounded; isGrounded = false; foreach (Transform point in groundCheckPoints) { if (Physics.Raycast(point.position, Vector3.down, groundCheckDistance, groundLayer)) { isGrounded = true; if (!wasGrounded) { jumpsLeft = maxJumps; } lastGroundedY = transform.position.y; return; } } }
    public bool IsGrounded() { return isGrounded; }
    public bool IsBoosting() { return isBoosting; }

    public void UIR_RequestJump() { RequestJump(); }
    public void UIR_RequestBoost() { Boost(); }
    public int GetJumpsLeft() { return jumpsLeft; }
    public float GetBoostCooldownProgress()
    {
        if (canBoost) return 1f;
        float timeSinceBoost = Time.time - lastBoostTime;
        return Mathf.Clamp01(timeSinceBoost / (boostDuration + boostCooldown));
    }

    private void RequestJump() { if (jumpsLeft > 0 && !isJumpOnCooldown) { jumpRequested = true; } }
    private void EndJump() { if (isJumping && rb.linearVelocity.y > 0) { rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * jumpHoldCutoff, rb.linearVelocity.z); } isJumping = false; }
    void Jump()
    {
        AudioManager.Instance.PlaySFX("Jump");
        HapticFeedback.Vibrate(40, 100);
        if (jumpParticlesPrefab != null) 
        {
            foreach (Transform point in groundCheckPoints)
            {
                ObjectPooler.Instance.SpawnFromPool("JumpFX", point.position, Quaternion.identity);
            }
        }
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); 
        jumpsLeft--;
        isJumping = true; 
        if (jumpsLeft == 0)
        {
            StartCoroutine(JumpCooldownCoroutine());
        }
    }
    IEnumerator JumpCooldownCoroutine() { isJumpOnCooldown = true; yield return new WaitForSeconds(jumpCooldown); isJumpOnCooldown = false; }
    void Boost() { if (canBoost) { StartCoroutine(BoostCoroutine()); } }
    IEnumerator BoostCoroutine() 
    {
        AudioManager.Instance.PlaySFX("Boost");
        canBoost = false; 
        isBoosting = true; 
        lastBoostTime = Time.time;
        yield return new WaitForSeconds(boostDuration); 
        isBoosting = false;
        yield return new WaitForSeconds(boostCooldown);
        canBoost = true;
    }
    private void OnCollisionEnter(Collision collision) 
    {
        if (GameManager.Instance.currentState == GameManager.GameState.GameOver) 
        {
            return;
        } 
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground")) 
        {
            ContactPoint contact = collision.contacts[0]; 
            if (contact.normal.y < 0.5f) 
            {
                HapticFeedback.Vibrate(150, 255);
                GameManager.Instance.EndGame(); 
                rb.isKinematic = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other) 
    { 
        if (other.gameObject.CompareTag("Coin"))
        {
            AudioManager.Instance.PlaySFX("Coin");
            GameManager.Instance.CollectCoin(); 
            Destroy(other.gameObject);
        }
    }

    void Update()
    {
        if (GameManager.Instance.currentState != GameManager.GameState.Playing) return;

        HandlePointerInput();
    }


    void HandlePointerInput()
    {

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                if (touch.position.x < Screen.width / 2) { RequestJump(); }
                else { Boost(); }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (Input.mousePosition.x < Screen.width / 2) { RequestJump(); }
            else { Boost(); }
        }
    }
}