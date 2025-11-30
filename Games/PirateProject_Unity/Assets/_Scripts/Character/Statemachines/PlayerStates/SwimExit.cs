using StarterAssets;
using UnityEngine;

public class SwimExit : PlayerBaseState
{
    private bool _hasStartedTransition = false;
    private float _transitionStartTime;
    private const float AnimationDuration = 2.0f;

    public SwimExit(ThirdPersonController owner) : base(owner) { }
    
    public override void OnEnter()
    {
        Animator.CrossFade(SwimExitHash, CrossFadeDuration);
        _transitionStartTime = Time.time;
        _hasStartedTransition = true;
    }

    public override void Update()
    {   
        // Wait for the animation transition to finish
        if (Time.time >= _transitionStartTime + AnimationDuration)
        {
            Owner.ChangeState(new LocomotionState(Owner));
        }
    }
}

