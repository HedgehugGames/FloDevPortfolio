using UnityEngine;

public class EPatrolState: EnemyBaseState
{
    public EPatrolState(WayPointEnemyMovement owner): base(owner) {}

    public override void OnEnter()
    {
        Owner.agent.isStopped = false;
        Animator.CrossFade(PatrolHash, CrossFadeDuration);
    }

    public override void Update()
    {
        if (Owner.waypoints.Length == 0) return; // no waypoints to patrol 

        Transform target = Owner.waypoints[Owner.currentWaypointIndex];
        Owner.agent.SetDestination(target.position);

        // reached destination and look around
        if (Vector3.Distance(Owner.transform.position, target.position) < 1f)
        {
            Owner.currentWaypointIndex = (Owner.currentWaypointIndex + 1) % Owner.waypoints.Length;
            Owner.ChangeState(new ELookState(Owner));
        }

        // detected player
        if (Vector3.Distance(Owner.transform.position, Owner.player.position) <= Owner.alarmRadius)
        {
            Owner.ChangeState(new EAlarmedState(Owner));
        }
    }
    public override void OnExit() { }
}
