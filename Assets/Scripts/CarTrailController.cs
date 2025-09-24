using UnityEngine;

public class CarTrailController : MonoBehaviour
{
    [Header("References")]
    public TrailRenderer[] trails;
    private CarController carController;

    void Start()
    {
        carController = GetComponent<CarController>();
        // Ensure trails are off when the game starts
        SetTrailsEmitting(false);
    }

    void Update()
    {
        // Check the car's boosting state every frame
        if (carController.IsBoosting())
        {
            // If the car is boosting, turn the trails ON.
            SetTrailsEmitting(true);
        }
        else
        {
            // If the car is NOT boosting, turn the trails OFF.
            SetTrailsEmitting(false);
        }
    }

    private void SetTrailsEmitting(bool isEmitting)
    {
        // This helper function efficiently turns all trails on or off
        foreach (TrailRenderer trail in trails)
        {
            if (trail.emitting != isEmitting)
            {
                trail.emitting = isEmitting;
            }
        }
    }
}