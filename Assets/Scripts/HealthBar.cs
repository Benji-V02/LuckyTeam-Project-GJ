using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private Image fillImage;

    [Header("Colors")]
    [SerializeField] private Color highHealthColor = Color.green;
    [SerializeField] private Color mediumHealthColor = Color.yellow;
    [SerializeField] private Color lowHealthColor = Color.red;

    [Header("Settings")]
    [SerializeField] private bool worldSpace = true;
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 0);

    private Camera mainCamera;
    private Transform targetTransform;

    private void Start()
    {
        mainCamera = Camera.main;
        targetTransform = transform.parent;

        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (fillImage == null && slider != null)
        {
            fillImage = slider.fillRect.GetComponent<Image>();
        }
    }

    private void LateUpdate()
    {
        // Ak je healthbar vo world space, otáèaj ho smerom ku kamere
        if (worldSpace && mainCamera != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }

    public void SetMaxHealth(float maxHealth)
    {
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }
        UpdateHealthColor();
    }

    public void SetHealth(float health)
    {
        if (slider != null)
        {
            slider.value = health;
        }
        UpdateHealthColor();
    }

    private void UpdateHealthColor()
    {
        if (fillImage == null || slider == null) return;

        float healthPercentage = slider.value / slider.maxValue;

        if (healthPercentage > 0.6f)
        {
            fillImage.color = highHealthColor;
        }
        else if (healthPercentage > 0.3f)
        {
            fillImage.color = mediumHealthColor;
        }
        else
        {
            fillImage.color = lowHealthColor;
        }
    }
}