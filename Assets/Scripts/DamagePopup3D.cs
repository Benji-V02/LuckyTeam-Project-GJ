using UnityEngine;
using TMPro; // Add this

public class DamagePopup3D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float moveSpeed = 2f;

    private TextMeshPro textMesh; // Changed from TextMesh
    private float timer = 0f;
    private Color targetColor = Color.red;
    private bool startFade = false;

    public void Initialize(float damage, Color color)
    {
        textMesh = GetComponent<TextMeshPro>(); // Changed

        if (textMesh != null)
        {
            textMesh.text = $"-{Mathf.RoundToInt(damage)}";
            targetColor = color;
            textMesh.color = new Color(color.r, color.g, color.b, 1f);
            Debug.Log($"[DamagePopup3D] Initialized! Damage: {damage}");
        }
        else
        {
            Debug.LogError("[DamagePopup3D] TextMeshPro component NOT FOUND!");
        }

        Invoke(nameof(StartFading), 0.01f);
        Destroy(gameObject, lifetime);
    }

    private void StartFading()
    {
        startFade = true;
    }

    private void Update()
    {
        if (textMesh == null) return;

        // Move upward
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        // Face camera
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0);
        }

        // Fade out
        if (startFade)
        {
            timer += Time.deltaTime;
            float alpha = 1f - (timer / lifetime);
            alpha = Mathf.Clamp01(alpha);
            textMesh.color = new Color(targetColor.r, targetColor.g, targetColor.b, alpha);
        }
    }
}