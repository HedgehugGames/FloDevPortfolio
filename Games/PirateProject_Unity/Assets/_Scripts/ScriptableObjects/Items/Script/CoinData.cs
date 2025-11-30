using UnityEngine;

[CreateAssetMenu(fileName = "CoinData", menuName = "Inventory/Coin")]
public class CoinData : ScriptableObject
{
    public int coinValue = 10; // amount added per coin pickup
    public string sound;
}