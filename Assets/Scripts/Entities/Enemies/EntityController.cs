using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EntityController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float chaseSpeed = 6f;
    [SerializeField] private float patrolRadius = 20f;
    [SerializeField] private float visionRange = 10f;
    [SerializeField] private float visionAngle = 60f;

    [Header("Alert Settings")]
    [SerializeField] private float alertCameraTime = 2f;
    [SerializeField] private float gracePeriod = 10f;
    [SerializeField] private float delayBeforeChase = 15f; // Tiempo de espera antes de moverse
    [SerializeField] private Camera entityCamera;
    [SerializeField] private AudioClip entityRoarSFX;
    [SerializeField] private AudioClip entityDangerSFX;

    [Header("Detección por Altura")]
    [SerializeField] private FlashLigth flashlight;   
    [SerializeField] private PlayerMovement playerMovement;     // Linterna del jugador

    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.05f;
    [SerializeField] private float shakeFrequency = 2f;
    [SerializeField] private Vector3 positionShakeAxis = new Vector3(1, 1, 0);
    [SerializeField] private Vector3 rotationShakeAxis = new Vector3(0, 0, 1);

    [Header("Death Settings")]
    [SerializeField] private float deathDelay = 4f; // Tiempo final antes del mensaje
    [SerializeField] private AudioClip gameOverSFX; // Sonido Game Over

    private Vector3 deathCamInitialPosition; // Posición inicial de la cámara de screamer

    private Vector3 initialCameraPosition;
    private Quaternion initialCameraRotation;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject playerFull;


    // Variables privadas
    private NavMeshAgent agent;
    private Animator animatorEntity;
    private Vector3 patrolCenter;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private Vector3 lastKnownPlayerPosition; // Última posición registrada del jugador
    private bool isDead = false; // Variable para controlar si el jugador ya está muerto
    private bool isChasing = false;
    private bool isInvestigating = false;
    private bool isInAlertState = false;
    private bool startHunt = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorEntity = GetComponent<Animator>();
        patrolCenter = transform.position;
        originalCamPos = entityCamera.transform.localPosition;
        originalCamRot = entityCamera.transform.localRotation;        
        GameManager.Instance.OnStartGame += OnGameStart;
    }
    void OnDestroy()
    {
        GameManager.Instance.OnStartGame -= OnGameStart;
    }
    public void OnGameStart(bool startGame)
    {
        if (!startHunt)
        {
            startHunt = true;
            PatrolNewPoint();
        }
    }
    void Update()
    {
        if (startHunt)
        {
            if (isChasing)
            {
                ChasePlayer();
            }
            else if (isInvestigating)
            {
                InvestigateArea();
            }
            else
            {
                Patrol();
            }

            CheckForPlayer();
        }
    }

    // ===== PATRULLA =====
    private void Patrol()
    {
        if (agent.remainingDistance < 1f)
        {
            PatrolNewPoint();
        }
    }

    private void PatrolNewPoint()
    {
        Vector3 randomPoint = patrolCenter + Random.insideUnitSphere * patrolRadius;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas);
        agent.SetDestination(hit.position);
        agent.speed = patrolSpeed;
        animatorEntity.SetBool("IsEntityWalk", true);
        isChasing = false;
    }

    // ===== ALERTA =====
    public void OnDollScream(Vector3 dollPosition)
    {
        if (!isInAlertState)
        {
            lastKnownPlayerPosition = player.position; // Guardamos la posición ACTUAL del jugador
            StartCoroutine(AlertSequence(dollPosition));
        }
    }

    IEnumerator AlertSequence(Vector3 dollPosition)
    {
        isInAlertState = true;

        // 1. Animación y efectos iniciales
        PlaySound(entityDangerSFX);
        agent.isStopped = true;
        animatorEntity.SetBool("IsEntityAngry", true);
        entityCamera.gameObject.SetActive(true);

        // 2. Vibración de cámara o efecto shaking
        float elapsed = 0f;
        Vector3 initialCamPos = entityCamera.transform.localPosition;
        Quaternion initialCamRot = entityCamera.transform.localRotation;

        while (elapsed < alertCameraTime)
        {
            float wave = Mathf.Sin(elapsed * shakeFrequency * Mathf.PI * 2);
            float damp = 1 - (elapsed / alertCameraTime);

            Vector3 posOffset = new Vector3(
                wave * shakeIntensity * positionShakeAxis.x * damp,
                wave * shakeIntensity * positionShakeAxis.y * damp,
                wave * shakeIntensity * positionShakeAxis.z * damp
            );

            Vector3 rotOffset = new Vector3(
                wave * shakeIntensity * 10 * rotationShakeAxis.x * damp,
                wave * shakeIntensity * 10 * rotationShakeAxis.y * damp,
                wave * shakeIntensity * 10 * rotationShakeAxis.z * damp
            );

            entityCamera.transform.localPosition = initialCamPos + posOffset;
            entityCamera.transform.localRotation = initialCamRot * Quaternion.Euler(rotOffset);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3. Restaurar cámara luego del efecto shaking
        entityCamera.transform.localPosition = originalCamPos;
        entityCamera.transform.localRotation = originalCamRot;
        entityCamera.gameObject.SetActive(false);
        animatorEntity.SetBool("IsEntityAngry", false);

        // 4. Esperar 15s antes de moverse
        yield return new WaitForSeconds(delayBeforeChase);

        // 5. Moverse a la última posición conocida del jugador
        agent.isStopped = false;
        agent.SetDestination(lastKnownPlayerPosition);
        agent.speed = chaseSpeed * 0.8f; // Velocidad reducida para "buscar"
        isInvestigating = true;

        // 6. Tiempo de búsqueda activa
        yield return new WaitForSeconds(gracePeriod);
        StartCoroutine(SearchPlayerRoutine());
        isInAlertState = false;
    }

    // ===== INVESTIGACIÓN =====
    private void InvestigateArea()
    {
        if (agent.remainingDistance < 2f)
        {
            StartCoroutine(SearchPlayerRoutine());
        }
    }

    IEnumerator SearchPlayerRoutine()
    {
        float searchTime = 10f;
        float timer = 0f;
        Vector3 searchCenter = lastKnownPlayerPosition;

        while (timer < searchTime)
        {
            if (playerMovement.CanStandUp())
            {
                ChasePlayer();
            }
            else
            {
                // Buscarmos en puntos aleatorios cerca de la última posición
                if (agent.remainingDistance < 1f)
                {
                    Vector3 randomPoint = searchCenter + Random.insideUnitSphere * 5f;
                    NavMeshHit hit;
                    NavMesh.SamplePosition(randomPoint, out hit, 5f, NavMesh.AllAreas);
                    agent.SetDestination(hit.position);
                }
            }

            timer += Time.deltaTime;
            yield return null;
        }

        isInvestigating = false;
        PatrolNewPoint();
    }

    // ===== DETECCIÓN DEL PLAYER =====
    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Verificamos condiciones de escondite
        bool isFlashlightOn = flashlight.transform.root.CompareTag("Player") && flashlight._light.enabled;

        if (distanceToPlayer < visionRange && playerMovement.CanStandUp() || distanceToPlayer < visionRange && isFlashlightOn)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer.normalized);
            if (angle < visionAngle / 2)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToPlayer, out hit, visionRange))
                {
                    if (hit.collider.CompareTag("Player"))
                    {
                        isChasing = true;
                    }
                }
            }
        }

        if (distanceToPlayer < 2f && isChasing)
        {
            PlayerDeath();// Activaremos la Animacion Screemer de Muerte
        }
    }

    private void ChasePlayer()
    {
        if (playerMovement.CanStandUp()) // Solo persigue si el jugador no está escondido
        {
            agent.SetDestination(player.position);
            agent.speed = chaseSpeed;
        }
    }

   private void PlayerDeath()
    {
        if (isDead) return; // Si ya está muerto, no hacer nada
        isDead = true;
        
        Debug.Log("¡Jugador eliminado!");
        agent.isStopped = true;
        animatorEntity.SetBool("IsEntityAttack", true);

        // 1. Desactivo la mano
        playerFull.gameObject.SetActive(false);
        // 2. Activar y configurar cámara de screamer
        entityCamera.gameObject.SetActive(true);
        // 3. Activar Sonido de Game Over
        PlaySound(gameOverSFX); // COrregir se instancian como 1000 audios xD

        StartCoroutine(DeathCameraSequence());
    }
    IEnumerator DeathCameraSequence()
    {
        yield return new WaitForSeconds(deathDelay);
        // 3. Esperar y finalizar
        Debug.Log("Jugador eliminado - Game Over");
        GameManager.Instance.ChangeSceneSelector(GameManager.TypeScene.GameOverScene);
       
    }


    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}