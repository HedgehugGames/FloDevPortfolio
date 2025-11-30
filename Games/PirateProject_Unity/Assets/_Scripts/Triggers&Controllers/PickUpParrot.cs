using UnityEngine;

public class PickUpParrot : Interactable
{
    [SerializeField] private GameObject parrotPrefab;
    [SerializeField] private Transform shoulderAttachPoint;

    public override void Interact()
    {
        GameObject parrot = Instantiate(parrotPrefab, shoulderAttachPoint.position, shoulderAttachPoint.rotation);
        parrot.transform.SetParent(shoulderAttachPoint);
        
        Destroy(gameObject);
    }
}