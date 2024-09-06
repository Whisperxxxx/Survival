using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackState : StateMachineBehaviour
{
    Transform player;
    UnityEngine.AI.NavMeshAgent agent;

    public float stopAttackingDistance = 2.5f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LookAtPlayer();

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer > stopAttackingDistance)
        {
            animator.SetBool("attacking", false);
        }
    }



    private void LookAtPlayer() 
    {
        Vector3  direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction); 

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

}
