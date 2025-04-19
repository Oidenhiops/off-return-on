using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
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
    public async Awaitable FadeIn()
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
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }
        await Task.Delay(TimeSpan.FromSeconds(0.05));
    }
    public async Awaitable FadeOut()
    {
        float decibelsMaster = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        while (decibelsMaster > -80)
        {
            if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute) break;
            decibelsMaster -= 1;
            audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), decibelsMaster);
            await Task.Delay(TimeSpan.FromSeconds(0.05));
        }
        await Task.Delay(TimeSpan.FromSeconds(0.05));
    }
    public void SetAudioMixerData()
    {
        float decibelsMaster = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue / 100);
        float decibelsBGM = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue / 100);
        float decibelsSFX = 20 * Mathf.Log10(GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue / 100);
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.MASTERValue == 0) decibelsMaster = -80;
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.BGMalue == 0) decibelsBGM = -80;
        if (GameData.Instance.saveData.configurationsInfo.soundConfiguration.SFXalue == 0) decibelsSFX = -80;
        audioMixer.SetFloat(ManagementOptions.TypeSound.BGM.ToString(), decibelsBGM);
        audioMixer.SetFloat(ManagementOptions.TypeSound.SFX.ToString(), decibelsSFX);
        audioMixer.SetFloat(ManagementOptions.TypeSound.Master.ToString(), GameData.Instance.saveData.configurationsInfo.soundConfiguration.isMute ? -80 : decibelsMaster);
        GameData.Instance.SaveGameData();
    }
}
