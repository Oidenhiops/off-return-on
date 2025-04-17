using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    public PlayerControls playerControls;
    public PlayerInputsInfo playerInputsInfo;
    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Enable();
        playerInputsInfo = new PlayerInputsInfo();
        InitInputs();
    }
    void InitInputs()
    {
        playerControls.Player.Move.performed += OnMovement;
        playerControls.Player.Move.canceled += OnMovement;
        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Look.canceled += OnLook;
    }
    void Update()
    {
        CurrentDevice();
    }
    void CurrentDevice()
    {
        if (!GameManager.Instance.isWebGlBuild)
        {
            if (ValidateDeviceIsMobile())
            {
                GameManager.Instance.currentDevice = GameManager.TypeDevice.MOBILE;
            }
            else if (IsGamepadInput())
            {
                GameManager.Instance.currentDevice = GameManager.TypeDevice.GAMEPAD;
            }
            else if (ValidateDeviceIsPc())
            {
                GameManager.Instance.currentDevice = GameManager.TypeDevice.PC;
            }
        }
        else
        {
            GameManager.Instance.currentDevice = GameManager.TypeDevice.PC;
        }
    }
    bool ValidateDeviceIsMobile()
    {
        return Touchscreen.current != null;
    }
    bool ValidateDeviceIsPc()
    {
        return Keyboard.current.anyKey.wasPressedThisFrame ||
            Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame ||
            Mouse.current.scroll.ReadValue() != Vector2.zero ||
            Mouse.current.delta.ReadValue() != Vector2.zero;
    }
    bool IsGamepadInput()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return false;
        bool currentDeviceIsGamepad = Gamepad.current != null;
        bool validateAnyGamepadInput = gamepad.buttonSouth.wasPressedThisFrame ||
               gamepad.buttonNorth.wasPressedThisFrame ||
               gamepad.buttonEast.wasPressedThisFrame ||
               gamepad.buttonWest.wasPressedThisFrame ||
               gamepad.leftStick.ReadValue() != Vector2.zero ||
               gamepad.rightStick.ReadValue() != Vector2.zero ||
               gamepad.dpad.ReadValue() != Vector2.zero ||
               gamepad.leftTrigger.wasPressedThisFrame ||
               gamepad.rightTrigger.wasPressedThisFrame;
        return currentDeviceIsGamepad && validateAnyGamepadInput;
    }
    public void OnMovement(InputAction.CallbackContext context){
        playerInputsInfo.movementInput = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        playerInputsInfo.lookInput = context.ReadValue<Vector2>();
    }
    public class PlayerInputsInfo{
        public Vector2 movementInput = new Vector2();
        public Vector2 lookInput = new Vector2();
    }
}
