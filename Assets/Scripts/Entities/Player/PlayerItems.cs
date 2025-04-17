using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class PlayerItems : MonoBehaviour, ManagementCharacter.IItems
{
    PlayerInputs playerInputs;
    public ManagementCharacter managementCharacter;
    public Item[] items;
    public int itemPos = 0;
    void Start()
    {
        playerInputs = GetComponent<PlayerInputs>();
        playerInputs.playerControls.Player.ChangeItem.started += OnChangeItem;
        playerInputs.playerControls.Player.SelectItem.started += OnSelectItem;
        playerInputs.playerControls.Player.DropItem.started += OnDropItem;
        playerInputs.playerControls.Player.UseItem.started += OnUseItem;
    }
    void OnChangeItem(InputAction.CallbackContext context)
    {
        ChangeItem(context.ReadValue<float>() > 0);
    }
    void OnSelectItem(InputAction.CallbackContext context)
    {
        ChangeItem(context.action.GetBindingIndexForControl(context.control));
    }
    void OnDropItem(InputAction.CallbackContext context)
    {
        DropItem(itemPos);
    }
    void OnUseItem(InputAction.CallbackContext context)
    {
        UseItem();
    }
    void ChangeItem(bool direction)
    {
        if (items[itemPos].itemInfo.itemSO) items[itemPos].itemInfo.itemObj.SetActive(false);
        itemPos += direction? 1 : -1;
        if (itemPos == items.Length)
        {
            itemPos = 0;
        }
        else if (itemPos < 0)
        {
            itemPos = items.Length - 1;
        }
        if (items[itemPos].itemInfo.itemSO) items[itemPos].itemInfo.itemObj.SetActive(true);
    }
    void ChangeItem(int pos)
    {
        if (items[itemPos].itemInfo.itemSO) items[itemPos].itemInfo.itemObj.SetActive(false);
        itemPos = pos;
        if (items[itemPos].itemInfo.itemSO) items[itemPos].itemInfo.itemObj.SetActive(true);
    }
    public void PickUpItem(ItemInteract.ItemInfo itemInfo)
    {
        if (items[itemPos].amount + 1 > itemInfo.itemSO.maxItems) return;
        items[itemPos].amount++;
        items[itemPos].itemInfo = itemInfo;
        ChangeLayer("Hand");
        itemInfo.managementObjectInteract.canInteract = false;
        itemInfo.managementObjectInteract.DisableBanner();
        itemInfo.rb.isKinematic = true;
        itemInfo.collider.enabled = false;
        itemInfo.itemObj.transform.SetParent(managementCharacter.character.rightHand);
        itemInfo.itemObj.transform.localPosition = Vector3.zero;
        itemInfo.itemObj.transform.localRotation = Quaternion.identity;
    }
    public void DropItem(int index)
    {
        ChangeLayer("Interact");
        items[index].itemInfo.rb.isKinematic = false;
        items[index].itemInfo.collider.enabled = true;
        items[index].itemInfo.itemObj.transform.SetParent(null);
        items[index].itemInfo.managementObjectInteract.canInteract = true;
        items[index].itemInfo.itemInteract.DropItem(managementCharacter);
        items[index].amount--;
        if (items[index].amount == 0) items[index].itemInfo = new ItemInteract.ItemInfo();
    }
    public void UseItem()
    {
        if (items[itemPos].itemInfo.itemSO)
        {
            items[itemPos].itemInfo.itemInteract.UseItem(managementCharacter);
        }
    }
    void ChangeLayer(String nameLayer)
    {
        for(int i = 1; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.layer = LayerMask.NameToLayer(nameLayer);
        }
    }
    [Serializable] public class Item
    {
        public int amount;
        public ItemInteract.ItemInfo itemInfo;
    }
}
