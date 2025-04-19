using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class PlayerMovement : MonoBehaviour, ManagementCharacter.IMovement
{
    PlayerInputs playerInputs;
    public ManagementCharacter managementCharacter;
    public float speed;
    Vector3 camForward;
    Vector3 camRight;
    Vector3 movementDirection;
    float otherSpeed = 1;
    [SerializeField] Transform playerPos;
    [SerializeField] CapsuleCollider playerCollider;
    bool isCrouch = false;
    public float rangeRay;
    public LayerMask layerMask;
    void Start()
    {
        playerInputs = GetComponent<PlayerInputs>();
        playerInputs.playerControls.Player.Crouch.performed += OnCrouch;
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

        movementDirection.x *= speed * otherSpeed;
        movementDirection.z *= speed * otherSpeed;
        movementDirection.y = managementCharacter.character.rb.linearVelocity.y;
        managementCharacter.character.rb.linearVelocity = movementDirection;
        ValidateStandUp();
    }
    void ValidateStandUp()
    {
        if (isCrouch && !playerInputs.playerControls.Player.Crouch.IsInProgress())
        {
            if (CanStandUp())
            {
                isCrouch = false;
                otherSpeed = 1;
                playerPos.localPosition = new Vector3(0, 0, 0);
                playerCollider.center = new Vector3(0, 0.75f, 0);
                playerCollider.height = 1.5f;
            }
        }
    }
    bool CanStandUp()
    {        
        if (Physics.Raycast(transform.position, Vector3.up * rangeRay, rangeRay, layerMask))
        {
            return false;
        }
        return true;
    }
    void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.action.IsPressed())
        {
            otherSpeed = 0.5f;
            playerPos.localPosition = new Vector3(0, -1.1f, 0);
            playerCollider.center = new Vector3(0, 0.2f, 0);
            playerCollider.height = 0.2f;
            isCrouch = true;
        }
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
    void OnDrawGizmos()
    {
        Gizmos.color = CanStandUp() ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.up * rangeRay);
    }
}
