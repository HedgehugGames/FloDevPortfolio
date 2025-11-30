using UnityEngine;
using StarterAssets;

public class LocomotionState : PlayerBaseState {
    
    private float _speed;
    private float _animationBlend;
    private float _targetRotation;
    private float _rotationVelocity;
    
    public LocomotionState(ThirdPersonController owner) : base(owner) { }
    
    public override void OnEnter()
    {
        //Owner.GetComponent<ThirdPersonController>().enabled = true;
        Animator.SetLayerWeight(1, 0f);
        Animator.CrossFade(LocomotionHash, CrossFadeDuration);
        
    }

    public override void OnExit()
    { }

    public override void Update()
    {
        // Reduce timeout 
        if (Owner.jumpTimeoutDelta > 0)
            Owner.jumpTimeoutDelta -= Time.deltaTime;
        
        if (Owner.isLanding || Owner.jumpTimeoutDelta > 0f)
            return;
        
        if (Owner.input.jump && Owner.Grounded && Owner.jumpTimeoutDelta <= 0f)
        {
            Owner.ChangeState(new JumpState(Owner));
            return;
        }

        if (!Owner.Grounded)
        {
            Owner.ChangeState(new FallState(Owner));
            return;
        }
        
        if (Owner.isInWater)
        {
            Owner.ChangeState(new SwimLoco(Owner));
            return;;
        }

        HandleMovement();
    }
    
    private void HandleMovement()
    {
        var input = Owner.input.move;
        var analogMovement = Owner.input.analogMovement;
        var sprinting = Owner.input.sprint;
        var cam = Owner.mainCamera.transform;
        var controller = Owner.controller;

        float targetSpeed = sprinting ? Owner.SprintSpeed : Owner.MoveSpeed;
        if (input == Vector2.zero) targetSpeed = 0f;

        float currentSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = analogMovement ? input.magnitude : 1f;

        // Smooth speed
        if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentSpeed, targetSpeed * inputMagnitude, Time.deltaTime * Owner.SpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * Owner.SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // Direction and rotation
        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;

        if (input != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float smoothRotation = Mathf.SmoothDampAngle(
                Owner.transform.eulerAngles.y,
                _targetRotation,
                ref _rotationVelocity,
                Owner.RotationSmoothTime
            );
            Owner.transform.rotation = Quaternion.Euler(0, smoothRotation, 0);
        }

        Vector3 moveDir = Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;

        // Apply movement
        controller.Move(
            moveDir.normalized * (_speed * Time.deltaTime) +
            new Vector3(0, Owner.verticalVelocity, 0) * Time.deltaTime
        );

        // Animator parameters
        Animator.SetFloat(SpeedHash, _animationBlend);
        Animator.SetFloat(MotionSpeedHash, inputMagnitude);
    }
   
}
