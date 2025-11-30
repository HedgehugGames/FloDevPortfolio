using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "Bottle", menuName = "Inventory/Items/Bottle")]
public class Bottle : ItemData
{
    public float drunkTime;

    public override void Use()
    {
        Debug.Log("Using Bottle");
        
        DrunkEffect drunk = GameObject.FindObjectOfType<DrunkEffect>();
        if (drunk != null)
        {
            drunk.TriggerDrunk(drunkTime);
            var player = GameObject.FindObjectOfType<ThirdPersonController>();
            player.ChangeState(new DrinkingState(player));
        }
    }
    
}
