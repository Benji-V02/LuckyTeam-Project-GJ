using UnityEngine;

public class InteractableItem : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private string itemName = "Item";
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private float interactionRadius = 2f;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Visual Feedback")]
    [SerializeField] private bool showGizmo = true;
    [SerializeField] private Color gizmoColor = Color.yellow;

    private Transform player;
    private bool playerInRange = false;
    private bool wasInRange = false; // Pre detekciu zmeny stavu

    private void Start()
    {
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

        float distance = Vector3.Distance(transform.position, player.position);
        playerInRange = distance <= interactionRadius;

        // Detekcia vstupu do dosahu
        if (playerInRange && !wasInRange)
        {
            ShowPickupPrompt();
        }
        // Detekcia opustenia dosahu
        else if (!playerInRange && wasInRange)
        {
            HidePickupPrompt();
        }

        wasInRange = playerInRange;

        // Interakcia
        if (playerInRange && Input.GetKeyDown(interactKey))
        {
            PickUpItem();
        }
    }

    private void ShowPickupPrompt()
    {
        if (InteractionPopup.Instance != null)
        {
            InteractionPopup.Instance.Show(interactKey, itemName);
        }
    }

    private void HidePickupPrompt()
    {
        if (InteractionPopup.Instance != null)
        {
            InteractionPopup.Instance.Hide();
        }
    }

    private void PickUpItem()
    {
        Debug.Log($"Item '{itemName}' picked up!");

        // Skry popup pred zničením objektu
        HidePickupPrompt();

        if (InventoryManager.Instance != null && itemSprite != null)
        {
            InventoryManager.Instance.AddItem(itemSprite, itemName);
        }
        else if (itemSprite == null)
        {
            Debug.LogWarning($"Item '{itemName}' has no sprite assigned!");
        }

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        // Uisti sa, že popup sa skryje aj pri zničení objektu iným spôsobom
        HidePickupPrompt();
    }

    private void OnDrawGizmos()
    {
        if (!showGizmo) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);
    }
}