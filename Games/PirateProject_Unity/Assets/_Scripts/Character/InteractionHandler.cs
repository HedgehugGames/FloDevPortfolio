using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    private MenuController menu;
    
    public void OnInteract()  // E
    {
           //Debug.Log("Interact called");
           GameManager.OnInteract();
    }

    public void OnUse()   // F / Left Mouse Click
    {
        menu = FindObjectOfType<MenuController>();
        if (InventoryManager.Instance.isInventoryOpen || menu.isMenuOpen) return;
        
        //Debug.Log("Use called");
        InventoryManager.Instance.UseItem(InventoryManager.Instance.equippedItem);
        
    }
}