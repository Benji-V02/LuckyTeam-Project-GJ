using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject enemyPrefab;
    public GameObject bossPrefab;

    [Header("Settings")]
    public float pauseBetweenStages = 5f;

    [Header("UI")]
    public TextMeshProUGUI timerText;

    [Header("Camera Filter")]
    public GameObject monochromeFilterObject;

    [Header("Victory Screen")]
    public GameObject gameWinObject;
    public RawImage winRawImage;
    public Texture winTexture;

    [Header("Game Over Screen")]
    public GameObject gameLostObject;
    public RawImage lostRawImage;
    public Texture lostTexture;

    private Transform[] spawnPoints;
    private List<GameObject> aliveEnemies = new List<GameObject>();

    private int currentStage = 1;
    private int currentWave = 1;
    private float currentTimer = 0f;
    private bool timerActive = false;

    void Start()
    {
        // Automaticky nájde všetky spawn pointy
        spawnPoints = GetComponentsInChildren<Transform>();

        // Skry timer na začiatku
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        // MonochromeFilter musí byť VYPNUTÝ na začiatku
        if (monochromeFilterObject != null)
        {
            monochromeFilterObject.SetActive(false);
            Debug.Log("✓ MonochromeFilter vypnutý na začiatku");
        }

        // Začne prvú stage
        StartCoroutine(GameLoop());
    }

    void Update()
    {
        // Update timer
        if (timerActive)
        {
            currentTimer -= Time.deltaTime;

            if (currentTimer <= 0)
            {
                currentTimer = 0;
                GameOver();
            }

            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTimer / 60f);
            int seconds = Mathf.FloorToInt(currentTimer % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            // Červená farba keď zostáva menej ako 10 sekúnd
            if (currentTimer <= 10f)
            {
                timerText.color = Color.red;
            }
            else
            {
                timerText.color = Color.white;
            }
        }
    }

    void StartTimer(float duration)
    {
        currentTimer = duration;
        timerActive = true;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        Debug.Log($"Timer started: {duration} seconds");
    }

    void StopTimer()
    {
        timerActive = false;

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        Debug.Log("Timer stopped and hidden");
    }

    void GameOver()
    {
        timerActive = false;
        StopAllCoroutines();

        Debug.Log("GAME OVER - Time's up!");

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false); // Skry timer
        }

        // 🎮 PREHRA - Zobraz Lost screen
        if (gameLostObject != null)
        {
            gameLostObject.SetActive(true);
        }

        if (lostRawImage != null && lostTexture != null)
        {
            lostRawImage.texture = lostTexture;
        }

        // Zamoruj hru
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Vypni spawner
        this.enabled = false;
    }

    IEnumerator GameLoop()
    {
        // STAGE 1
        yield return StartCoroutine(RunStage1());
        StopTimer();
        yield return new WaitForSeconds(pauseBetweenStages);

        // STAGE 2
        yield return StartCoroutine(RunStage2());
        yield return new WaitForSeconds(pauseBetweenStages);

        // STAGE 3
        yield return StartCoroutine(RunStage3());

        Debug.Log("Všetky stages dokončené!");
    }

    // ==================== STAGE 1 ====================
    IEnumerator RunStage1()
    {
        currentStage = 1;
        Debug.Log("=== STAGE 1 START ===");

        // Wave 1: 3 enemy naraz - 60 sekúnd
        currentWave = 1;
        StartTimer(60f);
        SpawnMultipleEnemies(3);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 2: Boss - 30 sekúnd
        currentWave = 2;
        StartTimer(30f);
        SpawnBoss();
        yield return StartCoroutine(WaitForAllEnemiesDead());

        Debug.Log("=== STAGE 1 COMPLETED ===");
    }

    // ==================== STAGE 2 ====================
    IEnumerator RunStage2()
    {
        currentStage = 2;
        Debug.Log("=== STAGE 2 START ===");

        // Wave 1: 2 enemy naraz
        currentWave = 1;
        SpawnMultipleEnemies(2);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 2: Boss
        currentWave = 2;
        SpawnBoss();
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // 🎬 ZAPNI ČIERNOBIELY FILTER
        if (monochromeFilterObject != null)
        {
            monochromeFilterObject.SetActive(true);
            Debug.Log("✅ MonochromeFilter ZAPNUTÝ - čiernobielo!");
        }

        // Po wave 2 začnú random spawny (1-5 sekúnd)
        yield return StartCoroutine(RandomSpawnLoop(1f, 5f));
    }

    // ==================== STAGE 3 ====================
    IEnumerator RunStage3()
    {
        currentStage = 3;
        Debug.Log("=== STAGE 3 START ===");

        // Wave 1: 2 enemy naraz
        currentWave = 1;
        SpawnMultipleEnemies(2);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 2: Boss (POSLEDNÝ!)
        currentWave = 2;
        SpawnBoss();
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // 🎬 VYPNI ČIERNOBIELY FILTER
        if (monochromeFilterObject != null)
        {
            monochromeFilterObject.SetActive(false);
            Debug.Log("✅ MonochromeFilter VYPNUTÝ - farby späť!");
        }

        // 🏆 VÍŤAZSTVO - Zobraz Win screen
        Debug.Log("=== VICTORY! ===");

        if (gameWinObject != null)
        {
            gameWinObject.SetActive(true);
        }

        if (winRawImage != null && winTexture != null)
        {
            winRawImage.texture = winTexture;
        }

        // Koniec hry - už nespawnuj ďalších nepriateľov

        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        this.enabled = false;
        yield break;
    }
    // ==================== SPAWN FUNKCIE ====================
    void SpawnMultipleEnemies(int count)
    {
        Debug.Log($"Stage {currentStage} - Wave {currentWave}: Spawning {count} enemies");

        List<int> availableSpawnPoints = new List<int>();
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(i);
        }

        for (int i = 0; i < count; i++)
        {
            if (availableSpawnPoints.Count == 0)
            {
                for (int j = 1; j < spawnPoints.Length; j++)
                {
                    availableSpawnPoints.Add(j);
                }
            }

            int randomListIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnPointIndex = availableSpawnPoints[randomListIndex];
            availableSpawnPoints.RemoveAt(randomListIndex);

            Transform spawnPoint = spawnPoints[spawnPointIndex];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            aliveEnemies.Add(enemy);
        }
    }

    void SpawnEnemy()
    {
        int randomIndex = Random.Range(1, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        aliveEnemies.Add(enemy);
    }

    void SpawnBoss()
    {
        Debug.Log($"Stage {currentStage} - Wave {currentWave}: Spawning BOSS!");
        int randomIndex = Random.Range(1, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        aliveEnemies.Add(boss);
    }

    IEnumerator RandomSpawnLoop(float minInterval, float maxInterval)
    {
        Debug.Log($"Starting random spawn loop (interval: {minInterval}-{maxInterval}s)");

        while (true)
        {
            float randomWait = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(randomWait);
            SpawnEnemy();
        }
    }

    IEnumerator WaitForAllEnemiesDead()
    {
        while (true)
        {
            // Ak čas vypršal počas Stage 1, ukonči hru
            if (!timerActive && currentStage == 1 && currentTimer <= 0)
            {
                Debug.Log("Time expired - GAME OVER!");
                GameOver();
                yield break;
            }

            aliveEnemies.RemoveAll(enemy => enemy == null);

            if (aliveEnemies.Count == 0)
            {
                Debug.Log($"Stage {currentStage} - Wave {currentWave}: All enemies defeated!");
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}