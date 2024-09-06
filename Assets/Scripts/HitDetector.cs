using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    public Enemy enemy; // Reference to the main Enemy script

    private void Start()
    {
        // Ensure that the parent has an Enemy script
        enemy = GetComponentInParent<Enemy>();
    }

    public void ReceiveDamage(int damage)
    {
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}

