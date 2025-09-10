using UnityEngine;

public class WheelRotator : MonoBehaviour
{
    [Header("References")]
    public Transform[] wheels; // We will assign all 4 wheels here

    [Header("Settings")]
    public float rotationSpeed = 500f;

    // We need a reference to the CarController to get its speed
    private CarController carController;

    void Start()
    {
        carController = GetComponent<CarController>();
    }

    void Update()
    {
        // Calculate the rotation amount based on the car's actual forward speed
        float rotationAmount = carController.forwardSpeed * Time.deltaTime * 100; // Multiplier for visual effect

        // Loop through each wheel in the array and rotate it
        foreach (Transform wheel in wheels)
        {
            // We rotate around the local X-axis (right). This might be different for your model.
            // If they spin the wrong way, try Vector3.left or experiment with other axes.
            wheel.Rotate(Vector3.right, rotationAmount, Space.Self);
        }
    }
}