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
    public GameObject homeButton;
    public GameObject muteCheck;
    public GameObject fullScreenCheck;
    public GameObject buttonResolution;

    void OnEnable()
    {
        InitializeLanguageDropdown();
        InitializeResolutionDropdown();
        InitializeSliders();
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.PC)
        {
            buttonResolution.SetActive(true);
        }
        muteCheck.SetActive(GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute);
        fullScreenCheck.SetActive(GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen);
        if (SceneManager.GetSceneByName("HomeScene").isLoaded) homeButton.SetActive(false);
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
    public void InitializeSliders()
    {
        FindSlider(TypeSound.Master).value = GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue;
        FindSlider(TypeSound.BGM).value = GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue;
        FindSlider(TypeSound.SFX).value = GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue;
    }
    public void UnloadScene()
    {
        Time.timeScale = 1;
        SceneManager.UnloadSceneAsync("OptionsScene");
        GameManager.Instance.isPause = false;
    }
    public void SetMixerValues()
    {
        GameManager.Instance.SetAudioMixerData();
    }
    public void ChangeSoundValue(int typeSound)
    {
        switch (typeSound)
        {
            case 0:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue = FindSlider(TypeSound.Master).value;
                break;
            case 1:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue = FindSlider(TypeSound.BGM).value;
                break;
            case 2:
                GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue = FindSlider(TypeSound.SFX).value;
                break;
        }
        SetMixerValues();
    }
    public void SetMute()
    {
        GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute = !GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute;
        muteCheck.SetActive(GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute);
        SetMixerValues();
        GameData.Instance.SaveGameData();
    }
    public void SetFullScreen()
    {
        GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen = !GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen;
        fullScreenCheck.SetActive(GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen);
        Screen.SetResolution(
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.width,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.currentResolution.height,
            GameData.Instance.saveData.configurationsInfo.resolutionConfiguration.isFullScreen);
        GameData.Instance.SaveGameData();
    }
    public void SetResolution()
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
    public Slider FindSlider(TypeSound typeSound)
    {
        foreach (var slider in soundInfo)
        {
            if (slider.typeSound == typeSound)
            {
                return slider.slider;
            }
        }
        return null;
    }
    [Serializable] public class SoundInfo
    {
        public TypeSound typeSound;
        public Slider slider;
    }
    public enum TypeSound
    {
        Master = 0,
        BGM = 1,
        SFX = 2
    }
}