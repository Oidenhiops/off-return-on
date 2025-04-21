using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    void OnDestroy()
    {
        playerControls.Player.Move.performed -= OnMovement;
        playerControls.Player.Move.canceled -= OnMovement;
        playerControls.Player.Look.performed -= OnLook;
        playerControls.Player.Look.canceled -= OnLook;
        playerControls.Player.Pause.started -= OnPause;
    }
    void InitInputs()
    {
        playerControls.Player.Move.performed += OnMovement;
        playerControls.Player.Move.canceled += OnMovement;
        playerControls.Player.Look.performed += OnLook;
        playerControls.Player.Look.canceled += OnLook;
        playerControls.Player.Pause.started += OnPause;
    }
    public void OnMovement(InputAction.CallbackContext context)
    {
        playerInputsInfo.movementInput = context.ReadValue<Vector2>();
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        playerInputsInfo.lookInput = context.ReadValue<Vector2>();
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (GameManager.Instance.startGame) GameManager.Instance.ChangeSceneSelector(GameManager.TypeScene.OptionsScene);
    }
    [Serializable] public class PlayerInputsInfo{
        public Vector2 movementInput = new Vector2();
        public Vector2 lookInput = new Vector2();
    }
}
