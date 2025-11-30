using UnityEngine;
using System.Collections;
using Tools;
using GameEvents;
using System.Collections.Generic;

public class GameManager : PersistentSingleton<GameManager>
{
    public int totalCoins = 0;
    protected virtual void OnEnable()
    {
        GameEventManager.AddListener<OpenChestEvent>(OnOpenChestEvent);
        GameEventManager.AddListener<ShipControlEvent>(OnShipControl);
        GameEventManager.AddListener<PatrolEvent>(OnPatrolEvent);
        GameEventManager.AddListener<UnderwaterEvent>(OnUnderwaterEvent);
        GameEventManager.AddListener<CollectCoinEvent>(OnCollectCoin);
    }
    protected virtual void OnDisable()
    {
        GameEventManager.RemoveListener<OpenChestEvent>(OnOpenChestEvent);
        GameEventManager.RemoveListener<ShipControlEvent>(OnShipControl);
        GameEventManager.RemoveListener<PatrolEvent>(OnPatrolEvent);
        GameEventManager.RemoveListener<UnderwaterEvent>(OnUnderwaterEvent);
        GameEventManager.RemoveListener<CollectCoinEvent>(OnCollectCoin);
    }
    //COINS_________________________________________________________________________________________
    private void OnCollectCoin(CollectCoinEvent e)
    {
        totalCoins += e.Amount;
        Debug.Log($"Collected coins: total now {totalCoins}");
        InventoryManager.Instance.onItemChangedCallback?.Invoke();
    }
    //CHEST__________________________________________________________________________________________
    private static void OnOpenChestEvent(OpenChestEvent e)
    {
        if (e.IsOpening)
        {
            e.ChestScript.ChestOpening();
        }
    }

    //SHIP__________________________________________________________________________________________
    private static void OnShipControl(ShipControlEvent e)
    {
        if (e.IsControllingShip)
        {
            Debug.Log("Player now controls the ship.");
            // Assuming 'e.ship' has the ToggleShipControl method
            e.Ship.EnableControl();
        }
        else
        {
            Debug.Log("Player left the ship.");
            e.Ship.DisableControl();
        }
    }
    
    //ENEMY PATROL___________________________________________________________________________________
    private void OnPatrolEvent(PatrolEvent e)
    {
        if (e.PlayerEnterTrigger)
        {
            e.AI.StartPatrol();
        }
        else if (!e.PlayerEnterTrigger)
        {
            Debug.Log("Player left the Trigger Zone.");
            e.AI.Idle();
        }
    }
// Underwater Effect_______________________________________________________________________________
    private void OnUnderwaterEvent(UnderwaterEvent e)
    {
        if (e.IsUnderwater)
        {
            e.Water.SetUnderwater(e.IsUnderwater);
        }

        else if (!e.IsUnderwater)
        {
            e.Water.NotUnderwater(!e.IsUnderwater);
        }
    }
    // Interact______________________________________________________________________________________
    public static void OnInteract()
    {
        Debug.Log("OnInteract called");
        Interactable[] interactables = FindObjectsOfType<Interactable>();
        Interactable nearest = null;
        float minDist = float.MaxValue;
        Vector3 playerPos = Camera.main.transform.position;

        foreach (var interactable in interactables)
        {
            if (!interactable.IsPlayerInRange()) continue;

            float dist = Vector3.Distance(playerPos, interactable.transform.position);
            Debug.Log($"Found {interactable.name} in range | Distance: {dist}");

            if (dist < minDist)
            {
                minDist = dist;
                nearest = interactable;
            }
        }

        if (nearest != null)
        {
            Debug.Log($"Interacting with {nearest.name}");
            nearest.Interact();
        }
        else
        {
            Debug.Log("No interactables in trigger range");
        }
    }
}
