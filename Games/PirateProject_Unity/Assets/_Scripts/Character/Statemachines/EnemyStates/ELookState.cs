using System.Threading;
using UnityEngine;

public class ELookState: EnemyBaseState
{
    private float timer;
    public ELookState(WayPointEnemyMovement owner):base(owner) {}

    public override void OnEnter()
    {
        Owner.agent.isStopped = true;
        Owner.agent.SetDestination(Owner.transform.position);
        
        Animator.CrossFade(LookHash, CrossFadeDuration);
        Owner.isWaiting = true;
        timer = Owner.waitTimer;
    }
    public override void Update()
    {
        // wait on sport until stat patrolling again
        WaitingTime();
        
        // detected player
        if (Vector3.Distance(Owner.transform.position, Owner.player.position) <= Owner.alarmRadius)
        {
            Owner.ChangeState(new EAlarmedState(Owner));
        }
    }
    private void WaitingTime()
    {
        if (Owner.isWaiting)
        {
            Owner.waitTimer -= Time.deltaTime;
            if (Owner.waitTimer <= 0f)
            {
                Owner.isWaiting = false;
                Owner.ChangeState(new EPatrolState(Owner));
            }
        }
    }

    public override void OnExit()
    {
        Owner.waitTimer = timer;
    }
}
