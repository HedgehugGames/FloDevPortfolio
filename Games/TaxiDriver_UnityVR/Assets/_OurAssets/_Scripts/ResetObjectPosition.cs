using Unity.VisualScripting;
using UnityEngine;

public class ResetObjectPosition : MonoBehaviour
{
    [SerializeField] private Transform resetPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Grabable"))
        {
            Debug.Log($"{other.name} is outside of the car");
            other.GameObject().transform.position = resetPosition.position;
            other.GameObject().transform.rotation = resetPosition.rotation;
        }
    }
}
