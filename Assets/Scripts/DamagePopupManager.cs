using UnityEngine;

public class DamagePopupManager : MonoBehaviour
{
    public static DamagePopupManager Instance { get; private set; }

    [Header("Prefabs")]
    [SerializeField] private GameObject damagePopupPrefab;

    [Header("Settings")]
    [SerializeField] private Vector3 offsetFromEnemy = new Vector3(0, 2f, 0);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[DamagePopupManager] Instance created!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowDamage(Vector3 position, float damage, Color? color = null)
    {
        if (damagePopupPrefab == null)
        {
            Debug.LogError("[DamagePopupManager] Damage Popup Prefab is NOT assigned!");
            return;
        }

        Vector3 spawnPosition = position + offsetFromEnemy;
        GameObject popupObj = Instantiate(damagePopupPrefab, spawnPosition, Quaternion.identity);

        Debug.Log($"[DamagePopupManager] Popup spawned at {spawnPosition}");

        DamagePopup3D popup = popupObj.GetComponent<DamagePopup3D>();
        if (popup != null)
        {
            popup.Initialize(damage, color ?? Color.red);
        }
        else
        {
            Debug.LogError("[DamagePopupManager] DamagePopup3D component NOT FOUND on prefab!");
        }
    }
}