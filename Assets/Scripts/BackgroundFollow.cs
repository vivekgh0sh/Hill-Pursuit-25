using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform cameraTransform;
    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("Camera Transform is not assigned in BackgroundFollow script!");
            return;
        }

        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        transform.position += deltaMovement;

        lastCameraPosition = cameraTransform.position;
    }
}