using UnityEngine;

public class ParallaxLayerScroller : MonoBehaviour
{
    [Tooltip("The speed at which this layer scrolls. Slower for distant layers.")]
    public float scrollSpeed;

    private Renderer rend;
    private float offset;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        // Continuously calculate the offset based on time and speed
        offset += Time.deltaTime * scrollSpeed;

        // A seamless loop by using the modulo operator
        if (offset > 1.0f)
            offset %= 1.0f;

        // Apply the offset to the material's texture
        rend.material.mainTextureOffset = new Vector2(offset, 0);
    }
}