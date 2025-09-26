using UnityEngine;
using System.Collections;

public class PoolableObject : MonoBehaviour
{
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        // When the object is activated from the pool, start a timer to return it.
        StartCoroutine(ReturnToPoolAfterDelay());
    }

    IEnumerator ReturnToPoolAfterDelay()
    {
        // Wait for the duration of the particle system before deactivating.
        yield return new WaitForSeconds(ps.main.duration);

        // Deactivate the object, returning it to the pool.
        gameObject.SetActive(false);
    }
}
