using System;
using UnityEngine;

public class ButtonDevice : MonoBehaviour
{
    public TypeButton[] buttons;
    void Start()
    {
        GameManager.Instance.OnDeviceChanged += OnDeviceChange;
        OnDeviceChange(GameManager.Instance.currentDevice);
    }
    void OnDestroy()
    {
        GameManager.Instance.OnDeviceChanged -= OnDeviceChange;
    }
    void OnDeviceChange(GameManager.TypeDevice typeDevice)
    {
        foreach(TypeButton button in buttons)
        {
            button.button.SetActive(button.typeDevice == typeDevice);
        }
    }
    [Serializable] public class TypeButton
    {
        public GameManager.TypeDevice typeDevice;
        public GameObject button;
    }
}
