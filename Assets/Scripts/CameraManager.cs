using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public CinemachineCamera virtualCamera;

    void OnEnable()
    {
        PlayerSpawner.OnPlayerSpawned += AssignFollowTarget;
    }

    void OnDisable()
    {
        PlayerSpawner.OnPlayerSpawned -= AssignFollowTarget;
    }

    public void AssignFollowTarget(Transform playerTransform)
    {
        if (virtualCamera != null)
        {
            virtualCamera.Follow = playerTransform;
        }
        else
        {
            Debug.LogError("Virtual Camera not assigned in CameraManager!");
        }
    }
}