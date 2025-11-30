using UnityEngine;
using GameEvents;
using System;

public class Collectible : Interactable
{
    [SerializeField] private ItemData itemData;  // make it a list or something that can hold item data and coin data
    [SerializeField] private CoinData coinData;
    private InventoryManager _invManager;

    private void Start()
    {
        _invManager = InventoryManager.Instance;
    }

    public override void Interact()
    {
        if (playerInRange && !_invManager.isPickingUp)
        {
            //Debug.Log("Interact called on Item");
            
            _invManager.isPickingUp = true;

            if (coinData != null)
            {
                GameEventManager.Raise(new CollectCoinEvent(coinData.coinValue));
                AudioManager.Instance.PlayItemSound(coinData.name);
                
                _invManager.ResetPickup();
                Destroy(gameObject);
            }
            else if (itemData != null)
            {
                bool wasPickedUp = _invManager.AddItem(itemData);
                GameEventManager.Raise(new CollectItemEvent(itemData.itemName));
                
                _invManager.ResetPickup();
                if (wasPickedUp)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}