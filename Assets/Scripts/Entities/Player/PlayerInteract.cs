using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour, ManagementCharacter.IInteract
{
    PlayerInputs playerInputs;
    public ManagementCharacter managementCharacter;
    public float rangeRay;
    public ManagementObjectInteract _currentInteract;
    public Action<ManagementObjectInteract> OnCurrentObjectInteract;
    public ManagementObjectInteract currentInteract{
        get => _currentInteract;
        set
        {
            if (_currentInteract != value)
            {
                if (value)
                {
                    ValidateShowBannerInteract(value, true);
                }
                else
                {
                    ValidateShowBannerInteract(_currentInteract, false);
                }
                _currentInteract = value;
                OnCurrentObjectInteract?.Invoke(_currentInteract);
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
        currentInteract = CheckInteract();
    }
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (currentInteract)
        {
            currentInteract.Interact(managementCharacter);
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
        Gizmos.color = currentInteract ? Color.cyan : Color.blue;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * rangeRay);
    }
}
