using StarterAssets;
using UnityEngine;

[CreateAssetMenu(fileName = "Sword", menuName = "Inventory/Items/Sword")]
public class Sword : ItemData
{
    private ThirdPersonController player;
    public override void Use()
    {
        Debug.Log("Sword Use");
        
        player = GameObject.FindObjectOfType<ThirdPersonController>();
        player.ChangeState(new AttackState(player));
    }
}
