using UnityEngine;
using UnityEngine.UI;

public class UIDebugger : MonoBehaviour
{
    [SerializeField] private Transform itemBarParent;

    private void Start()
    {
        if (itemBarParent == null)
        {
            Debug.LogError("ItemBar Parent not assigned!");
            return;
        }

        Debug.Log("=== UI DEBUG INFO ===");
        
        // Canvas info
        Canvas canvas = itemBarParent.GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            Debug.Log($"Canvas Render Mode: {canvas.renderMode}");
            Debug.Log($"Canvas Scale Factor: {canvas.scaleFactor}");
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                Debug.Log($"Canvas Scaler UI Scale Mode: {scaler.uiScaleMode}");
            }
        }
        else
        {
            Debug.LogError("ItemBar is not under a Canvas!");
        }

        // ItemBar info
        RectTransform itemBarRect = itemBarParent.GetComponent<RectTransform>();
        if (itemBarRect != null)
        {
            Debug.Log($"ItemBar Position: {itemBarRect.anchoredPosition}");
            Debug.Log($"ItemBar Size: {itemBarRect.sizeDelta}");
            Debug.Log($"ItemBar Scale: {itemBarRect.localScale}");
            Debug.Log($"ItemBar Anchors: Min({itemBarRect.anchorMin}) Max({itemBarRect.anchorMax})");
        }

        // Check children
        Debug.Log($"ItemBar has {itemBarParent.childCount} children");
        for (int i = 0; i < itemBarParent.childCount; i++)
        {
            Transform child = itemBarParent.GetChild(i);
            RectTransform childRect = child.GetComponent<RectTransform>();
            Image childImage = child.GetComponent<Image>();
            
            Debug.Log($"Child {i}: {child.name}");
            Debug.Log($"  - Active: {child.gameObject.activeSelf}");
            Debug.Log($"  - Position: {childRect.anchoredPosition}");
            Debug.Log($"  - Size: {childRect.sizeDelta}");
            Debug.Log($"  - Scale: {childRect.localScale}");
            if (childImage != null)
            {
                Debug.Log($"  - Has Image: YES, Sprite: {(childImage.sprite != null ? childImage.sprite.name : "NULL")}");
                Debug.Log($"  - Image Color: {childImage.color}");
                Debug.Log($"  - Image Enabled: {childImage.enabled}");
            }
            else
            {
                Debug.Log($"  - Has Image: NO");
            }
        }
    }
}