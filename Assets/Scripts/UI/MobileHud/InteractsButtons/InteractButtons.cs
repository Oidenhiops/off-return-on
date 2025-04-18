using System;
using UnityEngine;

public class InteractButtons : MonoBehaviour
{
    public PlayerItems playerItems;
    public PlayerInteract playerInteract;
    public GameObject[] itemButtons;
    public GameObject interactButtons;
    void Start()
    {
        playerItems.OnCurrentItemChange += OnItemChange;
        playerInteract.OnObjectInteractChange += OnInteractChange;
        OnItemChange(playerItems.currentItem);
        OnInteractChange(playerInteract.objectInteract);
    }

    void OnItemChange(PlayerItems.Item item)
    {
        if (item != null && item.itemInfo.itemSO)
        {
            foreach (GameObject button in itemButtons)
            {
                button.SetActive(true);
            }
            return;
        }
        foreach (GameObject button in itemButtons)
        {
            button.SetActive(false);
        }
    }
    void OnInteractChange(ManagementObjectInteract interact)
    {
        if (interact)
        {
            interactButtons.SetActive(true);
            return;
        }
        interactButtons.SetActive(false);
    }
}
