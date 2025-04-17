using UnityEngine;

public class FlashLigth : MonoBehaviour, ItemInteract.IItem
{
    public Light _light;
    public void UseItem(ManagementCharacter managementCharacter)
    {
        _light.enabled = !_light.enabled;
    }
}
