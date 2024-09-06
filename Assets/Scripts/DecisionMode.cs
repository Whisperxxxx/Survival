using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DecisionMode : MonoBehaviour
{
    public static DecisionMode Instance { get; set; }


    private float currentperformanceScore;
    // Dynamic factor
    public float enemySpawnFactor = 1.0f;
    public float resourceSpawnFactor = 1.0f;

    private float previousPerformanceScore = 0f;  // Follow the player's last performance

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

    public void CalculaetPerformanceScore()
    {
        MonitoringMode monitor = MonitoringMode.Instance;

        // Get the data
        Queue<int> enemiesKilledHistory = monitor.GetEnemiesKilledHistory();
        Queue<int> ammoUsedHistory = monitor.GetAmmoUsedHistory();
        Queue<float> headshotRateHistory = monitor.GetHeadshotRateHistory();
        Queue<int> damageTakenHistory = monitor.GetDamageTakenHistory();
        Queue<float> timeTakenHistory = monitor.GetTimeTakenHistory();

        // Calculate the performance score
        float averageEnemiesKilled = CalculateRollingAverage(enemiesKilledHistory);
        float averageAmmoUsed = CalculateRollingAverage(ammoUsedHistory);
        float averageHeadshotRate = CalculateRollingAverage(headshotRateHistory);
        float averageDamageTaken = CalculateRollingAverage(damageTakenHistory);
        float averageTimeTaken = CalculateRollingAverage(timeTakenHistory);

        // Calculate kill efficiency based on time and ammo used
        float killEfficiencyTime = averageEnemiesKilled / averageTimeTaken;
        float killEfficiencyAmmo = averageEnemiesKilled / (averageAmmoUsed + 1); // Add 1 to avoid division by zero
        float killEfficiency = 0.2f * killEfficiencyTime + 0.8f * killEfficiencyAmmo;

        // Calculate healthScore
        float healthPenalty = averageDamageTaken * 0.2f; // Lost 2 score if every 10hp loss
        float healthBonus = averageDamageTaken <= 40 ? 10 : 5;
        float healthScore = healthBonus - healthPenalty; // If the damage taken is less than 20, receive reward points


        float efficiencyScore = killEfficiency * 34.0f;
        float headShotScore = averageHeadshotRate * 20.0f;
        monitor.efficiency = killEfficiency;
        monitor.EfficiencyScore = efficiencyScore;
        monitor.HeadShotScore = headShotScore;
        monitor.HealthScore = healthScore;


        currentperformanceScore = efficiencyScore + headShotScore + healthScore;
        
        if (currentperformanceScore > 30)
        {
            currentperformanceScore = 30;
        }

        monitor.performanceScore = currentperformanceScore;

    }

    public void AdjustDifficulty()
    {
        CalculaetPerformanceScore();
        float adjustmentFactor = CalculateAdjustmentFactor(currentperformanceScore, previousPerformanceScore);

        // Adjust the difficulty according the performance score
        if (currentperformanceScore > 15)
        {
            IncreaseDifficulty(adjustmentFactor);
        }
        else if (currentperformanceScore < 11)
        {
            DecreaseDifficulty(adjustmentFactor);
        }

        previousPerformanceScore = currentperformanceScore;  // Update the previous performance

    }

    private float CalculateAdjustmentFactor(float currentScore, float previousScore)
    {
        float scoreDifference = currentScore - previousScore;
        float baseAdjustment = 1.2f;

        // Adjust the factor based on fluctuations by increasing or decreasing
        if (Mathf.Abs(scoreDifference) > 2)
        {
            baseAdjustment += 0.1f;  // Increase the adjustment factor
        }
        else if (Mathf.Abs(scoreDifference) < 1)
        {
            baseAdjustment -= 0.2f;  // Decrease the adjustment factor
        }

        // If the performance score is suddenly change, make significant adjustments
        if (Mathf.Abs(scoreDifference) > 4 && (currentScore > 21 || currentScore < 9))
        {
            return baseAdjustment * 1.5f;
        }
        else
        {
            return baseAdjustment;
        }
    }

 
    private float CalculateRollingAverage(Queue<int> queue)
    {
        if (queue.Count == 0)
            return 0;
        // Skip the historical data exceeding the window length, then calculate the average of the window length.
        return (float)queue.ToArray().Skip(Math.Max(0, queue.Count - MonitoringMode.Instance.windowLength)).Average();
    }

    private float CalculateRollingAverage(Queue<float> queue)
    {
        if (queue.Count == 0)
            return 0f;

        return queue.ToArray().Skip(Math.Max(0, queue.Count - MonitoringMode.Instance.windowLength)).Average();
    }

    /*
    // Calculate int
    private float CalculateAverage(Queue<int> queue)
    {
        if (queue.Count == 0)
        {
            return 0;
        }

        int sum = 0;

        foreach (int value in queue)
        {
            sum += value;
        }
        return (float)sum / queue.Count;
    }

    // Calculate float
    private float CalculateAverage(Queue<float> queue)
    {
        if (queue.Count == 0)
        {
            return 0f;

        }

        float sum = 0f;

        foreach (float value in queue)
        {
            sum += value;
        }
        return sum / queue.Count;
    }
    */

    private void IncreaseDifficulty(float adjustmentFactor)
    {
        enemySpawnFactor *= adjustmentFactor;
        resourceSpawnFactor *= 1 / adjustmentFactor;  

        ApplyDifficultyChanges();
    }

    private void DecreaseDifficulty(float adjustmentFactor)
    {
        enemySpawnFactor /= adjustmentFactor;
        resourceSpawnFactor *= adjustmentFactor;  

        ApplyDifficultyChanges();
    }

    private void ApplyDifficultyChanges()
    {
        ExecutionMode.Instance.AdjustSpawnRate(enemySpawnFactor);
        ExecutionMode.Instance.AdjustEnemyCount(enemySpawnFactor);
        ExecutionMode.Instance.AdjustResourceSpawn(resourceSpawnFactor);
    }

}
