using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    public virtual void Interact(ManagementCharacter managementCharacter) { Debug.LogWarning("No implemented"); }
    public virtual void DropItem(ManagementCharacter managementCharacter) { Debug.LogWarning("No implemented"); }
    public virtual void UseItem(ManagementCharacter managementCharacter) { Debug.LogWarning("No implemented"); }
}