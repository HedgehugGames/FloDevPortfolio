using StarterAssets;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public Transform itemsParent;
    public GameObject inventoryUI;
    public ThirdPersonController playerController;
    
    private InventoryManager _inventory;
    private InventorySlot[] _slots;
    private MenuController _menuController;
    
    
    [Header("Coins")]
    [SerializeField] private TextMeshProUGUI coinTextUI;
    
    void Start()
    {
        _inventory = InventoryManager.Instance;
        _inventory.onItemChangedCallback += UpdateUI;
        
        _slots = itemsParent.GetComponentsInChildren<InventorySlot>();
       _menuController = FindObjectOfType<MenuController>();
       _menuController.UpdateAllSlotStatuses(_menuController.saveSlots);
       _menuController.UpdateAllSlotStatuses(_menuController.loadSlots);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            _inventory.isInventoryOpen = !_inventory.isInventoryOpen;
            bool isActive = !inventoryUI.activeSelf;
            inventoryUI.SetActive(isActive);

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            
            playerController.enabled = !isActive;
        }
    }
    
    private void UpdateUI()
    {
        // Update Items UI
        for (int i = 0; i < _slots.Length; i++)
        {
            if (i < _inventory.items.Count)
            {
                _slots[i].AddItem(_inventory.items[i]);
            }
            else
            {
               _slots[i].ClearSlot(); 
            }
        }
        
        // Update Coins UI
        int coinCount = GameManager.Instance.totalCoins;
        coinTextUI.text = " " + coinCount;
    }
}
