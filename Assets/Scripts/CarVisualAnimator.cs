using UnityEngine;

public class CarVisualAnimator : MonoBehaviour
{
    [Header("References")]
    public Transform carVisuals;
    private Rigidbody rb;
    private CarController carController;

    [Header("Animation Settings")]
    [Tooltip("The rotation of the car's nose when jumping at full speed.")]
    public float maxJumpPitch = 20f;
    [Tooltip("The rotation of the car's nose when falling at high speed.")]
    public float maxFallPitch = -15f;
    [Tooltip("The upward velocity at which the max jump pitch is reached.")]
    public float velocityForMaxJumpPitch = 10f;
    [Tooltip("The downward velocity at which the max fall pitch is reached.")]
    public float velocityForMaxFallPitch = -15f;

    [Header("Smoothing")]
    [Tooltip("How long it takes for the visual rotation to smoothly catch up. Smaller values are faster.")]
    public float rotationSmoothTime = 0.1f;

    // Internal variables for the smoothing function
    private float currentPitch;
    private float pitchVelocityRef; // This is used by SmoothDamp internally

    void Start()
    {
        // We still need the controller to know if we are grounded.
        carController = GetComponent<CarController>();
        // We need the Rigidbody to get the raw velocity data.
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float targetPitch;

        // 1. Determine the Target Pitch
        if (carController.IsGrounded())
        {
            // If on the ground, the target is always to be perfectly flat.
            targetPitch = 0f;
        }
        else
        {
            // If in the air, calculate the pitch based on vertical speed.
            float yVelocity = rb.linearVelocity.y;

            if (yVelocity > 0) // Going UP
            {
                // Map the upward velocity to the jump pitch angle.
                // As velocity goes from 0 to 'velocityForMaxJumpPitch', pitch goes from 0 to 'maxJumpPitch'.
                targetPitch = (yVelocity / velocityForMaxJumpPitch) * -maxJumpPitch;
            }
            else // Going DOWN
            {
                // Map the downward velocity to the fall pitch angle.
                targetPitch = (yVelocity / velocityForMaxFallPitch) * -maxFallPitch;
            }
        }

        // 2. Smooth the Current Pitch towards the Target Pitch
        // This is the core of the fix. Mathf.SmoothDamp creates a stable, non-jittery value.
        currentPitch = Mathf.SmoothDamp(currentPitch, targetPitch, ref pitchVelocityRef, rotationSmoothTime);

        // 3. Apply the final, smoothed rotation to the visual model.
        carVisuals.localRotation = Quaternion.Euler(currentPitch, 0, 0);
    }
}