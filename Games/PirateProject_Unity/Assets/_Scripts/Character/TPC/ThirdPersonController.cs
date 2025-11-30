 using UnityEngine;
 using UnityEngine.EventSystems;

 using Unity.VisualScripting;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
    
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        [HideInInspector] public float verticalVelocity;
        [HideInInspector] public float terminalVelocity = 53.0f;
        [HideInInspector] public float jumpTimeoutDelta;
        [HideInInspector] public float fallTimeoutDelta;
        [HideInInspector] public bool isLanding = false;
        private BaseState<ThirdPersonController> _currentState;
        [HideInInspector] public bool hasAnimator;
        [HideInInspector] public bool jump;
        
        // in water
        private Underwater _underwaterChecker;
        [HideInInspector] public bool isInWater = false;
        [SerializeField] private Renderer waterTileRenderer;
        [HideInInspector] public float waveSpeed;
        [HideInInspector] public float waveFrequency;
        [HideInInspector] public float waveHeight;
        
        // inventory
        [HideInInspector] public InventoryManager inventory;
        //[SerializeField] private ItemData item;
        
        public Transform rightHandTransform;

         //animation IDs
        private int _animIDGrounded;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        [HideInInspector] public CharacterController controller;
        [HideInInspector]public StarterAssetsInputs input;
        [HideInInspector] public GameObject mainCamera;

        private const float Threshold = 0.01f;
        
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }
//________________________________________________________________________________________
//________________________________________________________________________________________
        private void Awake()
        {
            // get a reference to our main camera
            if (mainCamera == null)
            {
                mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            
            _underwaterChecker = GameObject.Find("WaterManager").GetComponent<Underwater>();
            
        }
        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            hasAnimator = TryGetComponent(out _animator);
            controller = GetComponent<CharacterController>();
            input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            AssignAnimationIDs();
            
            inventory = Object.FindObjectOfType<InventoryManager>(); 
            
            ChangeState(new LocomotionState(this));
            
            if (waterTileRenderer != null)
            {
                Material mat = waterTileRenderer.sharedMaterial;
                waveSpeed = mat.GetFloat("_WaveSpeed");
                waveFrequency = mat.GetFloat("_WaveFrequency");
                waveHeight = mat.GetFloat("_WaveHeight");
            }
        }

        private void Update()
        {
            _currentState?.Update(); 
            GroundedCheck();
            

            if (Input.GetKey(KeyCode.Q))
            {
                input.dive = true;
            }
            else input.dive = false;

            if (Input.GetKey(KeyCode.Space))
            {
                input.swimUp = true;
            }
            else input.swimUp = false;
            
            isInWater = _underwaterChecker.inWater;

          //if (Input.GetKey(KeyCode.U))
          //{
          //    InventoryManager.Instance.UnequipAll();
          //}
        }
        
        private void LateUpdate()
        {
            CameraRotation();
        }
        
        private void AssignAnimationIDs()
        {
            _animIDGrounded = Animator.StringToHash("Grounded");
        }

        public void ChangeState(BaseState<ThirdPersonController> newState)
        {
            _currentState?.OnExit();
            _currentState = newState;
            _currentState.OnEnter();
        }
        
        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (input.look.sqrMagnitude >= Threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        } 
        
        // Animation Events
        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (!isInWater)
            {
                if (animationEvent.animatorClipInfo.weight > 0.5f)
                {
                    if (FootstepAudioClips.Length > 0)
                    {
                        var index = Random.Range(0, FootstepAudioClips.Length);
                        AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(controller.center), FootstepAudioVolume);
                    }
                }
            }
            else return;
        }
        private void OnLand(AnimationEvent animationEvent)
        {
            if (!isInWater)
            {
                if (animationEvent.animatorClipInfo.weight > 0.5f)
                {
                    AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(controller.center),
                        FootstepAudioVolume);
                }
            }
            else return;
        }
        private void ExecuteEvent()
        {
            if (isInWater){}
        }
    }
}