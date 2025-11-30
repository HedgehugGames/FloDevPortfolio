using System.Security;
using UnityEngine;
using StarterAssets;

public class SwimLoco : PlayerBaseState
{
    private float _swimSpeed;
    private float _waterDrag = 2f;
    
    private float _animationBlend;
    private float _targetRotation;
    private float _rotationVelocity;
    
    private float _lastWaveOffset = 0f;
    
    public SwimLoco(ThirdPersonController owner) : base(owner) { }

    public override void OnEnter()
    {
        Animator.CrossFade(SwimHash, CrossFadeDuration);
        Animator.SetLayerWeight(1, 1f);
        Animator.SetFloat(SpeedHash, 0f);
    }

    public override void OnExit()
    { }
    public override void Update()
    {
        HandleSwimming();

        if (Owner.Grounded && !Owner.isInWater)
        {
            Owner.ChangeState(new LocomotionState(Owner));
        }
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            Owner.ChangeState(new UnderwaterLoco(Owner));
        }
    }
    private void HandleSwimming()
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

        // Smooth swimming speed
        if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _swimSpeed = Mathf.Lerp(currentSpeed, targetSpeed * inputMagnitude, Time.deltaTime * Owner.SpeedChangeRate);
            _swimSpeed = Mathf.Round(_swimSpeed * 1000f) / 1000f;
        }
        else
        {
            _swimSpeed = targetSpeed;
        }

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * Owner.SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // Calculate direction
        Vector3 inputDir = new Vector3(input.x, 0, input.y).normalized;

        if (input != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float smoothRotation = Mathf.SmoothDampAngle(Owner.transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, Owner.RotationSmoothTime);
            Owner.transform.rotation = Quaternion.Euler(0f, smoothRotation, 0f);
        }

        Vector3 moveDir = Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;

        // Lock Y position or apply slight vertical buoyancy if desired
        Vector3 surfaceMovement = moveDir.normalized * (_swimSpeed * Time.deltaTime);
        
        // Calculate wave offset based on player's X position and time, matching shader
        float currentWaveOffset = Mathf.Sin(Owner.transform.position.x * Owner.waveFrequency + Time.time * Owner.waveSpeed) * Owner.waveHeight;
        float waveDelta = currentWaveOffset - _lastWaveOffset;
        _lastWaveOffset = currentWaveOffset;

        // Add Y change based on delta
        surfaceMovement.y += waveDelta;
        controller.Move(surfaceMovement); 
        
        // Update animator
        Animator.SetFloat(SwimSpeedHash, _animationBlend);
        Animator.SetFloat(MotionSpeedHash, inputMagnitude);
        
    }
    
}
