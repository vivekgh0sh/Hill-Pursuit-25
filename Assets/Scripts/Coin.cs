using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Animation Settings")]
    public float spinSpeed = 180f; // Degrees per second
    public bool canBob = true;     // You can uncheck this in the Inspector for static coins
    public float bobSpeed = 2f;    // How fast it bobs
    public float bobHeight = 0.25f; // How high it bobs

    private Vector3 initialPosition;

    void Start()
    {
        // Remember our starting position for the bobbing calculation
        initialPosition = transform.position;
    }

    void Update()
    {
        // 1. Mandatory Spinning
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        // 2. Optional Bobbing
        if (canBob)
        {
            // Use a Sine wave for smooth up-and-down motion
            float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = initialPosition + new Vector3(0, yOffset, 0);
        }
    }
}