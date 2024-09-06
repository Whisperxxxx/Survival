using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class MonitoringMode : MonoBehaviour
{
    public static MonitoringMode Instance { set; get; }

    private string filePath;

    // Player performance parameters
    private string id = Guid.NewGuid().ToString().Substring(0, 5);
    public int wave;
    public int health = 100;
    public int ammoUsed;
    public int enemiesKilled;
    public int throwableUsed;
    public int hitshot;
    public int headshot;
    public float headshotRate = 0f;
    public int windowLength = 5;

    public float efficiency;
    public float EfficiencyScore;
    public float HeadShotScore;
    public float HealthScore;

    public float performanceScore;
    // Historical data storage
    private Queue<int> enemiesKilledHistory = new Queue<int>();
    private Queue<int> ammoUsedHistory = new Queue<int>();
    private Queue<int> throwableUsedHistory = new Queue<int>();
    private Queue<float> headshotRateHistory = new Queue<float>();
    private Queue<int> damageTakenHistory = new Queue<int>();
    private Queue<float> timeTakenHistory = new Queue<float>();

    private int historyLength = 10;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            InitializeCSV();
            Instance = this;
            InvokeRepeating(nameof(CollectData), 10f, 10f); // Call the collect method every 10s
        }
    }

    void Update()
    {
        headshotRate = hitshot > 0 ? (float)headshot / hitshot : 0f;
    }

    // Create the csv file
    private void InitializeCSV()
    {
        string folderPath = Path.Combine("D:\\", "Survival");

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        filePath = Path.Combine(folderPath, "GameData.csv");

        if (!File.Exists(filePath))
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Column Header
                writer.WriteLine("ID,Wave,Health,Ammo Used,Enemies Killed, Throwable Used,Headshot Rate,Performance Score");
            }
        }
            
   
    }

    // Collect the data
    private void CollectData()
    {
        int damageTaken = 100 - health;
        float timeTaken = Time.time;
        DecisionMode.Instance.CalculaetPerformanceScore();
        AddToQueue(enemiesKilledHistory, enemiesKilled);
        AddToQueue(ammoUsedHistory, ammoUsed);
        AddToQueue(headshotRateHistory, headshotRate);
        AddToQueue(damageTakenHistory, damageTaken);
        AddToQueue(timeTakenHistory, timeTaken);

        string record = $"{id},{wave},{health},{ammoUsed},{enemiesKilled},{throwableUsed},{headshotRate:F2},{performanceScore:F2}";

        AppendToCSV(record);
    }

    private void AppendToCSV(string record)
    {
        using (StreamWriter writer = File.AppendText(filePath))
        {
            writer.WriteLine(record);
        }
    }

    private void AddToQueue<T>(Queue<T> queue, T value)
    {
        if (queue.Count >= historyLength)
        {
            queue.Dequeue();
        }
        queue.Enqueue(value);
    }

    public Queue<int> GetEnemiesKilledHistory() => new Queue<int>(enemiesKilledHistory);
    public Queue<int> GetAmmoUsedHistory() => new Queue<int>(ammoUsedHistory);
    public Queue<float> GetHeadshotRateHistory() => new Queue<float>(headshotRateHistory);
    public Queue<int> GetDamageTakenHistory() => new Queue<int>(damageTakenHistory);
    public Queue<float> GetTimeTakenHistory() => new Queue<float>(timeTakenHistory);

}
