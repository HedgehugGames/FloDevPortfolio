using UnityEngine;
using StarterAssets;

public class JumpState : PlayerBaseState
{
    private bool _jumped;

    public JumpState(ThirdPersonController owner) : base(owner) { }

    public override void OnEnter()
    {
        _jumped = false;

        Animator.CrossFade(JumpHash, CrossFadeDuration);

        Owner.verticalVelocity = Mathf.Sqrt(Owner.JumpHeight * -2f * Owner.Gravity);
        
        // reset the fall timeout timer
        Owner.fallTimeoutDelta = Owner.FallTimeout;
        
        // reset the jump timeout timer
        Owner.jumpTimeoutDelta = Owner.JumpTimeout;
        
        _jumped = true;

        // Consume the jump input
        Owner.input.jump = false;
    }

    public override void Update()
    {
        JumpAndGravity();
        CheckStateTransitions();

        // Apply combined movement
        Vector3 move = Owner.transform.forward + Vector3.up * Owner.verticalVelocity;
        Owner.controller.Move(move * Time.deltaTime);
    }

    public override void OnExit()
    {
        _jumped = false;
    }

    private void JumpAndGravity()
    {
        // jump timeout
        if (Owner.jumpTimeoutDelta >= 0.0f)
            Owner.jumpTimeoutDelta -= Time.deltaTime;
            
        // fall timeout
        if (Owner.fallTimeoutDelta >= 0.0f)
            Owner.fallTimeoutDelta -= Time.deltaTime;
            
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (Owner.verticalVelocity < Owner.terminalVelocity)
        {
            Owner.verticalVelocity += Owner.Gravity * Time.deltaTime;
        }
    }
    private void CheckStateTransitions()
    {
        // Return to locomotion state when grounded
        if (Owner.Grounded && Owner.verticalVelocity < 0f)
        {
            Owner.jumpTimeoutDelta = Owner.JumpTimeout;
            Owner.ChangeState(new LocomotionState(Owner));
            return;
        }

        // Fall trigger
        if (!Owner.Grounded && Owner.verticalVelocity <= 0f)
        {
            Owner.ChangeState(new FallState(Owner));
            return;
        }
    }
}