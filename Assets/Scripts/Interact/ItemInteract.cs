using System;
using UnityEngine;

public class ItemInteract : ObjectBase
{
    public ItemInfo itemInfo;
    public override void Interact(ManagementCharacter managementCharacter)
    {
        managementCharacter.character.itemsCs.PickUpItem(itemInfo);
    }

    public override void DropItem(ManagementCharacter managementCharacter)
    {
        
    }
    public override void UseItem(ManagementCharacter managementCharacter)
    {
        if(TryGetComponent<IItem>(out IItem component)) component.UseItem(managementCharacter);
    }
    [Serializable] public class ItemInfo
    {
        public ItemInteract itemInteract;
        public ItemSO itemSO;
        public GameObject itemObj;
        public ManagementObjectInteract managementObjectInteract;
        public Rigidbody rb;
        public Collider collider;
    }
    public interface IItem
    {
        public void UseItem(ManagementCharacter managementCharacter);
    }
}
