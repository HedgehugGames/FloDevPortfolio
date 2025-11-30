using UnityEngine;
using GameEvents;
using UnityEngine.Serialization;

public class OpenChest : Interactable
{
    [SerializeField] private GameObject lid;
    [SerializeField] private GameObject treasure;
    [SerializeField] private float lidAngle = -45.0f;

    public override void Interact()
    {
        
        GameEventManager.Raise(new OpenChestEvent(this, true));
    }
    public void ChestOpening()
    {
        SetRotation(lidAngle);

        // Activate items in the chest
        if (treasure != null)
            treasure.gameObject.SetActive(true);

        // only make it visual, that it is only interactable once
        GetComponent<OutlineController>().DisableOutline();
    }

    private void SetRotation(float rot)
    {
        lid.GetComponent<Transform>().transform.localRotation = Quaternion.Euler(rot, 0, 0);
    }
}
