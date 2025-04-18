using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        SetInitialDevice();
    }
    public ManagementOpenCloseScene openCloseScene;
    public Coroutine fadeIn;
    public Coroutine fadeOut;
    public bool isWebGlBuild;
    public TypeDevice _currentDevice;
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
    public void ChangeSceneSelector(TypeScene typeScene)
    {
        Time.timeScale = 1;
        isPause = false;
        switch (typeScene)
        {
            case TypeScene.OptionsScene:
                isPause = true;
                SceneManager.LoadScene("OptionsScene", LoadSceneMode.Additive);
                Time.timeScale = 0;
                break;
            case TypeScene.Exit:
                openCloseScene.openCloseSceneAnimator.Play("Out");
                openCloseScene.openCloseSceneAnimator.SetBool("Out", true);
                StartCoroutine(ChangeScene(typeScene));
                break;
            default:
                openCloseScene.openCloseSceneAnimator.Play("Out");
                openCloseScene.openCloseSceneAnimator.SetBool("Out", true);
                StartCoroutine(ChangeScene(typeScene));
                break;
        }
    }
    public IEnumerator ChangeScene(TypeScene typeScene)
    {
        fadeOut = StartCoroutine(FadeOut());
        yield return new WaitForSecondsRealtime(2);
        if (typeScene != TypeScene.Exit)
        {
            SceneManager.LoadScene(typeScene.ToString());
        }
        else
        {
            Application.Quit();
        }
        ChangedScene();
    }
    public void EnterScene()
    {
        openCloseScene.openCloseSceneAnimator.SetBool("Out", false);
    }
    public void ChangedScene()
    {
        Cursor.visible = true;
        StopCoroutine(fadeOut);
    }
    public IEnumerator FadeIn()
    {
        float decibelsMaster = 20 * Mathf.Log10(ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
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
            if (ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.isMute) break;
            currentVolumen++;
            audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), currentVolumen);
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
    public IEnumerator FadeOut()
    {
        float decibelsMaster = 20 * Mathf.Log10(ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        while (decibelsMaster > -80)
        {
            if (ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.isMute) break;
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
        float decibelsMaster = 20 * Mathf.Log10(ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        float decibelsBGM = 20 * Mathf.Log10(ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue / 100);
        float decibelsSFX = 20 * Mathf.Log10(ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue / 100);

        if (ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue == 0)
        {
            decibelsMaster = -80;
        }
        if (ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue == 0)
        {
            decibelsBGM = -80;
        }
        if (ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue == 0)
        {
            decibelsSFX = -80;
        }
        audioMixer.SetFloat(ManagementOptions.TypeSound.BGM.ToString(), decibelsBGM);
        audioMixer.SetFloat(ManagementOptions.TypeSound.SFX.ToString(), decibelsSFX);
        audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), ManagementData.Instance.saveData.configurationsInfo.soundConfiguration.isMute ? -80 : decibelsMaster);
        ManagementData.Instance.SaveGameData();
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
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                currentDevice = TypeDevice.MOBILE;
            }
            else
            {
                currentDevice = TypeDevice.GAMEPAD;
            }
        }
        else
        {
            currentDevice = TypeDevice.PC;
        }
    }
    public enum TypeScene
    {
        HomeScene = 0,
        OptionsScene = 1,
        GameScene = 2,
        Exit = 3,
        NextLevel = 1,
        CreditsScene = 5,
    }
    public enum TypeDevice
    {
        None,
        PC,
        GAMEPAD,
        MOBILE,
    }
    public void OpenCreditsMenu()
    {
        SceneManager.LoadScene("CreditsMenu", LoadSceneMode.Additive);
    }

    public void OpenOptionsMenu()
    {
        SceneManager.LoadScene("OptionMenu", LoadSceneMode.Additive);
    }

    public void ReloadCurrentScene()
    {
    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        // Verifica si hay una siguiente escena
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No hay m�s escenas. �Has completado el juego!");
            ReloadCurrentScene(); // Recargar la primera escena
        }
    }

}
