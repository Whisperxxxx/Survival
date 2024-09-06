using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHand : MonoBehaviour
{
    public int damage;
    public float attackCooldown = 2.0f; 
    private float lastAttackTime = 0f;
    public Enemy enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !enemy.isDead)
        {
            if (Time.time > lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                other.GetComponent<Player>().TakeDamage(damage);
            }
        }
    }
}
