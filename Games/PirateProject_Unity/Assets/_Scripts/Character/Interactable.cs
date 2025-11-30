using UnityEngine;

public class Interactable : MonoBehaviour
{
   public float radius = 3f;
   protected bool playerInRange;

   public virtual void Interact()
   {
      Debug.Log("Base Interact called");
   }
   public bool IsPlayerInRange() { return playerInRange; }
   
   private void OnTriggerEnter(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         playerInRange = true;
         GetComponent<OutlineController>()?.EnableOutline();
      }
   }

   private void OnTriggerExit(Collider other)
   {
      if (other.CompareTag("Player"))
      {
         playerInRange = false;
         GetComponent<OutlineController>()?.DisableOutline();
      }
   }
}
