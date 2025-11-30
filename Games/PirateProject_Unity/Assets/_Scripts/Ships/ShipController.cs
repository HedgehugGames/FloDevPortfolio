using UnityEngine;
using GameEvents;
using StarterAssets;
using Unity.Cinemachine;
using UnityEngine.UI;

public class ShipController : Interactable
{
    
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerTargetPosition;
    private Animator _anim;
    
    // wave movement
    [SerializeField] private Renderer waterTileRenderer;
    private float _lastWaveOffset = 0f;
    private float _waveSpeed;
    private float _waveFrequency;
    private float _waveHeight;
    private ShipData _shipData;
    
    //[SerializeField] private Transform mainCamera;
    [SerializeField] private CinemachineCamera maincamera;
    [SerializeField] private Transform shipCameraTarget; 
    
    private OutlineController _outline; 
    
    private bool _isControlling = false;

    private void Awake()
    {
        _anim = player.GetComponent<Animator>();
        GetComponent<ShipMovement>().enabled = false;
        _outline = GetComponent<OutlineController>();
    }

    private void Start()
    {
        if (waterTileRenderer != null)
        {
            Material mat = waterTileRenderer.sharedMaterial;
            _waveSpeed = mat.GetFloat("_WaveSpeed");
            _waveFrequency = mat.GetFloat("_WaveFrequency");
            _waveHeight = mat.GetFloat("_WaveHeight");
        }
    }

    public override void Interact()
    {
        if (playerInRange)
        {
            _isControlling = !_isControlling;
            GameEventManager.Raise(new ShipControlEvent(this, _isControlling));
        }
        WaveMovement();
    }

    public void EnableControl() // enable control of ship
    {
        // disable the outline of the wheel
        _outline.DisableOutline();
        
        player.GetComponent<ThirdPersonController>().enabled = false;
        _anim.SetFloat("Speed", 0f); 
        _anim.GetComponent<Animator>().SetFloat("MotionSpeed", 0f);
        _anim.SetFloat("SwimSpeed", 0f);
        _anim.SetLayerWeight(1, 0);
        GetComponent<ShipMovement>().enabled = true;
        
        // set position and rotation of the player to the position on the ship
        player.transform.position = playerTargetPosition.position;
        player.transform.rotation = playerTargetPosition.rotation;
        
        Transform playerCameraTarget = player.Find("PlayerCameraRoot"); // your player’s follow target
        maincamera.Follow = shipCameraTarget;
        maincamera.LookAt = shipCameraTarget;

        //move player with ship
        player.SetParent(transform); 
    }

    public void DisableControl() // disable control of ship
    {
        player.GetComponent<ThirdPersonController>().enabled = true;
        GetComponent<ShipMovement>().enabled = false;
        
        Transform playerCameraTarget = player.Find("PlayerCameraRoot");
        maincamera.Follow = playerCameraTarget;
        maincamera.LookAt = playerCameraTarget;

        player.SetParent(null); // detach player from ship
    }

    private void WaveMovement()
    {
        float currentWaveOffset = Mathf.Sin(transform.position.x * _waveFrequency + Time.time * _waveSpeed) * _waveHeight;
        float waveDelta = currentWaveOffset - _lastWaveOffset;
        _lastWaveOffset = currentWaveOffset;

        // Apply wave offset to current position
        Vector3 pos = transform.position;
        pos.y += waveDelta;
        transform.position = pos;
    }
}