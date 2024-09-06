using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePatrolState : StateMachineBehaviour
{
    float timer;
    public float patrollingTime = 10f;

    Transform player;
    UnityEngine.AI.NavMeshAgent agent;

    public float detectionArea = 18f;
    public float patrolSpeed = 2f;

    List<Transform> waypointList = new List<Transform> ();

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();

        agent.speed = patrolSpeed;
        timer = 0;

        GameObject waypointCluster = GameObject.FindGameObjectWithTag("Waypoints");
        foreach (Transform t in waypointCluster.transform)
        {
            waypointList.Add(t);
        }

        Vector3 nextPosition = waypointList[Random.Range(0, waypointList.Count)].position;
        agent.SetDestination(nextPosition);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointList[Random.Range(0, waypointList.Count)].position);
        }

        timer += Time.deltaTime;
        if (timer > patrollingTime)
        {
            animator.SetBool("patrolling", false);
        }

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionArea)
        {
            animator.SetBool("chasing", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
    }

}
