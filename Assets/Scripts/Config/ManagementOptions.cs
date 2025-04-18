using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ManagementOptions : MonoBehaviour
{
    public TMP_Dropdown dropdownLanguage;
    public TMP_Dropdown dropdownResolution;
    public SoundInfo[] soundInfo;
    public WindowModeButtonsInfo windowModeButtonsInfo;
    public FpsButtonsInfo fpsButtonsInfo;
    public GameObject homeButton;
    public GameObject muteCheck;
    public GameObject buttonResolution;
    public ControlsInfo[] controlsInfo;

    void OnEnable()
    {
        InitializeLanguageDropdown();
        InitializeResolutionDropdown();
        SetFullScreenButtonsSprite();
        SetVolumeFillAmounts();
        GameManager.Instance.OnDeviceChanged += ChangeMenuButtons;
        ChangeMenuButtons(GameManager.Instance.currentDevice);
        muteCheck.SetActive(GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute);
        if (SceneManager.GetSceneByName("HomeScene").isLoaded) homeButton.SetActive(false);
    }
    void OnDestroy()
    {
        if (GameManager.Instance.principalDevice == GameManager.TypeDevice.PC) GameManager.Instance.OnDeviceChanged -= ChangeMenuButtons;
    }
    public void InitializeLanguageDropdown()
    {
        dropdownLanguage.options.Clear();
        foreach (GameData.TypeLanguage language in Enum.GetValues(typeof(GameData.TypeLanguage)))
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
            {
                text = language.ToString()
            };
            dropdownLanguage.options.Add(option);
        }
        for (int i = 0; i < dropdownLanguage.options.Count; i++)
        {
            if (dropdownLanguage.options[i].text == GameData.Instance.saveData.configurationsInfo.currentLanguage.ToString())
            {
                dropdownLanguage.value = i;
                break;
            }
        }
    }
    public void ChangeLanguage()
    {
        GameData.TypeLanguage language = (GameData.TypeLanguage)dropdownLanguage.value;
        GameData.Instance.ChangeLanguage(language);
    }
    public void InitializeResolutionDropdown()
    {
        dropdownResolution.options.Clear();
        foreach (var resolutions in GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.allResolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
            {
                text = $"{resolutions.width}X{resolutions.height}"
            };
            dropdownResolution.options.Add(option);
        }
        for (int i = 0; i < dropdownResolution.options.Count; i++)
        {
            GameData.ResolutionsInfo resolutionsInfo = GetCurrentResolution(dropdownResolution.options[i].text);
            if (resolutionsInfo.width == GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.width &&
                resolutionsInfo.height == GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.height)
            {
                dropdownResolution.value = i;
                break;
            }
        }
    }
    public void SetVolumeFillAmounts()
    {
        FindSlider(TypeSound.Master).fillAmount = GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100;
        FindSlider(TypeSound.BGM).fillAmount = GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue / 100;
        FindSlider(TypeSound.SFX).fillAmount = GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue / 100;
    }
    public Image FindSlider(TypeSound typeSound)
    {
        foreach (var sound in soundInfo)
        {
            if (sound.typeSound == typeSound)
            {
                return sound.image;
            }
        }
        return null;
    }
    public void SetMixerValues()
    {
        GameManager.Instance.SetAudioMixerData();
    }
    public void SetSoundValue(int typeSound, bool isAdd)
    {
        int amount = isAdd ? 1 : -1;
        switch (typeSound)
        {
            case 0:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue += amount;
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue = 
                    Math.Clamp(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue, 0, 100);
                break;
            case 1:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue += amount;
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue = 
                    Math.Clamp(GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue, 0, 100);
                break;
            case 2:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue += amount;
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue = 
                    Math.Clamp(GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue, 0, 100);
                break;
        }
        SetVolumeFillAmounts();
        SetMixerValues();
    }
    public void PlusVolume(int typeSound)
    {
        SetSoundValue(typeSound, true);
    }
    public void MiunsVolume(int typeSound)
    {
        SetSoundValue(typeSound, false);
    }
    public void SetMute()
    {
        GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute = !GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute;
        muteCheck.SetActive(GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute);
        SetMixerValues();
        GameData.Instance.SaveGameData();
    }
    public void SetFullScreen(bool isFullScreen)
    {
        GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen = isFullScreen;
        SetFullScreenButtonsSprite();
        Screen.SetResolution(
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.width,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.height,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen);
        GameData.Instance.SaveGameData();
    }
    public void SetFullScreenButtonsSprite()
    {
        bool isFullScreen = GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen;
        if (isFullScreen)
        {
            windowModeButtonsInfo.buttonsImage[0].sprite = windowModeButtonsInfo.spriteOn;
            windowModeButtonsInfo.buttonsImage[1].sprite = windowModeButtonsInfo.spriteOff;
            return;
        }
        windowModeButtonsInfo.buttonsImage[0].sprite = windowModeButtonsInfo.spriteOff;
        windowModeButtonsInfo.buttonsImage[1].sprite = windowModeButtonsInfo.spriteOn;
    }
    public void SetFpsLimit(int id)
    {
        int fps = id * 30;
        Application.targetFrameRate = fps;
        SetFpsLimitButtonsSprite();
    }
    public void SetFpsLimitButtonsSprite()
    {
        foreach(FpsButton fpsButton in fpsButtonsInfo.buttons)
        {
            if (fpsButton.id == Application.targetFrameRate)
            {
                fpsButton.buttonImage.sprite = fpsButtonsInfo.spriteOn;
                return;
            }
            fpsButton.buttonImage.sprite = fpsButtonsInfo.spriteOff;
        }
    }
    public void ChangeResolution()
    {
        GameData.ResolutionsInfo currentResolution = GetCurrentResolution(dropdownResolution.options[dropdownResolution.value].text);
        GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution = currentResolution;
        Screen.SetResolution(
            currentResolution.width,
            currentResolution.height,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen);
        GameData.Instance.SaveGameData();
    }
    public GameData.ResolutionsInfo GetCurrentResolution(string resolution)
    {
        int index = resolution.IndexOf("X");
        int width = int.Parse(resolution.Substring(0, index));
        int height = int.Parse(resolution.ToString().Substring(index + 1));
        return new GameData.ResolutionsInfo(width, height);
    }
    public void ChangeMenuButtons(GameManager.TypeDevice typeDevice)
    {
        foreach (ControlsInfo control in controlsInfo)
        {
            if (control.typeDevice == typeDevice)
            {
                control.container.SetActive(true);
            }
            else
            {
                control.container.SetActive(false);
            }
        }
        if (typeDevice != GameManager.TypeDevice.MOBILE)
        {
            buttonResolution.SetActive(true);
        }
        else
        {
            buttonResolution.SetActive(false);
        }
    }
    [Serializable] public class SoundInfo
    {
        public TypeSound typeSound;
        public Image image;
    }
    [Serializable] public class WindowModeButtonsInfo
    {
        public Sprite spriteOn;
        public Sprite spriteOff;
        public Image[] buttonsImage;
    }
    [Serializable] public class FpsButtonsInfo
    {
        public Sprite spriteOn;
        public Sprite spriteOff;
        public FpsButton[] buttons;
    }
    [Serializable] public class FpsButton
    {
        public int id;
        public Image buttonImage;
    }
    [Serializable] public class ControlsInfo
    {
        public GameManager.TypeDevice typeDevice;
        public GameObject container;
    }
    public enum TypeSound
    {
        Master = 0,
        BGM = 1,
        SFX = 2
    }
}