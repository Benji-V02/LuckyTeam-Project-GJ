using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Transform itemBarParent; // ItemBar GameObject
    [SerializeField] private GameObject itemCardPrefab; // Prefab pre kartičku
    
    [Header("Card Settings")]
    [SerializeField] private float cardSpacing = 120f; // Vzdialenosť medzi kartičkami
    [SerializeField] private float animationSpeed = 0.3f; // Rýchlosť animácie posunu
    
    private List<GameObject> itemCards = new List<GameObject>();
    
    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("InventoryManager Instance created!");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Debug check
        if (itemBarParent == null)
        {
            Debug.LogError("ItemBar Parent is NOT assigned in InventoryManager!");
        }
        else
        {
            Debug.Log($"ItemBar Parent assigned: {itemBarParent.name}");
        }

        if (itemCardPrefab == null)
        {
            Debug.LogError("Item Card Prefab is NOT assigned in InventoryManager!");
        }
        else
        {
            Debug.Log($"Item Card Prefab assigned: {itemCardPrefab.name}");
        }
    }

    public void AddItem(Sprite itemSprite, string itemName)
    {
        Debug.Log($"=== AddItem called for '{itemName}' ===");
        
        // Kontroly
        if (itemBarParent == null)
        {
            Debug.LogError("ItemBar Parent is NULL! Cannot add item.");
            return;
        }

        if (itemCardPrefab == null)
        {
            Debug.LogError("Item Card Prefab is NULL! Cannot add item.");
            return;
        }

        if (itemSprite == null)
        {
            Debug.LogWarning($"Item sprite is NULL for '{itemName}'!");
        }

        // Vytvor novú kartičku
        GameObject newCard = Instantiate(itemCardPrefab, itemBarParent);
        Debug.Log($"New card instantiated: {newCard.name}");
        
        // Nastav RectTransform
        RectTransform newCardRect = newCard.GetComponent<RectTransform>();
        
        // KRITICKÉ: Nastav veľkosť kartičky
        newCardRect.sizeDelta = new Vector2(100f, 140f); // Šírka x Výška
        
        // Nastav anchor na stred
        newCardRect.anchorMin = new Vector2(0.5f, 0.5f);
        newCardRect.anchorMax = new Vector2(0.5f, 0.5f);
        newCardRect.pivot = new Vector2(0.5f, 0.5f);
        
        // Nastav scale
        newCardRect.localScale = Vector3.one;
        
        // Nová kartička sa objaví v STREDE
        newCardRect.anchoredPosition = Vector2.zero;
        
        // Nastav obrázok kartičky
        Image cardImage = newCard.GetComponent<Image>();
        if (cardImage != null)
        {
            cardImage.sprite = itemSprite;
            cardImage.color = Color.white;
            cardImage.raycastTarget = false;
            Debug.Log($"Card image sprite set to: {itemSprite.name}");
        }
        else
        {
            Debug.LogError("Card prefab doesn't have Image component!");
        }
        
        // POSUŇ všetky existujúce kartičky DOPRAVA (pozitívne hodnoty)
        for (int i = 0; i < itemCards.Count; i++)
        {
            RectTransform cardRect = itemCards[i].GetComponent<RectTransform>();
            // Každá stará kartička sa posunie o cardSpacing doprava
            Vector2 newPos = new Vector2((i + 1) * cardSpacing, 0);
            
            // Animovaný posun
            StartCoroutine(AnimateCardPosition(cardRect, newPos));
        }
        
        // Pridaj novú kartičku do zoznamu NA ZAČIATOK
        itemCards.Insert(0, newCard);
        
        Debug.Log($"✓ Item '{itemName}' added to inventory! Total items: {itemCards.Count}");
        Debug.Log($"New card position: (0, 0), Old cards moved RIGHT");
    }

    private System.Collections.IEnumerator AnimateCardPosition(RectTransform cardRect, Vector2 targetPos)
    {
        Vector2 startPos = cardRect.anchoredPosition;
        float elapsed = 0f;

        while (elapsed < animationSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationSpeed;
            
            // Smooth interpolation
            cardRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            
            yield return null;
        }

        cardRect.anchoredPosition = targetPos;
    }

    // Voliteľné: Odstránenie itemu z inventára
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < itemCards.Count)
        {
            Destroy(itemCards[index]);
            itemCards.RemoveAt(index);
            
            // Prepočítaj pozície zvyšných kariet
            UpdateCardPositions();
        }
    }

    private void UpdateCardPositions()
    {
        for (int i = 0; i < itemCards.Count; i++)
        {
            RectTransform cardRect = itemCards[i].GetComponent<RectTransform>();
            Vector2 newPos = new Vector2((itemCards.Count - 1 - i) * cardSpacing, 0);
            StartCoroutine(AnimateCardPosition(cardRect, newPos));
        }
    }
}