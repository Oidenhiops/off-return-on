using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public ManagementOpenCloseScene openCloseScene;
    public Coroutine fadeIn;
    public Coroutine fadeOut;
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
    public AudioMixer audioMixer;
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
        switch (typeScene)
        {
            case TypeScene.OptionsScene:
                isPause = true;
                if (SceneManager.GetActiveScene().name != "HomeScene") Time.timeScale = 0;
                SceneManager.LoadScene("OptionsScene", LoadSceneMode.Additive);                
                break;
            case TypeScene.CreditsScene:                
                SceneManager.LoadScene("CreditsScene", LoadSceneMode.Additive);
                break;
            default:
                StartCoroutine(ChangeScene(typeScene));
                break;
        }
    }
    public IEnumerator ChangeScene(TypeScene typeScene)
    {
        Time.timeScale = 1;
        openCloseScene.ResetValues();
        openCloseScene.openCloseSceneAnimator.Play("Out");
        openCloseScene.openCloseSceneAnimator.SetBool("Out", true);
        fadeOut = StartCoroutine(FadeOut());
        yield return new WaitForSecondsRealtime(2);
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
        ChangedScene();
    }
    public void EnterScene()
    {
        openCloseScene.openCloseSceneAnimator.SetBool("Out", false);
    }
    public void ChangedScene()
    {
        StopCoroutine(fadeOut);
    }
    public IEnumerator FadeIn()
    {
        float decibelsMaster = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        float currentVolumen;
        float volume;
        if (audioMixer.GetFloat(ManagementOptions.TypeSound.Master.ToString(), out volume))
        {
            currentVolumen = volume;
        }
        else
        {
            currentVolumen = -80f;
        }
        while (currentVolumen < decibelsMaster)
        {
            if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute) break;
            currentVolumen++;
            audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), currentVolumen);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    public IEnumerator FadeOut()
    {
        float decibelsMaster = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        while (decibelsMaster > -80)
        {
            if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute) break;
            decibelsMaster -= 1;
            audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), decibelsMaster);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    public void PlayASound(AudioClip audioClip)
    {
        AudioSource audioBox = Instantiate(Resources.Load<GameObject>("Prefabs/AudioBox/AudioBox")).GetComponent<AudioSource>();
        audioBox.clip = audioClip;
        audioBox.Play();
        Destroy(audioBox.gameObject, audioBox.clip.length);
    }
    public void PlayASound(AudioClip audioClip, float initialRandomPitch)
    {
        AudioSource audioBox = Instantiate(Resources.Load<GameObject>("Prefabs/AudioBox/AudioBox")).GetComponent<AudioSource>();
        audioBox.clip = audioClip;
        audioBox.pitch = UnityEngine.Random.Range(initialRandomPitch - 0.1f, initialRandomPitch + 0.1f);
        audioBox.Play();
        Destroy(audioBox.gameObject, audioBox.clip.length);
    }
    public void SetAudioMixerData()
    {
        float decibelsMaster = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        float decibelsBGM = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue / 100);
        float decibelsSFX = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue / 100);

        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue == 0)
        {
            decibelsMaster = -80;
        }
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue == 0)
        {
            decibelsBGM = -80;
        }
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue == 0)
        {
            decibelsSFX = -80;
        }
        audioMixer.SetFloat(ManagementOptions.TypeSound.BGM.ToString(), decibelsBGM);
        audioMixer.SetFloat(ManagementOptions.TypeSound.SFX.ToString(), decibelsSFX);
        audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute ? -80 : decibelsMaster);
        GameData.Instance.SaveGameData();
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
    }
    public enum TypeDevice
    {
        None,
        PC,
        GAMEPAD,
        MOBILE,
    }
}