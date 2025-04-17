using Unity.Cinemachine;
using UnityEngine;

public class PlayerCamera : MonoBehaviour, ManagementCharacter.IDirection
{
    PlayerInputs playerInputs;
    [SerializeField] CinemachineCamera vcam;
    [SerializeField] float speed = 0.01f;
    public GameObject playerModel;
    void Start()
    {
        playerInputs = GetComponent<PlayerInputs>();
    }
    public void HandleDirection()
    {
        var orbital = vcam.GetComponent<CinemachinePanTilt>();
        orbital.PanAxis.Value += playerInputs.playerInputsInfo.lookInput.x * speed;
        playerModel.transform.rotation = Quaternion.Euler
        (
            playerModel.transform.rotation.x,
            orbital.PanAxis.Value,
            playerModel.transform.rotation.z
        );
        float newTilt = orbital.TiltAxis.Value - playerInputs.playerInputsInfo.lookInput.y * speed;
        orbital.TiltAxis.Value = Mathf.Clamp(newTilt, orbital.TiltAxis.Range.x, orbital.TiltAxis.Range.y);
    }
}
