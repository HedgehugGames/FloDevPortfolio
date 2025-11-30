using System.Collections.Generic;

[System.Serializable]
public class InventorySaveData
{
    public List<string> itemNames = new List<string>();
    public List<int> itemCounts = new List<int>();
}