using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StarterAssets;
using TMPro;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject menuPanel;
    public GameObject saveSlots;
    public GameObject loadSlots;
    [SerializeField] private int selectedSaveSlot = 1; // default to slot 1
    
    private GameData _data;
    
    [SerializeField] private TextMeshProUGUI saveConfirmationText;
    [SerializeField] private float confirmationDuration = 2f;

    [SerializeField] private GameObject player;
    [SerializeField] private List<ItemData> itemData;

    [SerializeField] private string emptySlotText;
    [SerializeField] private string savedSlotText;
    
    public bool isMenuOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            bool isActive = !menuPanel.activeSelf;
            menuPanel.SetActive(isActive);

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
            
            player.GetComponent<ThirdPersonController>().enabled = !isActive;
            
            isMenuOpen = !isMenuOpen;
        }
    }
    public void SetSaveSlot(int slot)
    {
        selectedSaveSlot = slot;
    }
    public void OnSaveSlotClicked(int slot)
    {
      SaveManager.SaveData(player, slot);
      UpdateAllSlotStatuses(saveSlots);
      UpdateAllSlotStatuses(loadSlots);
      StartCoroutine(ShowConfirmationRoutine());
    }
    private IEnumerator ShowConfirmationRoutine()
    {
        saveConfirmationText.gameObject.SetActive(true);
        yield return new WaitForSeconds(confirmationDuration);
        saveConfirmationText.gameObject.SetActive(false);
    }

    public void OnLoadSlotClicked(int slot)
    {
        GameData data = SaveManager.Load(slot);
        if (data == null) return;
        
        // Restore inventory
        InventoryManager.Instance.items.Clear();

        for (int i = 0; i < data.itemNames.Count; i++)
        {
            ItemData foundItem = itemData.Find(item => item.itemName == data.itemNames[i]);
            if (foundItem != null)
            {
                InventoryItem item = new InventoryItem
                {
                    data = foundItem,
                    count = data.itemCounts[i]
                };
                InventoryManager.Instance.items.Add(item);
            }
            else
            {
                Debug.LogWarning($"ItemData not found for: {data.itemNames[i]}");
            }
        }
        foreach (var item in itemData)
        {
            if (item != null)
                Debug.Log($"Item in database: {item.itemName}");
            else
                Debug.LogWarning("Null item in item database!");
        }

        InventoryManager.Instance.onItemChangedCallback?.Invoke();

        // Restore coins
        GameManager.Instance.totalCoins = data.totalCoins;

        // Restore player position
        if (!player.gameObject.activeInHierarchy)
            player.gameObject.SetActive(true);

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            player.transform.eulerAngles = new Vector3(data.rotation[0], data.rotation[1], data.rotation[2]);
            cc.enabled = true;
        }
        else
        {
            player.transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
            player.transform.eulerAngles = new Vector3(data.rotation[0], data.rotation[1], data.rotation[2]);
        }
    }
    public void UpdateAllSlotStatuses(GameObject slotsParent)
    {
        for (int i = 1; i <= slotsParent.transform.childCount; i++)
        {
            Transform slotTransform = slotsParent.transform.GetChild(i - 1); // zero-based index
            TextMeshProUGUI statusText = slotTransform.GetComponentInChildren<TextMeshProUGUI>();
            if (statusText != null)
            {
                string path = SaveManager.GetPath(i);
                if (File.Exists(path))
                {
                    statusText.text = savedSlotText;
                }
                else
                {
                    statusText.text = emptySlotText;
                }
            }
            else
            {
                Debug.LogWarning($"No TextMeshProUGUI found in slot {i}");
            }
        }
    }
    
    public void OnSaveClicked()
    {
        bool isActive = !saveSlots.activeSelf;
        saveSlots.SetActive(isActive);
        loadSlots.SetActive(false);
        
    }
    public void OnLoadClicked()
    {
        bool isActive = !loadSlots.activeSelf;
        loadSlots.SetActive(isActive);
        saveSlots.SetActive(false);
    }
    public void OnNewGameClicked()
    {
        // Reset coins
        GameManager.Instance.totalCoins = 0;
        
        // Reset inventory
        InventoryManager.Instance.ResetInventory();
        
        //// Reset player position (example: your start position)
        //player.transform.position = startPosition; // define startPosition as a Vector3
        //player.transform.rotation = Quaternion.identity;

        // Optionally delete save files
        for (int i = 1; i <= 3; i++)
        {
            string path = SaveManager.GetPath(i);
            if (File.Exists(path))
                File.Delete(path);
        }
        
        UpdateAllSlotStatuses(saveSlots);
        UpdateAllSlotStatuses(loadSlots);
        
        Debug.Log("New Game");
    }

    public void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop play mode in editor
#else
    Application.Quit(); // Quit the built game
#endif
    }

    
}