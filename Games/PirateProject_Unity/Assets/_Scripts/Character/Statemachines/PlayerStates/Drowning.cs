using StarterAssets;
using UnityEngine;

public class Drowning : PlayerBaseState
{
    public Drowning(ThirdPersonController owner) : base(owner) { }
    
    public override void OnEnter()
    {
        Animator.CrossFade(DrowningHash, CrossFadeDuration);
    }
}
