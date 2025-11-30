using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryItem _item;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI count;
    [SerializeField] private Button removeButton;
    [SerializeField] private TextMeshProUGUI descriptionText;
    
    public GameObject choicePanel;
    public Button equipButton;

    public void AddItem(InventoryItem newItem)
    {
        _item = newItem;
        icon.sprite = _item.data.icon;
        icon.enabled = true;
        removeButton.interactable = true;
        
        count.text = _item.count > 1 ? _item.count.ToString() : ""; 
    }

    public void ClearSlot()
    {
        _item = null;
        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
        count.text = "";
    }

    public void OnRemoveButton()
    {
        InventoryManager.Instance.RemoveItem(_item);
    }

    public void OptionItem()
    {
        if (_item != null)
        {
            bool isActive = choicePanel.activeSelf;
            choicePanel.SetActive(!isActive);
        }
    }

    public void Equip()
    { 
        InventoryManager.Instance.EquipInventoryItem(_item.data);
        choicePanel.SetActive(false);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_item != null && descriptionText != null)
            descriptionText.text = _item.data.description;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionText != null)
            descriptionText.text = "";
    }

}
