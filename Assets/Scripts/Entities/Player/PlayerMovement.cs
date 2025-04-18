using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour, ManagementCharacter.IMovement
{
    PlayerInputs playerInputs;
    public ManagementCharacter managementCharacter;
    public float speed;
    Vector3 camForward;
    Vector3 camRight;
    Vector3 movementDirection;
    void Start()
    {
        playerInputs = GetComponent<PlayerInputs>();
    }
    public void HandleMove()
    {
        Vector3 inputs = new Vector3
        (
            playerInputs.playerInputsInfo.movementInput.x,
            0,
            playerInputs.playerInputsInfo.movementInput.y
        ).normalized;

        CamDirection();

        Vector3 camDirection = (inputs.x * camRight + inputs.z * camForward).normalized;
        movementDirection = new Vector3
        (
            camDirection.x,
            0,
            camDirection.z
        );

        movementDirection.x *= speed;
        movementDirection.z *= speed;
        movementDirection.y = managementCharacter.character.rb.linearVelocity.y;
        managementCharacter.character.rb.linearVelocity = movementDirection;
    }
    void CamDirection()
    {
        Vector3 camForwardDirection = Camera.main.transform.forward;
        Vector3 camRightDirection = Camera.main.transform.right;

        camForwardDirection.y = 0;
        camRightDirection.y = 0;

        camForward = camForwardDirection.normalized;
        camRight = camRightDirection.normalized;
    }
}
