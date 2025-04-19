using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour, ManagementCharacter.IInteract
{
    PlayerInputs playerInputs;
    public ManagementCharacter managementCharacter;
    public float rangeRay;
    public ManagementObjectInteract _objectInteract;
    public Action<ManagementObjectInteract> OnObjectInteractChange;
    public ManagementObjectInteract objectInteract
    {
        get => _objectInteract;
        set
        {
            if (_objectInteract != value)
            {
                if (value)
                {
                    ValidateShowBannerInteract(value, true);
                }
                else
                {
                    ValidateShowBannerInteract(_objectInteract, false);
                }
                _objectInteract = value;
                OnObjectInteractChange?.Invoke(_objectInteract);
            }
        }
    }
    void Start()
    {
        playerInputs = GetComponent<PlayerInputs>();
        playerInputs.playerControls.Player.Interact.started += OnInteract;
    }
    public void HandleInteract(){
        
    }
    public void FixedUpdate()
    {
        if (!GameManager.Instance.startGame || !managementCharacter.character.isActive) return;
        objectInteract = CheckInteract();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (objectInteract)
        {
            objectInteract.Interact(managementCharacter);
        }
    }
    public void ValidateShowBannerInteract(ManagementObjectInteract objectToInteract, bool canShow)
    {
        if (canShow)
        {
            objectToInteract.EnableBanner();
        }
        else
        {
            objectToInteract.DisableBanner();
        }
    }
    ManagementObjectInteract CheckInteract()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, rangeRay, LayerMask.GetMask("Interact")))
        {
            if (hit.collider.TryGetComponent<ManagementObjectInteract>(out ManagementObjectInteract component)){
                if (component.canInteract)
                {
                    return component;       
                }
            }
        }
        return null;
    }
    void OnDrawGizmos()
    {
        if (!managementCharacter.character.collidersInfo.useGizmos) return;
        Gizmos.color = objectInteract ? Color.cyan : Color.blue;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * rangeRay);
    }
}
