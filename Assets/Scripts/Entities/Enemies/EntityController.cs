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
    [SerializeField] private float alertCameraTime = 2f;       // Tiempo de animación + cámara
    [SerializeField] private float gracePeriod = 10f;          // Tiempo para esconderse
    [SerializeField] private Camera entityCamera;              // Cámara de la entidad
    [SerializeField] private AudioClip entityRoarSFX;          // Sonido de alerta
    [SerializeField] private AudioClip suspenseSFX;          // Sonido de suspenso 12 segundos

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip detectionSound;

    private NavMeshAgent agent;
    private Animator animatorEntity;
    private Vector3 patrolCenter;
    private bool isChasing = false;
    private bool isInvestigating = false;
    private bool isInAlertState = false;                       // Evita superposición de alertas

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorEntity = GetComponent<Animator>();
        patrolCenter = transform.position;
        PatrolNewPoint();
    }

    void Update()
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
    }

    // ===== ALERTA POR GRITO DE MUÑECA =====
    public void OnDollScream(Vector3 dollPosition)
    {
        if (!isInAlertState)
            StartCoroutine(AlertSequence(dollPosition));
    }

    IEnumerator AlertSequence(Vector3 dollPosition)
    {
        isInAlertState = true;

        // 1. Detener movimiento y activar animación/cámara
        PlaySound(suspenseSFX);
        agent.isStopped = true;
        animatorEntity.SetBool("IsEntityAngry", true);
        PlaySound(entityRoarSFX);
        entityCamera.gameObject.SetActive(true);

        // 2. Esperar tiempo de la animación
        yield return new WaitForSeconds(alertCameraTime);

        // 3. Desactivar cámara y reiniciar animación
        entityCamera.gameObject.SetActive(false);
        animatorEntity.SetBool("IsEntityAngry", false);

        // 4. Movimiento hacia la muñeca
        agent.isStopped = false;
        agent.SetDestination(dollPosition);
        agent.speed = chaseSpeed * 1.5f;  // Velocidad aumentada
        isInvestigating = true;

        // 5. Esperar tiempo de gracia (10s)
        yield return new WaitForSeconds(gracePeriod);

        // 6. Iniciar búsqueda activa
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

        while (timer < searchTime)
        {
            ChasePlayer();
            timer += Time.deltaTime;
            yield return null;
        }

        isInvestigating = false;
        PatrolNewPoint();
    }

    // ===== DETECCIÓN DEL JUGADOR =====
    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

        // Detección por visión
        if (distanceToPlayer < visionRange)
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

        // Detección por proximidad
        if (distanceToPlayer < 2f)
        {
            PlayerDeath();
        }
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        agent.speed = chaseSpeed;
    }

    private void PlayerDeath()
    {
        Debug.Log("¡Jugador eliminado!");
        // Lógica de Game Over aquí
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}