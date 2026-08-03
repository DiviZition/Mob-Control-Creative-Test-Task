using UnityEngine;

public class SimpleShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    public float amplitude = 0.1f;   // How far it moves
    public float frequency = 12f;    // How fast it shakes

    private Vector3 originalLocalPosition;

    void Start()
    {
        originalLocalPosition = transform.localPosition;
    }

    void Update()
    {
        //float offsetX = Mathf.Sin(Time.time * frequency) * amplitude;
        float offsetY = Mathf.Sin(Time.time * frequency * 1.3f) * amplitude; // slight phase difference feels nicer

        transform.localPosition = originalLocalPosition + new Vector3(0f, offsetY, 0f);
    }
}