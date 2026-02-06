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

        // Skry timer na zaèiatku
        if (timerText != null)
            timerText.gameObject.SetActive(false);

        // Zaène prvú stage
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

            // Èervená farba keï zostáva menej ako 10 sekúnd
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
            timerText.text = "GAME OVER!";
            timerText.color = Color.red;
        }

        // Tu môžeš prida vlastnú game over logiku
        // Napr. naèíta game over scénu, zobrazi menu, atï.
    }

    IEnumerator GameLoop()
    {
        // STAGE 1
        yield return StartCoroutine(RunStage1());
        StopTimer(); // Timer sa vypne po Stage 1
        yield return new WaitForSeconds(pauseBetweenStages);

        // STAGE 2
        yield return StartCoroutine(RunStage2());
        yield return new WaitForSeconds(pauseBetweenStages);

        // STAGE 3
        yield return StartCoroutine(RunStage3());

        Debug.Log("Všetky stages dokonèené!");
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

        // Wave 2: 5 enemy naraz - 45 sekúnd
        currentWave = 2;
        StartTimer(45f);
        SpawnMultipleEnemies(5);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 3: Boss - 30 sekúnd
        currentWave = 3;
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

        // Wave 1: 5 enemy naraz
        currentWave = 1;
        SpawnMultipleEnemies(5);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 2: 5 enemy naraz
        currentWave = 2;
        SpawnMultipleEnemies(5);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 3: Boss
        currentWave = 3;
        SpawnBoss();
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Po wave 3 zaènú random spawny (1-5 sekúnd)
        yield return StartCoroutine(RandomSpawnLoop(1f, 5f));
    }

    // ==================== STAGE 3 ====================
    IEnumerator RunStage3()
    {
        currentStage = 3;
        Debug.Log("=== STAGE 3 START ===");

        // Wave 1: 5 enemy naraz
        currentWave = 1;
        SpawnMultipleEnemies(5);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 2: 10 enemy naraz
        currentWave = 2;
        SpawnMultipleEnemies(10);
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Wave 3: Boss
        currentWave = 3;
        SpawnBoss();
        yield return StartCoroutine(WaitForAllEnemiesDead());

        // Po wave 3 zaènú random spawny (1-3 sekundy)
        yield return StartCoroutine(RandomSpawnLoop(1f, 3f));
    }

    // ==================== SPAWN FUNKCIE ====================
    void SpawnMultipleEnemies(int count)
    {
        Debug.Log($"Stage {currentStage} - Wave {currentWave}: Spawning {count} enemies");

        // Vytvor zoznam dostupných spawn pointov (bez indexu 0 - parent)
        List<int> availableSpawnPoints = new List<int>();
        for (int i = 1; i < spawnPoints.Length; i++)
        {
            availableSpawnPoints.Add(i);
        }

        // Spawn enemies na rôznych spawn pointoch
        for (int i = 0; i < count; i++)
        {
            // Ak už nie sú dostupné spawn pointy, resetuj zoznam
            if (availableSpawnPoints.Count == 0)
            {
                for (int j = 1; j < spawnPoints.Length; j++)
                {
                    availableSpawnPoints.Add(j);
                }
            }

            // Vyber náhodný dostupný spawn point
            int randomListIndex = Random.Range(0, availableSpawnPoints.Count);
            int spawnPointIndex = availableSpawnPoints[randomListIndex];

            // Odstráò použitý spawn point zo zoznamu
            availableSpawnPoints.RemoveAt(randomListIndex);

            // Spawn enemy
            Transform spawnPoint = spawnPoints[spawnPointIndex];
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            aliveEnemies.Add(enemy);
        }
    }

    void SpawnEnemy()
    {
        // Vyber náhodný spawn point (preskoèí index 0 - parent)
        int randomIndex = Random.Range(1, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Spawn enemy
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        aliveEnemies.Add(enemy);
    }

    void SpawnBoss()
    {
        Debug.Log($"Stage {currentStage} - Wave {currentWave}: Spawning BOSS!");

        // Vyber náhodný spawn point
        int randomIndex = Random.Range(1, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[randomIndex];

        // Spawn boss
        GameObject boss = Instantiate(bossPrefab, spawnPoint.position, spawnPoint.rotation);
        aliveEnemies.Add(boss);
    }

    // ==================== RANDOM SPAWN LOOP ====================
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

    // ==================== WAIT FOR ENEMIES DEAD ====================
    IEnumerator WaitForAllEnemiesDead()
    {
        while (true)
        {
            // Kontrola, èi timer nevypršal
            if (!timerActive && currentStage == 1)
            {
                yield break; // Game over už sa zavolal
            }

            // Odstráò null references (mertve enemies)
            aliveEnemies.RemoveAll(enemy => enemy == null);

            // Ak sú všetci màtvi, pokraèuj
            if (aliveEnemies.Count == 0)
            {
                Debug.Log($"Stage {currentStage} - Wave {currentWave}: All enemies defeated!");
                yield break;
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
}