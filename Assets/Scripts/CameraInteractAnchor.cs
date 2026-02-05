using UnityEngine;

public class CameraInteractAnchor : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;

    [Header("World Offset")]
    [SerializeField] private float xOffset = 29f;
    [SerializeField] private float zOffset = 0f;

    [Header("UI")]
    [SerializeField] private GameObject uiElement;

    private void LateUpdate()
    {
        if (!cameraTransform) return;

        Vector3 pos = transform.position;   // Y ostáva nedotknuté
        pos.x = cameraTransform.position.x + xOffset;
        pos.z = cameraTransform.position.z + zOffset;

        transform.position = pos;
    }

    private void OnDisable()
    {
        DisableUI();
    }

    private void OnDestroy()
    {
        DisableUI();
    }

    private void DisableUI()
    {
        if (uiElement != null)
        {
            uiElement.SetActive(false);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.2f);
    }
}
