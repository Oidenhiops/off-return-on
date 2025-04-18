using System;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour, ManagementCharacter.IDirection
{
    PlayerInputs playerInputs;
    [SerializeField] CinemachineCamera vcam;
    [SerializeField] float speed = 0.01f;
    float currentSpeed = 0;
    public GameObject playerModel;
    void Start()
    {
        playerInputs = GetComponent<PlayerInputs>();
        GameManager.Instance.OnDeviceChanged += SetCurrentSpeed;
        SetCurrentSpeed(GameManager.Instance.currentDevice);
    }

    private void SetCurrentSpeed(GameManager.TypeDevice device)
    {
        if (device == GameManager.TypeDevice.PC)
        {
            currentSpeed = speed;
        }
        else if (device == GameManager.TypeDevice.GAMEPAD)
        {
            currentSpeed = speed * 50;
        }
        else if (device == GameManager.TypeDevice.MOBILE)
        {
            currentSpeed = speed * 50;
        }
    }
    public void HandleDirection()
    {
        var orbital = vcam.GetComponent<CinemachinePanTilt>();
        orbital.PanAxis.Value += playerInputs.playerInputsInfo.lookInput.x * currentSpeed;
        playerModel.transform.rotation = Quaternion.Euler
        (
            playerModel.transform.rotation.x,
            orbital.PanAxis.Value,
            playerModel.transform.rotation.z
        );
        float newTilt = orbital.TiltAxis.Value - playerInputs.playerInputsInfo.lookInput.y * currentSpeed / 2;
        orbital.TiltAxis.Value = Mathf.Clamp(newTilt, orbital.TiltAxis.Range.x, orbital.TiltAxis.Range.y);
    }
}
