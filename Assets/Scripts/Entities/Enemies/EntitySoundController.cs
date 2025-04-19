using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class EntitySoundController : MonoBehaviour
{
    [Header("3D Sound Settings")]
    [SerializeField] private AudioClip breathingSound;  // Audio de respiración en loop

    [Header("Reverb Settings")]
    [SerializeField] private AudioReverbPreset reverbPreset = AudioReverbPreset.StoneCorridor;

    private AudioSource breathingSource;
    private AudioReverbFilter reverbFilter;

    void Start()
    {
        ConfigureAudioSystem();
        InitializeBreathing();
    }

    private void ConfigureAudioSystem()
    {
        // Configurar Reverb Filter
        reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
        reverbFilter.reverbPreset = reverbPreset;

        // Configurar AudioSource para respiración
        breathingSource = gameObject.AddComponent<AudioSource>();
        
        // Ajustes 3D
        breathingSource.spatialBlend = 1f;           // Sonido totalmente 3D
        breathingSource.rolloffMode = AudioRolloffMode.Linear;
        breathingSource.maxDistance = 15f;
        breathingSource.loop = true;                // Loop infinito
    }

    private void InitializeBreathing()
    {
        breathingSource.clip = breathingSound;
        breathingSource.Play();
    }
}