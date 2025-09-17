using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform cameraTransform; // Assign your Main Camera's transform

    // We will store the camera's position from the previous frame
    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform is not assigned in BackgroundFollow script!");
            return;
        }

        // At the start, store the camera's initial position.
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // 1. Calculate how much the camera has moved since the last frame.
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // 2. Apply that exact same movement to our background container.
        // This preserves the initial offset you created in the editor.
        transform.position += deltaMovement;

        // 3. Update the last camera position for the next frame's calculation.
        lastCameraPosition = cameraTransform.position;
    }
}