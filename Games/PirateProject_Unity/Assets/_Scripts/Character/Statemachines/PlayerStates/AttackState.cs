using System;
using System.Collections;
using StarterAssets;
using UnityEngine;

public class AttackState : PlayerBaseState
{
   private bool _hasAttacked;
   public AttackState(ThirdPersonController owner) : base(owner) {}

   public override void OnEnter()
   {
      //Owner.GetComponent<ThirdPersonController>().enabled = false;
      Animator.CrossFade(AttackHash, CrossFadeDuration);
      _hasAttacked = true;
      Owner.StartCoroutine(Attacking());
   }

   public override void Update()
   {
      if (_hasAttacked)
      {
         Owner.ChangeState(new LocomotionState(Owner));
      }
   }

   public override void OnExit()
   {
      _hasAttacked = false;
      //Owner.GetComponent<ThirdPersonController>().enabled = true;
   }
   
   private IEnumerator Attacking()
   {
      yield return null;

      AnimatorClipInfo[] currentClipInfo = Animator.GetCurrentAnimatorClipInfo(0);

      if (currentClipInfo.Length > 0)
      {
         float currentClipLength = currentClipInfo[0].clip.length;

         yield return new WaitForSeconds(currentClipLength); 
      }
   }
}
