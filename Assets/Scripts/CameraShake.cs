using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 originalPosition;
    private float shakeTimer = 0f;
    private float shakeDuration = 0f;
    private float shakeIntensity = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (Camera.main != null)
        {
            originalPosition = Camera.main.transform.localPosition;
        }
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;

            // Random shake offset
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            Camera.main.transform.localPosition = originalPosition + shakeOffset;

            if (shakeTimer <= 0f)
            {
                // Reset camera position
                Camera.main.transform.localPosition = originalPosition;
            }
        }
    }

    public void Shake(float duration = 0.2f, float intensity = 0.3f)
    {
        originalPosition = Camera.main.transform.localPosition;
        shakeDuration = duration;
        shakeIntensity = intensity;
        shakeTimer = duration;
    }
}