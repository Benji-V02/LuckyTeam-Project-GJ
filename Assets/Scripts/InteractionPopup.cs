using UnityEngine;
using TMPro;
using System.Collections;

public class InteractionPopup : MonoBehaviour
{
    public static InteractionPopup Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject popupPanel;
    [SerializeField] private TextMeshProUGUI promptText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private string promptFormat = "Press [{0}] to pick up";
    [SerializeField] private float fadeSpeed = 5f;

    private Coroutine fadeCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    public void Show(KeyCode key, string itemName)
    {
        if (popupPanel == null || promptText == null) return;

        promptText.text = string.Format(promptFormat, key.ToString(), itemName);
        popupPanel.SetActive(true);

        if (canvasGroup != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    public void Hide()
    {
        if (canvasGroup != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeOut());
        }
        else if (popupPanel != null)
        {
            popupPanel.SetActive(false);
        }
    }

    private IEnumerator FadeIn()
    {
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += Time.deltaTime * fadeSpeed;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= Time.deltaTime * fadeSpeed;
            yield return null;
        }
        popupPanel.SetActive(false);
    }
}