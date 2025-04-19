using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }
    string nameSaveData = "SaveData.json";
    public SaveData saveData = new SaveData();    
    public List<string[]> csvData = new List<string[]>();
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
        _ = LoadData();
    }
    public async Awaitable LoadData()
    {
        CheckFileExistance(DataPath());
        saveData = ReadDataFromJson();
        LoadCSV();
        SetResolutionData();
        await InitializeAudioMixerData();
        GameManager.Instance.StartCoroutine(AudioManager.Instance.FadeIn());
    }
    async Awaitable InitializeAudioMixerData()
    {
        await Awaitable.NextFrameAsync();
        float decibelsBGM = 20 * Mathf.Log10(saveData.configurationsInfo.soundConfiguration.BGMalue / 100);
        float decibelsSFX = 20 * Mathf.Log10(saveData.configurationsInfo.soundConfiguration.SFXalue / 100);
        if (saveData.configurationsInfo.soundConfiguration.BGMalue == 0) decibelsBGM = -80;
        if (saveData.configurationsInfo.soundConfiguration.SFXalue == 0) decibelsSFX = -80;
        AudioManager.Instance.audioMixer.SetFloat(ManagementOptions.TypeSound.BGM.ToString(), decibelsBGM);
        AudioManager.Instance.audioMixer.SetFloat(ManagementOptions.TypeSound.SFX.ToString(), decibelsSFX);
        _= AudioManager.Instance.FadeIn();
        await Awaitable.NextFrameAsync();
    }
    void SetStartingResolution(SaveData dataInfo)
    {
        Resolution[] resolutions = Screen.resolutions;
        Array.Reverse(resolutions);
        foreach (Resolution res in resolutions)
        {
            dataInfo.configurationsInfo.resolutionConfiguration.allResolutions.Add(new ResolutionsInfo(res.width, res.height));
        }
        Screen.SetResolution(resolutions[0].width, resolutions[0].height, true);
        dataInfo.configurationsInfo.resolutionConfiguration.isFullScreen = true;
        dataInfo.configurationsInfo.resolutionConfiguration.currentResolution = new ResolutionsInfo(
            resolutions[0].width,
            resolutions[0].height);
    }
    void SetStartingPlayerData(SaveData dataInfo)
    {
        CharacterInfo newCharacterInfo = new CharacterInfo();
        newCharacterInfo.characterSelectedName = "Warrior";
        dataInfo.gameInfo.characterInfo = newCharacterInfo;
    }
    void SetStartingDataSound(SaveData dataInfo)
    {
        dataInfo.configurationsInfo.soundConfiguration.MASTERValue = 25;
        dataInfo.configurationsInfo.soundConfiguration.BGMalue = 25;
        dataInfo.configurationsInfo.soundConfiguration.SFXalue = 25;
    }
    void SetResolutionData()
    {
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.PC)
        {
            Screen.SetResolution(
                saveData.configurationsInfo.resolutionConfiguration.currentResolution.width,
                saveData.configurationsInfo.resolutionConfiguration.currentResolution.height,
                saveData.configurationsInfo.resolutionConfiguration.isFullScreen
            );
        }
        else
        {
            Screen.SetResolution(
                Screen.width,
                Screen.height,
                true
            );
        }
    }
    void CheckFileExistance(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            SetStartingData();
            string dataString = JsonUtility.ToJson(saveData);
            File.WriteAllText(filePath, dataString);
        }
    }
    SaveData ReadDataFromJson()
    {
        string dataString;
        string jsonFilePath = DataPath();
        dataString = File.ReadAllText(jsonFilePath);
        saveData = JsonUtility.FromJson<SaveData>(dataString);
        return saveData;
    }
    void WriteDataToJson()
    {
        string dataString;
        string jsonFilePath = DataPath();
        dataString = JsonUtility.ToJson(saveData);
        File.WriteAllText(jsonFilePath, dataString);
    }
    string DataPath()
    {
        if (Directory.Exists(Application.persistentDataPath))
        {
            return Path.Combine(Application.persistentDataPath, nameSaveData);
        }
        return Path.Combine(Application.streamingAssetsPath, nameSaveData);
    }
    public void SaveGameData()
    {
        WriteDataToJson();
    }
    public void SetStartingData()
    {
        SaveData dataInfo = new SaveData();
        dataInfo.configurationsInfo.currentLanguage = TypeLanguage.English;
        SetStartingDataSound(dataInfo);
        SetStartingPlayerData(dataInfo);
        dataInfo.configurationsInfo.FpsLimit = 60;
        Application.targetFrameRate = 60;
        if (GameManager.Instance.currentDevice == GameManager.TypeDevice.PC) SetStartingResolution(dataInfo);
        saveData.gameInfo = new GameInfo();
        saveData = dataInfo;
        SaveGameData();
    }
    void LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Language/Language");
        string[] lines = csvFile.text.Split('\n');
        List<String[]> textData = new List<string[]>();
        foreach (string line in lines)
        {
            string[] columns = line.Split(';');
            textData.Add(columns);
        }
        csvData = textData;
    }
    public string GetDialog(int id)
    {
        int languageIndex = 0;
        for (int i = 0; i < csvData[0].Length; i++)
        {
            if (csvData[0][i] == saveData.configurationsInfo.currentLanguage.ToString())
            {
                languageIndex = i;
                break;
            }
        }
        if (languageIndex != 0) return csvData[id][languageIndex];
        return null;
    }
    public void ChangeLanguage(TypeLanguage language)
    {
        saveData.configurationsInfo.currentLanguage = language;
    }
    [Serializable] public class SaveData
    {
        public GameInfo gameInfo = new GameInfo();
        public ConfigurationsInfo configurationsInfo = new ConfigurationsInfo();
    }
    [Serializable] public class GameInfo
    {
        public CharacterInfo characterInfo = new CharacterInfo();
    }
    [Serializable] public class CharacterInfo
    {
        public bool isInitialize = false;
        public string characterSelectedName;
    }
    [Serializable] public class ConfigurationsInfo
    {
        public TypeLanguage _currentLanguage;
        public Action<TypeLanguage> OnLanguageChange;
        public TypeLanguage currentLanguage{
            get => _currentLanguage;
            set{
                if (_currentLanguage != value){
                    _currentLanguage = value;
                    OnLanguageChange?.Invoke(_currentLanguage);
                }
            }
        }
        public int FpsLimit = 0;
        public ResolutionConfiguration resolutionConfiguration = new ResolutionConfiguration();
        public SoundConfiguration soundConfiguration = new SoundConfiguration();
    }
    [Serializable] public class SoundConfiguration
    {
        public bool isMute = false;
        public float MASTERValue;
        public float BGMalue;
        public float SFXalue;
    }
    [Serializable] public class ResolutionConfiguration
    {
        public bool isFullScreen = false;
        public List<ResolutionsInfo> allResolutions = new List<ResolutionsInfo>();
        public ResolutionsInfo currentResolution;
    }
    [Serializable] public class ResolutionsInfo
    {
        public int width = 0;
        public int height = 0;
        public ResolutionsInfo(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }
    public enum TypeLanguage
    {
        English = 0,
        Español = 1,
    }
}
