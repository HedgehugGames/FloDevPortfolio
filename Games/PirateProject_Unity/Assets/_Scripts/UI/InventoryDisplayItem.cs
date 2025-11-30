using TMPro;
using UnityEngine;
using UnityEngine.UI;

// class to handle the visuals in the inventory
[System.Serializable]
public class InventoryDisplayItem
{
    public string item;    // Item key in inventory
    public string itemNameUI;  // Text label prefix (e.g., "Coins: ")
    public Image iconImage;    // Icon that shows the item
    public TextMeshProUGUI itemCountUI;    // Reference to the UI Text element
    public TextMeshProUGUI descriptionTextUI; // Reference for the description text
    public GameObject itemPanel;   // Reference to the whole item panel
}