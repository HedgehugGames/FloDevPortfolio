using System.Collections.Generic;
using System.Collections;
using System;
using StarterAssets;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<InventoryItem> items = new();
    public bool isPickingUp = false;
    public int space = 12;
    
    // drop item
    public Transform dropPoint;
    
    // equip item
    [SerializeField] private ThirdPersonController player;
    [HideInInspector] public ItemData equippedItem;
    [HideInInspector]public GameObject equippedInstance;

    public bool isInventoryOpen = false;
    public delegate void OnItemChanged();
    public OnItemChanged onItemChangedCallback;

    #region Singleton

    public static InventoryManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    public bool AddItem(ItemData itemData)
    {
        var existing = items.Find(i => i.data == itemData);

        if (existing != null && itemData.stackable)
        {
            existing.count += 1;
        }
        else
        {
            if (items.Count >= space)
            {
                Debug.Log("Inventory is full");
                return false;
            }

            existing = new InventoryItem { data = itemData, count = 1 };
            items.Add(existing);
        }

        // play sound
        if (!string.IsNullOrEmpty(itemData.sound))
            AudioManager.Instance.PlayItemSound(itemData.sound);

        // update UI
        if (onItemChangedCallback != null)
            onItemChangedCallback?.Invoke();

        ResetPickup();
        return true;
    }

    public void RemoveItem(InventoryItem item)
    {
        if (item == null || item.data == null) return;

        if (item.data.stackable)
        {
            item.count--;

            if (item.count <= 0)
                items.Remove(item);
        }
        else
        {
            items.Remove(item);
        }

        Instantiate(item.data.dropItemPrefab, dropPoint.position, Quaternion.identity);
        onItemChangedCallback?.Invoke();
    }

    //private void DropOf(InventoryItem item)
    //{
    //    // drop logic
    //    Transform player = GameObject.FindWithTag("Player").transform;
    //    Vector3 playerPos = player.position;
    //    Vector3 forward = player.forward;

    //    // Try dropping 1–2 meters in front of player, avoid collisions
    //    float dropDistance = 1.5f;
    //    float radius = 0.5f;
    //    Vector3 baseDropPos = playerPos + forward * dropDistance;

    //    // Find clear space using raycast to ground
    //    Vector3 dropPos = baseDropPos;
    //    if (Physics.CheckSphere(baseDropPos, radius))
    //    {
    //        // Try alternate directions
    //        Vector3[] directions = {
    //            Vector3.forward, Vector3.right, Vector3.left, Vector3.back,
    //            (Vector3.forward + Vector3.right).normalized,
    //            (Vector3.forward + Vector3.left).normalized,
    //            (Vector3.back + Vector3.right).normalized,
    //            (Vector3.back + Vector3.left).normalized
    //        };

    //        foreach (var dir in directions)
    //        {
    //            Vector3 testPos = playerPos + dir * dropDistance;
    //            if (!Physics.CheckSphere(testPos, radius))
    //            {
    //                dropPos = testPos;
    //                break;
    //            }
    //        }
    //    }

    //    // Make sure it's on the ground
    //    if (Physics.Raycast(dropPos + Vector3.up * 5f, Vector3.down, out RaycastHit hit, 10f))
    //    {
    //        dropPos = hit.point;
    //    }

    //    Instantiate(item.data.itemPrefab, dropPos, Quaternion.identity);
    //}

    public int GetInventoryItemCount(string itemName)
    {
        var item = items.Find(i => i.data != null && i.data.itemName == itemName);
        return item != null ? item.count : 0;
    }

    public void UseItem(ItemData itemData)
    {
        if (itemData != null)
        {
            Debug.Log("Use Item called");
            itemData.Use();
        }
    }

    public void EquipInventoryItem(ItemData itemData)
    {
        var existing = items.Find(i => i.data == itemData);
        
        if (existing != null)
        {
            Transform handTransform = player.rightHandTransform;

            // Remove any item already in the hand
            foreach (Transform child in handTransform)
            {
                if (equippedItem != null)
                {
                    AddItem(equippedItem);
                }
                Destroy(child.gameObject);
            }
            
            // Instantiate and attach the new item to the hand
            if (itemData.equipItemPrefab != null)
            {
                equippedInstance = Instantiate(itemData.equipItemPrefab, player.rightHandTransform);
                equippedInstance.transform.localPosition = Vector3.zero;
                equippedInstance.transform.localRotation = Quaternion.identity;

            }
            else
            {
                Debug.LogWarning("Item prefab is missing!");
            }

            // Remove it from inventory
            if (itemData.stackable)
            {
                equippedItem = existing.data;
                existing.count--;
                if (existing.count <= 0)
                    items.Remove(existing);
            }
            else
            {
                equippedItem = existing.data;
                items.Remove(existing);
            }
        }

        onItemChangedCallback?.Invoke();
    }
    private void Unequip(Transform handTransform)
    {
        foreach (Transform child in handTransform)
        {
            InventoryItem item = child.GetComponent<InventoryItem>();
            if (item != null)
            {
                AddItem(item.data); // return item to inventory
            }
            Destroy(child.gameObject); // remove from hand
        }
        
        // clear equipped reference
        equippedItem = null; 
        
        onItemChangedCallback?.Invoke(); 
    }

    public void ResetPickup()
        {
            StartCoroutine(ResetPickupDelay());
        }

        private IEnumerator ResetPickupDelay()
        {
            yield return new WaitForSeconds(0.1f);
            isPickingUp = false;
        }

        public void ResetInventory()
        {
            items.Clear();
            PlayerPrefs.DeleteKey("InventoryData");
            onItemChangedCallback?.Invoke();
        }
    }