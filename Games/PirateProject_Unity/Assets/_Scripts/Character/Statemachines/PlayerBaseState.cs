using StarterAssets;
using UnityEngine;

public class PlayerBaseState: BaseState<ThirdPersonController>
{
    protected static readonly int LocomotionHash = Animator.StringToHash("Locomotion");
    protected static readonly int SpeedHash = Animator.StringToHash("Speed");
    protected static readonly int DiveSpeedHash = Animator.StringToHash("DiveSpeed");
    protected static readonly int MotionSpeedHash = Animator.StringToHash("MotionSpeed");
    protected static readonly int SwimSpeedHash = Animator.StringToHash("SwimSpeed");
    protected static readonly int JumpHash = Animator.StringToHash("Jump");
    protected static readonly int FallHash = Animator.StringToHash("Fall");
    protected static readonly int AttackHash = Animator.StringToHash("Attack");
    protected static readonly int DrinkHash = Animator.StringToHash("Drink");
    protected static readonly int SwimHash = Animator.StringToHash("SwimLocomotion");
    protected static readonly int DiveHash = Animator.StringToHash("ShallowDiveStart");
    protected static readonly int UnderwaterHash = Animator.StringToHash("UnderwaterLoco");
    protected static readonly int DrowningHash = Animator.StringToHash("Drowning");
    protected static readonly int SwimExitHash = Animator.StringToHash("SwimExit");
    
    protected PlayerBaseState(ThirdPersonController owner):base(owner){}
}
