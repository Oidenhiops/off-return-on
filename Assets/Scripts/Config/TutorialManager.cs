using UnityEngine;
using TMPro;
using Unity.Cinemachine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [System.Serializable]
    public class TutorialStep
    {
        public GameObject canvas;             // Canvas del paso (contiene el TMP_Text)
        public CinemachineCamera vcam; // Cámara virtual
        public Button continueButton;         // Botón "Continuar"
    }

    [Header("Cámara del Jugador")]
    [SerializeField] private CinemachineCamera playerCamera; // Referencia a la cámara del jugador
    [SerializeField] private int playerCameraPriority = 20; // Prioridad al finalizar
    [SerializeField] private TutorialStep[] steps;
    

    private int currentStep = -1;

    void Start()
    {
        InitializeTutorial();
    }

    private void InitializeTutorial()
    {
        Time.timeScale = 0f; // Pausar el juego

        // Desactivar todos los Canvas y cámaras al inicio
        foreach (var step in steps)
        {
            step.canvas.SetActive(false);
            step.vcam.Priority = 0;
        }

        NextStep();
    }

    public void NextStep()
    {
        // Desactivar paso anterior
        if (currentStep >= 0)
        {
            steps[currentStep].canvas.SetActive(false);
            steps[currentStep].vcam.Priority = 0;
        }

        currentStep++;

        // Finalizar tutorial
        if (currentStep >= steps.Length)
        {
            EndTutorial();
            return;
        }

        // Activar nuevo paso
        var current = steps[currentStep];
        current.canvas.SetActive(true);
        current.vcam.Priority = 10; // Prioridad mayor que la cámara principal
        current.continueButton.onClick.RemoveAllListeners();
        current.continueButton.onClick.AddListener(NextStep);
    }

    private void EndTutorial()
    {
        Time.timeScale = 1f;

        // Destruir el objeto del tutorial si es necesario
        Destroy(gameObject); 

        foreach (var step in steps)
        {
            step.canvas.SetActive(false);
            step.vcam.Priority = 0;
        }

        // Activar cámara del jugador
        if (playerCamera != null)
        {
            playerCamera.Priority = playerCameraPriority;
        }
    }
}