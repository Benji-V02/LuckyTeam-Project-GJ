using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string itemName = "Item";
    [SerializeField] private Sprite itemSprite; // Sprite pre kartičku (kanvica.png)
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = Color.yellow;
    
    private Transform player;
    private bool playerInRange = false;

    private void Start()
    {
        // Nájdi hráča v scéne
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning("Player not found! Make sure your player has the 'Player' tag.");
        }
    }

    private void Update()
    {
        if (player == null) return;

        // Skontroluj vzdialenosť od hráča
        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRadius;

        // Ak je hráč v dosahu a stlačí klávesu E
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            PickUpItem();
        }
    }

    private void PickUpItem()
    {
        // Vypíš správu do konzoly
        Debug.Log($"Item '{itemName}' picked up!");

        // Pridaj item do inventára (zobraz kartičku v ItemBar-e)
        if (InventoryManager.Instance != null && itemSprite != null)
        {
            InventoryManager.Instance.AddItem(itemSprite, itemName);
        }
        else if (itemSprite == null)
        {
            Debug.LogWarning($"Item '{itemName}' has no sprite assigned!");
        }

        // Zničí objekt
        Destroy(gameObject);
    }

    // Vizualizácia dosahu v editore
    private void OnDrawGizmos()
    {
        if (!showGizmo) return;

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    // Vizualizácia dosahu keď je objekt vybraný
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}