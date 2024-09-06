using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExecutionMode : MonoBehaviour
{
    public static ExecutionMode Instance { get; set; }

    public GameObject[] enemyPrefabs;
    public List<Transform[]> waveSpecificSpawnPoints;
    public Transform[] wave1SpawnPoints;
    public Transform[] wave2SpawnPoints;
    public Transform[] wave3SpawnPoints;
    public Transform[] wave4SpawnPoints;
    public GameObject[] resourcePrefabs; 
    public Transform[] resourceSpawnPoints; 
    private List<GameObject> currentWaveEnemies; 
    public float baseSpawnInterval = 0.5f;  
    public int baseEnemyCount = 10;
    public int baseResourceCount = 3; 
    public int maxWaves = 20;
    private int minEnemyCount = 15;
    private int maxEnemyCount = 30;
    private int minResourceCount = 3;
    private int maxResourceCount = 10;

    public int currentWave = 0;  
    public int enemiesRemaining;  
    public float spawnInterval;
    public int enemiesToSpawn;
    public int resourcesToSpawn;

    private float waveCooldown = 10f; // Time in seconds betwwen waves
    private bool inCooldown ;
    private float cooldownCounter = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
            waveSpecificSpawnPoints = new List<Transform[]>
        {
            wave1SpawnPoints,
            wave2SpawnPoints,
            wave3SpawnPoints,
            wave4SpawnPoints
        };
        currentWaveEnemies = new List<GameObject>();
        StartCoroutine(SpawnWaves());
    }

    void Update()
    {
        if (inCooldown)
        {
            cooldownCounter -= Time.deltaTime;
        }
        else
        {
            cooldownCounter = waveCooldown;
        }
        HUDManager.Instance.cooldownCouterUI.text = cooldownCounter.ToString("F2");
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < maxWaves)
        {
            currentWave++;
            HUDManager.Instance.currentWaveUI.text = "Wave: " + currentWave.ToString();
            MonitoringMode.Instance.wave = currentWave;

            // Adjust the difficulty before every wave
            DecisionMode.Instance.AdjustDifficulty();

            // Set the amount of enemies
            SetWaveEnemies();

            SpawnResources();

            for (int i = 0; i < enemiesToSpawn; i++)
            {
                // Randomly select an enemy type for the current wave to generate
                SpawnEnemy(currentWaveEnemies[Random.Range(0, currentWaveEnemies.Count)]);
                yield return new WaitForSeconds(0.5f);
            }

            // Waiting all the enemies been killed
            while (enemiesRemaining > 0)
            {
                yield return null;
            }
            // Victory conditions
            if (currentWave >= maxWaves)
            {
                HUDManager.Instance.EndGame();
                yield break;  
            }

            if (enemiesRemaining == 0 && inCooldown == false)
            {
                StartCoroutine(WaveCooldown());
            }

            // Next wave
            yield return new WaitForSeconds(waveCooldown);

        }
    }

    void SetWaveEnemies()
    {
        currentWaveEnemies.Clear();  


        if (currentWave <= 5)
        {
            currentWaveEnemies.Add(enemyPrefabs[0]);
            minEnemyCount = 15;
        }
        else if (currentWave > 5 && currentWave <= 10)
        {
            currentWaveEnemies.Add(enemyPrefabs[0]);  
            currentWaveEnemies.Add(enemyPrefabs[1]);
            minEnemyCount = 10;
        }
        else if (currentWave > 10 && currentWave <= 15)
        {
            currentWaveEnemies.Add(enemyPrefabs[1]);  
            currentWaveEnemies.Add(enemyPrefabs[2]);
            minEnemyCount = 7;

        }
        else if (currentWave > 15)
        {
            currentWaveEnemies.Add(enemyPrefabs[2]);  
            currentWaveEnemies.Add(enemyPrefabs[3]);
            minEnemyCount = 5;

        }
    }

    void SpawnEnemy(GameObject enemyPrefab)
    {
        int index = (currentWave - 1) / 5;
        index = Mathf.Clamp(index, 0, waveSpecificSpawnPoints.Count - 1);

        Transform[] selectedSpawnPoints = waveSpecificSpawnPoints[index];
        Transform spawnPoint = selectedSpawnPoints[Random.Range(0, selectedSpawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        enemiesRemaining++;
        HUDManager.Instance.EnemyRemainUI.text = "Remain: " + enemiesRemaining.ToString();

    }

    void SpawnResources()
    {
        if (resourceSpawnPoints.Length == 0)
        {
            Debug.LogError("No resource spawn points available.");
            return;
        }

        for (int i = 0; i < resourcesToSpawn; i++)
        {
            int spawnIndex = i % resourceSpawnPoints.Length;
            Transform spawnPoint = resourceSpawnPoints[spawnIndex];
            GameObject resourcePrefab = resourcePrefabs[Random.Range(0, resourcePrefabs.Length)];
            Instantiate(resourcePrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }

    private IEnumerator WaveCooldown()
    {
        inCooldown = true;
        HUDManager.Instance.waveOverUI.gameObject.SetActive(true);

        yield return new WaitForSeconds(waveCooldown);

        inCooldown = false;
        HUDManager.Instance.waveOverUI.gameObject.SetActive(false);
    }

    public void AdjustSpawnRate(float factor)
    {
        spawnInterval = baseSpawnInterval / factor;
    }

    public void AdjustEnemyCount(float factor)
    {
        enemiesToSpawn = Mathf.RoundToInt(baseEnemyCount * factor );
        if (enemiesToSpawn < minEnemyCount)
        {
            enemiesToSpawn = minEnemyCount;
        }

        if (enemiesToSpawn > maxEnemyCount)
        {
            enemiesToSpawn = maxEnemyCount;
        }
    }

    public void AdjustResourceSpawn(float factor)
    {
        resourcesToSpawn = Mathf.RoundToInt(baseResourceCount * factor);
        if (resourcesToSpawn < minResourceCount)
        {
            resourcesToSpawn = minResourceCount;
        }

        if (resourcesToSpawn > maxResourceCount)
        {
            resourcesToSpawn = maxResourceCount;
        }
    }

}
