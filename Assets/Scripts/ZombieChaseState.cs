using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieChaseState : StateMachineBehaviour
{
    Transform player;
    UnityEngine.AI.NavMeshAgent agent;

    public float chaseSpeed = 6f;

    public float stopChasingDistance = 21f;
    public float attackingDistance = 2.5f;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();

        agent.speed = chaseSpeed;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(player.position);
        animator.transform.LookAt(player);

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        if (distanceFromPlayer > stopChasingDistance)
        {
            animator.SetBool("chasing", false );
        }

        if (distanceFromPlayer < attackingDistance)
        {
            animator.SetBool("attacking", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(animator.transform.position);
    }

}
