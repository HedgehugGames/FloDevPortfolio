using GameEvents;

public class PatrolEvent : GameEvent
{
    public WayPointEnemyMovement AI;
    public bool PlayerEnterTrigger; 

    public void StartPatrolEvent(WayPointEnemyMovement ai, bool playerInRange)
    {
        this.PlayerEnterTrigger = playerInRange;
        this.AI = ai;
    }
}
