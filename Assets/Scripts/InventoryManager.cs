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

    [Header("Inventory Mode")]
    [SerializeField] private float focusedCardRaiseY = 40f; // O koľko sa focusnutá karta vysunie vyššie
    [SerializeField] private bool startInInventoryMode = false;

    private List<GameObject> itemCards = new List<GameObject>();
    private readonly Dictionary<RectTransform, Coroutine> activeMoveCoroutines = new Dictionary<RectTransform, Coroutine>();

    private Coroutine exitInventoryRoutine;

    private bool inventoryMode = false;
    private int focusedIndex = 0;


    [Header("Inventory Mode Exit")]
    [SerializeField] private bool focusLeftmostBeforeExit = true;
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

        inventoryMode = startInInventoryMode;
        focusedIndex = 0;
        RefreshLayoutInstant();
    }

    private void Update()
    {
        // TAB = zapnúť/vypnúť inventory mode
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // Ak sme v inventory mode a ideme ho vypnúť:
            // voliteľne (toggle) najprv focusni najľavejšiu kartu (index 0) a až potom prejdime do collapsed layoutu.
            if (inventoryMode)
            {
                if (exitInventoryRoutine != null)
                    StopCoroutine(exitInventoryRoutine);

                if (focusLeftmostBeforeExit)
                {
                    // Pred vypnutím: vycentruj najľavejšiu (index 0) a až potom prepneme do collapsed
                    exitInventoryRoutine = StartCoroutine(ExitInventoryModeSequence());
                }
                else
                {
                    // Vypnutie okamžite bez "pre-focus" sekvencie
                    inventoryMode = false;
                    RefreshLayoutAnimated();
                }
            }
            else
            {
                // Zapnutie inventory mode
                inventoryMode = true;
                focusedIndex = Mathf.Clamp(focusedIndex, 0, Mathf.Max(0, itemCards.Count - 1));
                RefreshLayoutAnimated();
            }
        }

        // F = použitie focused karty (len keď je inventoryMode)
        if (inventoryMode && Input.GetKeyDown(KeyCode.F))
        {
            UseFocusedCard();
        }


        if (!inventoryMode || itemCards.Count == 0)
            return;

        // Scrollovanie medzi kartami
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            // Scroll UP (kladné) -> menší index, Scroll DOWN (záporné) -> väčší index
            int next = focusedIndex + (scroll < 0f ? 1 : -1);
            next = Mathf.Clamp(next, 0, itemCards.Count - 1);

            if (next != focusedIndex)
            {
                focusedIndex = next;
                RefreshLayoutAnimated();
            }
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

        // Pridaj novú kartičku do zoznamu NA ZAČIATOK (index 0 = najnovšia)
        itemCards.Insert(0, newCard);

        // V inventory mode chceme mať focusnutú kartu v strede.
        // Keď pridám nový item, dám focus na index 0 (nový item).
        if (inventoryMode)
            focusedIndex = 0;

        RefreshLayoutAnimated();

        AttachCardLogicByName(newCard, itemName);

        Debug.Log($"✓ Item '{itemName}' added to inventory! Total items: {itemCards.Count}");
    }

    // Voliteľné: Odstránenie itemu z inventára
    public void RemoveItem(int index)
    {
        if (index >= 0 && index < itemCards.Count)
        {
            Destroy(itemCards[index]);
            itemCards.RemoveAt(index);

            focusedIndex = Mathf.Clamp(focusedIndex, 0, Mathf.Max(0, itemCards.Count - 1));
            RefreshLayoutAnimated();
        }
    }

    
    // Sekvencia pre vypnutie inventory mode:
    // 1) focusni najľavejšiu kartu (index 0) a nechaj prebehnúť animáciu centrovania
    // 2) následne prepni do collapsed layoutu
    private System.Collections.IEnumerator ExitInventoryModeSequence()
    {
        // Ak nemáme karty, iba vypni režim
        if (itemCards.Count == 0)
        {
            inventoryMode = false;
            RefreshLayoutAnimated();
            exitInventoryRoutine = null;
            yield break;
        }

        // Najľavejšia karta v tomto systéme = index 0
        focusedIndex = 0;

        // stále sme v inventoryMode = true, takže sa animuje inventory layout
        RefreshLayoutAnimated();

        // počkaj kým dobehne animácia pozícií
        yield return new WaitForSeconds(animationSpeed);

        // teraz vypni inventory mode a prerátaj collapsed layout
        inventoryMode = false;
        RefreshLayoutAnimated();

        exitInventoryRoutine = null;
    }


    // ===== Layout / Animácie =====

    private void RefreshLayoutAnimated()
    {
        if (inventoryMode)
            ApplyInventoryLayout(true);
        else
            ApplyCollapsedLayout(true);
    }

    private void RefreshLayoutInstant()
    {
        if (inventoryMode)
            ApplyInventoryLayout(false);
        else
            ApplyCollapsedLayout(false);
    }

    // Inventory mode:
    // - focusnutá karta je vždy v strede (x=0)
    // - focusnutá karta je vyššie (y=focusedCardRaiseY)
    // - karty s menším indexom sú vľavo, s väčším vpravo, s rovnakým spacingom
    private void ApplyInventoryLayout(bool animated)
    {
        for (int i = 0; i < itemCards.Count; i++)
        {
            RectTransform cardRect = itemCards[i].GetComponent<RectTransform>();

            float x = (i - focusedIndex) * cardSpacing;
            float y = (i == focusedIndex) ? focusedCardRaiseY : 0f;

            Vector2 target = new Vector2(x, y);

            if (animated) AnimateTo(cardRect, target);
            else cardRect.anchoredPosition = target;
        }

        // Render order (prekryvanie):
        // vyššie vo vrstvách = karta bližšie k focusedIndex
        // zodpovedá vzoru: 0, -1, -2, ... podľa -abs(i - focusedIndex)
        ApplyInventoryZOrder();
    }

    // Nastaví poradie vrstiev tak, aby:
    // focused karta bola navrchu, potom karty s abs(dist)=1, potom 2, ...
    // Pri rovnakom dist (ľavá/pravá strana) je ľavá karta navrchu nad pravou.
    private void ApplyInventoryZOrder()
    {
        int n = itemCards.Count;
        if (n == 0) return;

        List<int> order = new List<int>(n);
        for (int i = 0; i < n; i++) order.Add(i);

        order.Sort((a, b) =>
        {
            int da = Mathf.Abs(a - focusedIndex);
            int db = Mathf.Abs(b - focusedIndex);

            // väčší dist ide skôr (nižšie vo vrstvách)
            if (da != db) return db.CompareTo(da);

            // rovnaký dist: pravá strana ide nižšie, ľavá vyššie
            bool aLeft = a < focusedIndex;
            bool bLeft = b < focusedIndex;
            if (aLeft != bLeft)
            {
                // pravá (aLeft=false) má ísť skôr
                return aLeft ? 1 : -1;
            }

            // stabilné poradie v rámci strany
            return a.CompareTo(b);
        });

        // nastav sibling index odspodu (0) po vrch (n-1)
        for (int sibling = 0; sibling < n; sibling++)
        {
            int cardIndex = order[sibling];
            itemCards[cardIndex].transform.SetSiblingIndex(sibling);
        }
    }

    // Collapsed (pôvodné správanie):
    // - karta s indexom 0 v strede
    // - zvyšné karty sa ukladajú doprava
    private void ApplyCollapsedLayout(bool animated)
    {
        for (int i = 0; i < itemCards.Count; i++)
        {
            RectTransform cardRect = itemCards[i].GetComponent<RectTransform>();

            Vector2 target = new Vector2(i * cardSpacing, 0f);

            if (animated) AnimateTo(cardRect, target);
            else cardRect.anchoredPosition = target;
        }
    }

    private void AnimateTo(RectTransform cardRect, Vector2 targetPos)
    {
        if (cardRect == null) return;

        // Stopni predošlú animáciu na tejto karte, aby sa nekumulovali coroutines
        if (activeMoveCoroutines.TryGetValue(cardRect, out Coroutine running) && running != null)
        {
            StopCoroutine(running);
        }

        Coroutine c = StartCoroutine(AnimateCardPosition(cardRect, targetPos));
        activeMoveCoroutines[cardRect] = c;
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

        // uprac dictionary
        if (activeMoveCoroutines.ContainsKey(cardRect))
            activeMoveCoroutines[cardRect] = null;
    }

    private void UseFocusedCard()
    {
        if (!inventoryMode) return;
        if (itemCards.Count == 0) return;

        focusedIndex = Mathf.Clamp(focusedIndex, 0, itemCards.Count - 1);
        GameObject cardGO = itemCards[focusedIndex];

        ItemCard card = cardGO.GetComponent<ItemCard>();
        if (card == null)
        {
            Debug.LogWarning("Focused card has no ItemCard component!");
            return;
        }

        // 1) použi
        card.Use();

        // 2) zmaž z itembaru (consume)
        itemCards.RemoveAt(focusedIndex);
        Destroy(cardGO);

        // 3) oprav focus
        if (focusedIndex >= itemCards.Count)
            focusedIndex = itemCards.Count - 1;

        focusedIndex = Mathf.Max(0, focusedIndex);

        RefreshLayoutAnimated();
    }

    private void AttachCardLogicByName(GameObject cardGO, string itemName)
    {
        // normalizuj názov
        string key = (itemName ?? "").Trim().ToLowerInvariant();

        // ak by prefab náhodou mal ItemCard, odstráň ho (aby neboli 2 logiky)
        ItemCard existing = cardGO.GetComponent<ItemCard>();
        if (existing != null)
            Destroy(existing);

        // vyber typ podľa názvu
        switch (key)
        {
            case "bomb":
            case "bomba":
                cardGO.AddComponent<BombCard>();
                break;

            default:
                // fallback – aby UseFocusedCard vždy našlo ItemCard
                cardGO.AddComponent<ItemCard>();
                break;
        }
    }

}
