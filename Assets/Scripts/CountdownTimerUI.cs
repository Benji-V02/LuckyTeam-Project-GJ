using UnityEngine;
using TMPro;

public class CountdownTimerUI : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float startTime = 60f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    private float timeLeft;
    private bool isRunning = true;

    private void Start()
    {
        timeLeft = startTime;
        UpdateUI();
    }

    private void Update()
    {
        if (!isRunning) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            isRunning = false;
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        int seconds = Mathf.FloorToInt(timeLeft);
        int milliseconds = Mathf.FloorToInt((timeLeft - seconds) * 100f);

        timerText.text = $"{seconds:00}:{milliseconds:00}";
    }
}
