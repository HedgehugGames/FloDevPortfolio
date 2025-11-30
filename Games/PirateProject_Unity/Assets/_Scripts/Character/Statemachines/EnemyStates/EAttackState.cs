using System.Collections;
using UnityEngine;

public class EAttackState: EnemyBaseState
{
    public EAttackState(WayPointEnemyMovement owner): base(owner) {}

    public override void OnEnter()
    {
        Animator.CrossFade(AttackHash, CrossFadeDuration);
        Owner.weaponHand.SetActive(true);
        Owner.weaponShaft.SetActive(false);
    }

    public override void Update()
    {
        // Only trigger attack if it hasn't already been done
        if (!Owner.alreadyAttacked)
        {
            Owner.agent.isStopped = true;
            Owner.transform.LookAt(Owner.player);

            Owner.alreadyAttacked = true;

            // Start the cooldown timer
            Owner.StartCoroutine(AttackCooldown(Owner.timeBetweenAttacks));
        }
    }
    private IEnumerator AttackCooldown(float time)
    {
        // Wait for the specified time before allowing another attack
        yield return new WaitForSeconds(time);

        // Reset the attack flag and re-enable movement after the cooldown
        Owner.alreadyAttacked = false;
        Owner.agent.isStopped = false;

        // Check the distance to the player after cooldown
        float distanceToPlayer = Vector3.Distance(Owner.player.position, Owner.transform.position);

        if (distanceToPlayer <= Owner.attackRange)
        {
            Owner.ChangeState(new EAttackState(Owner));
        }
        else
        {
            // If the player is not in range, either idle (if in front) or chase again
            if (distanceToPlayer <= 1f) 
            {  
               Owner.ChangeState(new ELookState(Owner));
            }
            else
            {
                Owner.ChangeState(new EChaseState(Owner));
            }
        }
    }

    public override void OnExit()
    {
        Owner.weaponHand.SetActive(false);
        Owner.weaponShaft.SetActive(true);
    }
}
