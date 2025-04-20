using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ManagementOpenCloseScene openCloseScene;
    public bool isWebGlBuild;
    public TypeDevice _currentDevice;
    public TypeDevice principalDevice;
    public event Action<TypeDevice> OnDeviceChanged;
    public TypeDevice currentDevice
    {
        get => _currentDevice;
        set
        {
            if (_currentDevice != value)
            {
                _currentDevice = value;
                OnDeviceChanged?.Invoke(_currentDevice);
            }
        }
    }
    public bool isPause;
    public bool startGame;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SetInitialDevice();
    }
    void Update()
    {
        if (startGame)
        {
            CheckCurrentDevice();
        }
    }
    public void ChangeSceneSelector(TypeScene typeScene)
    {
        isPause = false;
        Time.timeScale = 0;
        switch (typeScene)
        {
            case TypeScene.OptionsScene:
                isPause = true;
                Scene optionsScene = SceneManager.GetSceneByName("OptionsScene");
                if (!optionsScene.isLoaded) SceneManager.LoadScene("OptionsScene", LoadSceneMode.Additive);
                break;
            case TypeScene.CreditsScene:
                Scene creditsScene = SceneManager.GetSceneByName("CreditsScene");
                if (!creditsScene.isLoaded) SceneManager.LoadScene("CreditsScene", LoadSceneMode.Additive);
                break;
            case TypeScene.GameOver:
                Scene gameOverScene = SceneManager.GetSceneByName("GameOverScene");
                if (!gameOverScene.isLoaded) SceneManager.LoadScene("GameOverScene", LoadSceneMode.Additive);
                break;
            default:
                _ = ChangeScene(typeScene);
                break;
        }
    }
    public async Awaitable ChangeScene(TypeScene typeScene)
    {
        openCloseScene.openCloseSceneAnimator.SetBool("Out", true);
        await AudioManager.Instance.FadeOut();
        while (openCloseScene.openCloseSceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) await Task.Delay(TimeSpan.FromSeconds(0.05)); ;
        if (typeScene == TypeScene.NextLevel)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (typeScene == TypeScene.Reload)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else
        {
            Application.Quit();
        }
        if (typeScene != TypeScene.HomeScene ||
            typeScene != TypeScene.CreditsScene ||
            typeScene != TypeScene.OptionsScene)
        {
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
        await Task.Delay(TimeSpan.FromSeconds(0.05));
        _ = openCloseScene.WaitFinishCloseAnimation();
    }
    public void EnterScene()
    {
        openCloseScene.openCloseSceneAnimator.SetBool("Out", false);
    }
    public void SetInitialDevice()
    {
        if (!isWebGlBuild)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor ||
                Application.platform == RuntimePlatform.WindowsPlayer ||
                 Application.platform == RuntimePlatform.OSXPlayer ||
                 Application.platform == RuntimePlatform.LinuxPlayer)
            {
                currentDevice = TypeDevice.PC;
                principalDevice = TypeDevice.PC;
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                currentDevice = TypeDevice.MOBILE;
                principalDevice = TypeDevice.MOBILE;
            }
            else
            {
                currentDevice = TypeDevice.GAMEPAD;
                principalDevice = TypeDevice.GAMEPAD;
            }
        }
        else
        {
            currentDevice = TypeDevice.PC;
            principalDevice = TypeDevice.PC;
        }
    }
    void CheckCurrentDevice()
    {
        if (!isWebGlBuild)
        {
            if (ValidateDeviceIsMobile())
            {
                currentDevice = TypeDevice.MOBILE;
            }
            else if (IsGamepadInput())
            {
                currentDevice = TypeDevice.GAMEPAD;
            }
            else if (ValidateDeviceIsPc())
            {
                currentDevice = TypeDevice.PC;
            }
        }
        else
        {
            currentDevice = TypeDevice.PC;
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
    public enum TypeScene
    {
        HomeScene = 0,
        OptionsScene = 1,
        NextLevel = 2,
        CreditsScene = 3,
        Reload = 4,
        Exit = 5,
        GameOver = 6
    }
    public enum TypeDevice
    {
        None,
        PC,
        GAMEPAD,
        MOBILE,
    }
}