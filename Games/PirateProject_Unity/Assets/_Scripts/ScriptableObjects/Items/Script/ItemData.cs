using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName; // name of the item
    public string sound;    // sound played when it is collected
    public Sprite icon; // icon to display the item in the inventory
    public GameObject dropItemPrefab;   // item prefab to drop
    public GameObject equipItemPrefab;  // item prefab for equipping
    public bool stackable;
    [TextArea] public string description = " ";   // description for the item

    public virtual void Use()
    {
        Debug.Log("Using Item. No use functionality");
    }

    public void EquipToHand(Transform hand)
    {
        if (equipItemPrefab != null && hand != null)
        {
            GameObject instance = GameObject.Instantiate(equipItemPrefab, hand);
            instance.transform.localPosition = Vector3.zero;
            instance.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Item prefab or hand is missing!");
        }
    }

}