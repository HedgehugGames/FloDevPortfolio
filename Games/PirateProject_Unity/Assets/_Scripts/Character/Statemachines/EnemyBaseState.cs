using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseState : BaseState<WayPointEnemyMovement>
{
    protected static readonly int PatrolHash = Animator.StringToHash("Walking");
    protected static readonly int LookHash = Animator.StringToHash("Idle");
    protected static readonly int AlarmedHash = Animator.StringToHash("Alarmed");
    protected static readonly int ChaseHash = Animator.StringToHash("Running");
    protected static readonly int AttackHash = Animator.StringToHash("Attacking");
    protected EnemyBaseState(WayPointEnemyMovement owner) : base(owner) { }
}
