using UnityEngine;

[CreateAssetMenu(fileName = "New Ship", menuName = "Ships/ShipData")]
public class ShipData : ScriptableObject
{
    public string shipName;
    public float speed;
    public float turnSpeed; 
    //public float health;
    // public Sprite shipIcon;
}