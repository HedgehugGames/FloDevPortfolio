using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Player
    public float[] position;
    public float[] rotation;

    // Inventory
    public List<string> itemNames = new List<string>();
    public List<int> itemCounts = new List<int>();
    public int totalCoins;
    
    // Constructor
    public GameData(GameObject player, List<InventoryItem> items)
    {
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;
        
        rotation = new float[3];
        rotation[0] = player.transform.eulerAngles.x;
        rotation[1] = player.transform.eulerAngles.y;
        rotation[2] = player.transform.eulerAngles.z;


        foreach (var i in items)
        {
            itemNames.Add(i.data.itemName);
            itemCounts.Add(i.count);
        }
        
        totalCoins = GameManager.Instance.totalCoins;
    }
}
