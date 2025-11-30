using UnityEngine;

public class InfiniteRoad : MonoBehaviour
{
   [SerializeField] private GameObject[] roads;
   [SerializeField] private Transform endPoint;
   [SerializeField] private Transform buffer;
   
   [SerializeField] private float roadLength = 100f;

   private int nextRoadIndex = 0;

   void Update()
   {
      Respawn();
   }
   
   private void Respawn()
   {
      // if the endpoint of a road gameobject is past the player transform on the z position, spawn a new gameobject at the endpoint of the last go in the array. if the same gameobject is past the bugffer position as well despawn it
      // make use of object poling
      
      foreach (var road in roads)
      {
         // Despawn old roads that passed behind buffer
         if (road.transform.position.z < buffer.position.z)
         {
            // Move it to the front
            GameObject nextRoad = roads[nextRoadIndex];
            float newZ = endPoint.position.z + roadLength;

            road.transform.position = new Vector3(road.transform.position.x, road.transform.position.y, newZ);

            // Update endPoint for next spawn
            endPoint = road.transform;

            // Advance index in circular fashion
            nextRoadIndex = (nextRoadIndex + 1) % roads.Length;
         }
      }
   } 
   
}