using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using GameEvents;

public class PatrolTriggerZone : MonoBehaviour
{
    public List<WayPointEnemyMovement> enemiesInZone;
    public UIPatrolZone uiAlert;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiAlert.ShowAlert();
            foreach (var patrolEvent in enemiesInZone.Select(ai => new PatrolEvent { AI = ai, PlayerEnterTrigger = true }))
            {
                GameEventManager.Raise(patrolEvent);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            uiAlert.HideAlert();
            foreach (var patrolEvent in enemiesInZone.Select(ai => new PatrolEvent { AI = ai, PlayerEnterTrigger = false }))
            {
                GameEventManager.Raise(patrolEvent);
            }
        }
    }
}