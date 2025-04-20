using UnityEngine;
using TMPro;
using Unity.Cinemachine; // Namespace correcto para Cinemachine
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public GameObject canvas;             
        public CinemachineCamera vcam; // Usar CinemachineVirtualCamera (no CinemachineCamera)
        public TMP_Text textComponent;        
        public UnityEngine.UI.Button continueButton; 
        [TextArea(3, 5)] public string message; 
    }

    [Header("Configuración")]
    [SerializeField] private TutorialStep[] steps;
    [SerializeField] private float typingSpeed = 30f; 

    [Header("Cámara del Jugador")]
    [SerializeField] private CinemachineCamera playerCamera; // Tipo correcto
    [SerializeField] private int playerCameraPriority = 20; 

    public GameObject globalVolume;

    private int currentStep = -1;
    private Coroutine typingCoroutine;

    void Start()
    {
        Time.timeScale = 0f;
        InitializeTutorial();
    }

    private void InitializeTutorial()
    {
        
        foreach (var step in steps)
        {
            step.canvas.SetActive(false);
            step.vcam.Priority = 0;
            step.textComponent.text = ""; 
            // Vincular el botón al método NextStep
            step.continueButton.onClick.AddListener(NextStep); // ¡Este es el cambio clave!
        }
        NextStep();
    }

    public void NextStep()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        if (currentStep >= 0)
        {
            steps[currentStep].canvas.SetActive(false);
            steps[currentStep].vcam.Priority = 0;
            // Remover listener del botón anterior para evitar duplicados
            steps[currentStep].continueButton.onClick.RemoveListener(NextStep);
        }

        currentStep++;

        if (currentStep >= steps.Length)
        {
            EndTutorial();
            return;
        }

        var current = steps[currentStep];
        current.canvas.SetActive(true);
        current.vcam.Priority = 15;
        current.continueButton.interactable = false; 

        typingCoroutine = StartCoroutine(TypeText(current));
    }

    private IEnumerator TypeText(TutorialStep step)
    {
        step.textComponent.text = "";
        char[] messageArray = step.message.ToCharArray();

        foreach (char c in messageArray)
        {
            step.textComponent.text += c;
            yield return new WaitForSeconds(1f / typingSpeed);
        }

        step.continueButton.interactable = true; 
    }

    private void EndTutorial()
    {
        Time.timeScale = 1f;
        foreach (var step in steps)
        {
            step.canvas.SetActive(false);
            step.vcam.Priority = 0;
            step.continueButton.onClick.RemoveListener(NextStep); // Limpiar listeners
        }

        if (playerCamera != null)
        {
            playerCamera.Priority = playerCameraPriority;
        }

        globalVolume.gameObject.SetActive(true); 
        Destroy(gameObject);
    }
}