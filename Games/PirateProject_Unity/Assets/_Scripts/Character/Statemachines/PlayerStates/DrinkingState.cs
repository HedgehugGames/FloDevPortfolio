using StarterAssets;
using UnityEngine;

public class DrinkingState : PlayerBaseState
{

    private bool _isDrinking = false;
    private float _drinkingTimer = 0f;
    private const float DrinkingDuration = 3.0f;
    public DrinkingState(ThirdPersonController owner) : base(owner) { }

    public override void OnEnter()
    {
        Animator.CrossFade(DrinkHash, CrossFadeDuration);
        _isDrinking = true;
        _drinkingTimer = 0f;
    }
    public override void Update()
    {
        if (_isDrinking)
        {
            _drinkingTimer += Time.deltaTime;
            if (_drinkingTimer >= DrinkingDuration)
            {
                _isDrinking = false;
                Owner.ChangeState(new LocomotionState(Owner));
            }
        }
    }

    public override void OnExit()
    {
        _isDrinking = false;
        
        // destroy bottle after using
        if (InventoryManager.Instance.equippedInstance != null)
        {
            GameObject.Destroy(InventoryManager.Instance.equippedInstance);
            InventoryManager.Instance.equippedInstance = null;
        }
        
        // allow movement again
        Owner.GetComponent<ThirdPersonController>().enabled = true;
    }
    
    
    // get drinking duration from the animation clip
}
