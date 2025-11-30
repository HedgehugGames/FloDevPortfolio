using System.Collections;
using UnityEngine;

public class EAlarmedState: EnemyBaseState
{
    private bool _isAlarmed;

    public EAlarmedState(WayPointEnemyMovement owner):base(owner) {}

    public override void OnEnter()
    {
        Owner.agent.isStopped = true;
        Animator.CrossFade(AlarmedHash, 0);
        _isAlarmed = true;
        
        Owner.StartCoroutine(DelayedAlarmedStart());
    }
    public override void Update()
    {
        if (Vector3.Distance(Owner.transform.position, Owner.player.position) >= Owner.alarmRadius)
        {
            Owner.ChangeState(new ELookState(Owner));
        }

        if (!_isAlarmed)
        {
            Owner.ChangeState(new EChaseState(Owner));
        }
    }

    private IEnumerator AlarmedTime(float time)
    {
        Owner.transform.LookAt(Owner.player);
        yield return new WaitForSeconds(time); 
        _isAlarmed = false;
    }
    private IEnumerator DelayedAlarmedStart()
    {
        yield return null;

        AnimatorClipInfo[] currentClipInfo = Animator.GetCurrentAnimatorClipInfo(0);

        if (currentClipInfo.Length > 0)
        {
            float currentClipLength = currentClipInfo[0].clip.length;
            string clipName = currentClipInfo[0].clip.name;

            yield return AlarmedTime(currentClipLength);
        }
    }
    public override void OnExit() {}
}
