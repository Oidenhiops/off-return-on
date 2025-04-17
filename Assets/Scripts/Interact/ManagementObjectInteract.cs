using UnityEngine;

public class ManagementObjectInteract : MonoBehaviour
{
    public GameObject bannerInteract;
    public bool canInteract = false;
    ObjectBase _objectBase;
    void Awake()
    {
       _objectBase = GetComponent<ObjectBase>(); 
    }
    public void Interact(ManagementCharacter managementCharacter)
    {
        _objectBase.Interact(managementCharacter);
    }

    public void EnableBanner()
    {
        bannerInteract.SetActive(true);
    }

    public void DisableBanner()
    {
        bannerInteract.SetActive(false);
    }
}