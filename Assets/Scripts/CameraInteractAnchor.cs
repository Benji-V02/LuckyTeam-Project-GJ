using UnityEngine;
using TMPro;

public class CameraInteractAnchor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("World Offset")]
    [SerializeField] private float xOffset = 29f;
    [SerializeField] private float zOffset = 0f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI uiText;

    private void LateUpdate()
    {
        if (!cameraTransform) return;

        Vector3 pos = transform.position;   // Y sa nemení
        pos.x = cameraTransform.position.x + xOffset;
        pos.z = cameraTransform.position.z + zOffset;
        transform.position = pos;
    }

    private void OnDisable()
    {
        HideUI();
    }

    private void OnDestroy()
    {
        HideUI();
    }

    private void HideUI()
    {
        if (uiText != null)
        {
            Color c = uiText.color;
            c.a = 0f;           // alpha na 0
            uiText.color = c;
        }
    }
}
