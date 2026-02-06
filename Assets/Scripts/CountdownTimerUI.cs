using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CountdownTimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float startTime = 60f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Fade Settings")]
    [SerializeField] private Image fadeImage; // Čierny obrázok cez celú obrazovku
    [SerializeField] private float fadeStartTime = 5f; // Kedy začne stmavovať (5 sekúnd pred koncom)
    [SerializeField] private string nextSceneName = "GameOver"; // Názov scény alebo nechaj prázdne pre ďalšiu v poradí

    private float timeLeft;
    private bool isRunning = true;
    private bool hasLoadedScene = false;

    private void Start()
    {
        timeLeft = startTime;
        UpdateUI();

        // Nastav fade image na priehľadný na začiatku
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ Fade Image nie je priradený! Vytvor UI Image cez celú obrazovku.");
        }
    }

    private void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;

        // Fade efekt v posledných 5 sekundách
        if (timeLeft <= fadeStartTime && fadeImage != null)
        {
            float fadeProgress = 1f - (timeLeft / fadeStartTime); // 0 → 1
            Color c = fadeImage.color;
            c.a = fadeProgress;
            fadeImage.color = c;
        }

        // Timer dosiahol 0
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isRunning = false;

            if (!hasLoadedScene)
            {
                hasLoadedScene = true;
                LoadNextScene();
            }
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (timerText == null) return;

        int seconds = Mathf.FloorToInt(timeLeft);
        int milliseconds = Mathf.FloorToInt((timeLeft - seconds) * 100f);
        timerText.text = $"{seconds:00}:{milliseconds:00}";

        // Červená farba v posledných 10 sekundách
        if (timeLeft <= 10f)
        {
            timerText.color = Color.red;
        }
    }

    private void LoadNextScene()
    {
        Debug.Log("⏰ Čas vypršal! Načítavam scénu...");

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            // Načítaj scénu podľa mena
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // Načítaj ďalšiu scénu v poradí
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
    }

    // Verejné metódy pre ovládanie zvonka
    public void StopTimer()
    {
        isRunning = false;
        Debug.Log("⏸️ Timer zastavený");
    }

    public void AddTime(float seconds)
    {
        timeLeft += seconds;
        Debug.Log($"⏱️ Pridaných {seconds} sekúnd");
    }
}