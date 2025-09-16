using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform playerTransform; // Assign your player's transform here

    private float initialZOffset;
    private float initialXPosition;
    private float initialYPosition;

    void Start()
    {
        if (playerTransform == null)
        {
            Debug.LogError("Player Transform is not assigned in BackgroundFollow script!");
            return;
        }

        // Store our starting position
        initialXPosition = transform.position.x;
        initialYPosition = transform.position.y;

        // Calculate and store the initial distance between the background and the player on the Z-axis
        initialZOffset = transform.position.z - playerTransform.position.z;
    }

    // Use LateUpdate to ensure the player has finished moving for the frame
    void LateUpdate()
    {
        if (playerTransform == null) return;

        // Create the new position for the background container.
        // We use our stored initial X and Y to prevent any vertical or sideways movement.
        // We update the Z position based on the player's current Z plus our initial offset.
        Vector3 newPosition = new Vector3(
            initialXPosition,
            initialYPosition,
            playerTransform.position.z + initialZOffset
        );

        transform.position = newPosition;
    }
}