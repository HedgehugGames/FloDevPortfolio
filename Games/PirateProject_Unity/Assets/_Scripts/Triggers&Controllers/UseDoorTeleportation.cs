using Unity.VisualScripting;
using UnityEngine;

public class UseDoorTeleportation : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private Transform destination;

    private void OnTriggerEnter(Collider other)
    {
      
        
        if (other.CompareTag("Player"))
        {
            player.transform.position = destination.position;
            player.transform.rotation = destination.rotation;
        }
    }
}
