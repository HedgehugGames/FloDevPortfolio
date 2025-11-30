using UnityEngine;

public class EChaseState: EnemyBaseState
{
    public EChaseState(WayPointEnemyMovement owner): base(owner) {}

    public override void OnEnter()
    {
        Owner.agent.isStopped = false;  // Allow movement again
        Animator.CrossFade(ChaseHash, CrossFadeDuration);
    }

    public override void Update()
    {
        Owner.agent.SetDestination(Owner.player.position);
        
        if (Vector3.Distance(Owner.transform.position, Owner.player.position) >= Owner.alarmRadius) 
        {
            Owner.ChangeState(new ELookState(Owner));
        }

        if (Vector3.Distance(Owner.transform.position, Owner.player.position) <= Owner.attackRange)
        {
            Owner.ChangeState(new EAttackState(Owner));
        }
    }
    public override void OnExit() { }
}
