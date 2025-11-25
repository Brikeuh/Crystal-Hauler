using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class Enemy : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private EnemyStateMachine stateMachine;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine = new EnemyStateMachine();
        animator = GetComponent<Animator>();

        var wanderingState = new WanderingState(this);
        var enemyAttackState = new EnemyAttackState(this);
        var chaseState = new ChaseState(this);

        stateMachine.AddState("Wandering", wanderingState);
        stateMachine.AddState("EnemyAttackState", enemyAttackState);
        stateMachine.AddState("ChaseState", chaseState);

        stateMachine.SetState("Wandering");
    }
}
