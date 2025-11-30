using UnityEngine;
using StarterAssets;

public class UnderwaterLoco : PlayerBaseState
{
    private float _swimSpeed = 3.0f;
    private float _vertical;

    private float _animationBlend;
    private float _targetRotation;
    private float _rotationVelocity;

    private const float VerticalSwimSpeed = 2.0f;
    private const float NewYPos = -4.0f;

    private bool _isSwimming = false;

    public UnderwaterLoco(ThirdPersonController owner) : base(owner) { }

    public override void OnEnter()
    {
        _isSwimming = false;

        var pos = Owner.transform.position;
        pos.y = NewYPos;
        Owner.transform.position = pos;
        
        Animator.CrossFade(UnderwaterHash, CrossFadeDuration);
    }

    public override void Update()
    {
        DiveMovement();

        if (!_isSwimming && Owner.transform.position.y >= -3.5f) _isSwimming = true;

        if (_isSwimming) Owner.ChangeState(new SwimLoco(Owner));
    }

    private void DiveMovement()
    {
        var input = Owner.input.move;
        var analogMovement = Owner.input.analogMovement;
        var sprinting = Owner.input.sprint;
        var cam = Owner.mainCamera.transform;
        var controller = Owner.controller;

        var targetSpeed = sprinting ? Owner.SprintSpeed : Owner.MoveSpeed;
        if (input == Vector2.zero) targetSpeed = 0f;

        var currentSpeed = new Vector3(controller.velocity.x, 0, controller.velocity.z).magnitude;
        var speedOffset = 0.1f;
        var inputMagnitude = analogMovement ? input.magnitude : 1f;

        if (currentSpeed < targetSpeed - speedOffset || currentSpeed > targetSpeed + speedOffset)
        {
            _swimSpeed = Mathf.Lerp(currentSpeed, targetSpeed * inputMagnitude, Time.deltaTime * Owner.SpeedChangeRate);
            _swimSpeed = Mathf.Round(_swimSpeed * 1000f) / 1000f;
        }
        else
        {
            _swimSpeed = targetSpeed;
        }

        float targetVertical = 0f;
        if (Owner.input.swimUp) targetVertical = 1f;
        if (Owner.input.dive) targetVertical = -1f;

        _vertical = Mathf.MoveTowards(_vertical, targetVertical, Time.deltaTime * 2f);
        Animator.SetFloat("VerticalSwim", _vertical); // Use smoothed value here

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * Owner.SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        var inputDir = new Vector3(input.x, 0, input.y).normalized;

        if (input != Vector2.zero)
        {
            _targetRotation = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            var smoothRotation = Mathf.SmoothDampAngle(Owner.transform.eulerAngles.y, _targetRotation,
                ref _rotationVelocity, Owner.RotationSmoothTime);
            Owner.transform.rotation = Quaternion.Euler(0f, smoothRotation, 0f);
        }

        var moveDir = Quaternion.Euler(0, _targetRotation, 0) * Vector3.forward;
        var diveMovement = moveDir.normalized * (_swimSpeed * Time.deltaTime);

        // vertical movement (Q = down, E = up)
        diveMovement.y = targetVertical * VerticalSwimSpeed * Time.deltaTime;

        controller.Move(diveMovement);

        Animator.SetFloat(DiveSpeedHash, _animationBlend);
        Animator.SetFloat(MotionSpeedHash, inputMagnitude);
    }
    
    public override void OnExit()
    {
    }
}