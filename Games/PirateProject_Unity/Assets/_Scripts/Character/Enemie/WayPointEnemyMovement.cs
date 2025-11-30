using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class WayPointEnemyMovement : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] private Animator animator;
    public Transform player;
    
    private BaseState<WayPointEnemyMovement> _currentState;

    [Header("Patrolling")]
    public Transform[] waypoints;
    public int currentWaypointIndex = 0;

    // Timer
    public float waitTimer = 0f;
    public bool isWaiting = false;

    [Header("Chasing")]
    public float alarmRadius = 10f;

    [Header("Attacking")]
    public float timeBetweenAttacks;
    public float attackRange;
    public bool alreadyAttacked;
    public GameObject weaponHand;
    public GameObject weaponShaft;
    
    void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    public void StartPatrol()
    {
        ChangeState(new EPatrolState(this));
    }

    public void Idle()
    {
        ChangeState(new ELookState(this));
    }
    void Update()
    {
       _currentState?.Update();
    }

    public void ChangeState(EnemyBaseState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }
    
    public void OnFootstep()
    {
    }

  private void OnDrawGizmosSelected()
  {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(transform.position, alarmRadius);
  }

}
