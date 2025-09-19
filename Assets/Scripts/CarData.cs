using UnityEngine;

[System.Serializable]
public class CarData
{
    public string carID; // A unique ID like "Racer_01"
    public string carName;
    public GameObject carPrefab;
    public int unlockCost;

    [Header("Showroom Display Settings")]
    public Vector3 displayPositionOffset;
    public Vector3 displayRotation;
    public float displayScale = 1f;
}