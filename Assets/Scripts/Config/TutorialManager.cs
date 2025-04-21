using UnityEngine;
using TMPro;
using Unity.Cinemachine; // Namespace correcto para Cinemachine
using System.Collections;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public GameObject skipButton;
    [System.Serializable]
    public class TutorialStep
    {
        public CinemachineCamera vcam; // Usar CinemachineVirtualCamera (no CinemachineCamera)
        public int idText;
    }
    public GameObject canvas;
    public TMP_Text textComponent;
    public UnityEngine.UI.Button continueButton;
    [Header("Configuración")]
    [SerializeField] private TutorialStep[] steps;
    [SerializeField] private float typingSpeed = 30f;

    [Header("Cámara del Jugador")]
    [SerializeField] private CinemachineCamera playerCamera; // Tipo correcto
    [SerializeField] private int playerCameraPriority = 20;

    public GameObject globalVolume;

    private int currentStep = -1;
    private Coroutine typingCoroutine;
    public PlayerInputs playerInputs;

    void Start()
    {
        InitializeTutorial();
        playerInputs.playerControls.Player.Interact.started += OnSkipTutotial;
    }
    void OnDestroy()
    {
        playerInputs.playerControls.Player.Interact.started -= OnSkipTutotial;
    }
    private void InitializeTutorial()
    {

        canvas.SetActive(true);
        textComponent.text = "";
        continueButton.onClick.AddListener(NextStep);
        foreach (var step in steps)
        {
            step.vcam.Priority = 0;
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
            steps[currentStep].vcam.Priority = 0;
        }

        currentStep++;

        if (currentStep >= steps.Length)
        {
            EndTutorial();
            return;
        }

        var current = steps[currentStep];
        current.vcam.Priority = 15;
        continueButton.interactable = false;

        typingCoroutine = StartCoroutine(TypeText(current));
    }

    private IEnumerator TypeText(TutorialStep step)
    {
        textComponent.text = "";
        char[] messageArray = GameData.Instance.GetDialog(step.idText).ToCharArray();

        foreach (char c in messageArray)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(1f / typingSpeed);
        }

        continueButton.interactable = true;
    }
    public void OnSkipTutotial(InputAction.CallbackContext context)
    {
        SkipTutorial();
    }
    public void SkipTutorial()
    {
        EndTutorial();
    }
    private void EndTutorial()
    {
        playerInputs.playerControls.Player.Interact.started -= OnSkipTutotial;
        canvas.SetActive(false);
        GameManager.Instance.startGame = true;
        foreach (var step in steps)
        {
            step.vcam.Priority = 0;
            continueButton.onClick.RemoveListener(NextStep); // Limpiar listeners
        }

        if (playerCamera != null)
        {
            playerCamera.Priority = playerCameraPriority;
        }

        //globalVolume.gameObject.SetActive(true);
        Destroy(gameObject);
    }
}