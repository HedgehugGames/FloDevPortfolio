using UnityEngine;
using StarterAssets;

public class FallState : PlayerBaseState
{
    private float _speed;
    private float _targetRotation;
    private float _rotationVelocity;
    private const float Threshold = 0.01f;
    
    public FallState(ThirdPersonController owner) : base(owner) { }

    public override void OnEnter()
    {
        if (Owner.hasAnimator)
            Animator.CrossFade(FallHash, CrossFadeDuration);

        Owner.fallTimeoutDelta = Owner.FallTimeout;
        Owner.verticalVelocity = Owner.verticalVelocity; 
    }

    public override void Update()
    {
         // Apply gravity
        if (Owner.verticalVelocity < Owner.terminalVelocity)
        {
            Owner.verticalVelocity += Owner.Gravity * Time.deltaTime;
        }

        // Apply movement input while falling
        if (Owner.input.move != Vector2.zero)
        {
            // move
            Vector3 targetDirection = Vector3.zero;

            // Note: get the right-facing vector of the reference transform
            // determine the direction the player intends to move in relative to the camera orientation
            targetDirection = Owner.mainCamera.transform.forward * Owner.input.move.y + Owner.mainCamera.transform.right * Owner.input.move.x;
            targetDirection.Normalize();

            Vector3 targetVelocity = targetDirection * Owner.MoveSpeed;

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (Owner.input.move == Vector2.zero)
            {
                targetVelocity = Vector3.zero;
            }

            float currentHorizontalSpeed = new Vector3(Owner.controller.velocity.x, 0.0f, Owner.controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = Owner.input.analogMovement ? Owner.input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < Owner.MoveSpeed - speedOffset ||
                currentHorizontalSpeed > Owner.MoveSpeed + speedOffset)
            {
                // creates slow and smooth speed change that smoothly reaches the target
                _speed = Mathf.Lerp(currentHorizontalSpeed, Owner.MoveSpeed * inputMagnitude,
                    Time.deltaTime * Owner.SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = Owner.MoveSpeed;
            }

            // normalise input direction to ensure magnitude is no more than 1
            Vector3 inputDirection = new Vector3(Owner.input.move.x, 0.0f, Owner.input.move.y).normalized;

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (Owner.input.move != Vector2.zero)
            {
                targetVelocity = Owner.transform.forward * _speed;
            }
            else
            {
                targetVelocity = Vector3.zero;
            }

            // move the player
            Owner.controller.Move(targetVelocity * Time.deltaTime + Vector3.up * Owner.verticalVelocity * Time.deltaTime);
        }
        else
        {
            Owner.controller.Move(Vector3.up * Owner.verticalVelocity * Time.deltaTime);
        }

        // Ground check
        if (Owner.Grounded)
        {
            // Reset fall timeout
            Owner.fallTimeoutDelta = Owner.FallTimeout;

            // Reset vertical velocity when grounded
            if (Owner.verticalVelocity < 0.0f)
            {
                Owner.verticalVelocity = -2f; // Stick the character to the ground slightly
            }

            // Transition back to locomotion
            Owner.ChangeState(new LocomotionState(Owner));
        }
        else
        {
            // Fall timeout
            if (Owner.fallTimeoutDelta >= 0.0f)
            {
                Owner.fallTimeoutDelta -= Time.deltaTime;
            }
        }
    }
    public override void OnExit()
    {
        Owner.verticalVelocity = Owner.verticalVelocity; 
    }
   
}
