 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int HP = 100;
    private Animator animator;

    private UnityEngine.AI.NavMeshAgent navAgent;
    private Zombie zombieScript; 
    public bool isDead;


    private void Start()
    {
        animator = GetComponent<Animator>();
        navAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        zombieScript = GetComponent<Zombie>(); 
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;

        if (HP <= 0 && !isDead) 
        {
            Die();
        }
        else
        {
            animator.SetTrigger("damage");
        }
    }
    
    private void Die()
    {
        if (zombieScript != null)
        {
            zombieScript.zombieDamage = 0;
        }
        isDead = true;
        animator.SetTrigger("die");
        MonitoringMode.Instance.enemiesKilled++;

    }

    private IEnumerator DestroySelf()
    {
        ExecutionMode.Instance.enemiesRemaining--;
        HUDManager.Instance.EnemyRemainUI.text = "Remain: " + ExecutionMode.Instance.enemiesRemaining.ToString();
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 25f);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 30f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 4f);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, 3.4f);
    }
}
