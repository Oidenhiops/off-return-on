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
    [SerializeField] private Camera entityCamera;
    [SerializeField] private AudioClip entityRoarSFX;
    [SerializeField] private AudioClip entityDangerSFX;

    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.05f;   // Intensidad base
    [SerializeField] private float shakeFrequency = 2f;      // Oscilaciones por segundo
    [SerializeField] private Vector3 positionShakeAxis = new Vector3(1, 1, 0); // Ejes afectados
    [SerializeField] private Vector3 rotationShakeAxis = new Vector3(0, 0, 1); 

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip detectionSound;

    private NavMeshAgent agent;
    private Animator animatorEntity;
    private Vector3 patrolCenter;
    private Vector3 originalCamPos;
    private Quaternion originalCamRot;
    private bool isChasing = false;
    private bool isInvestigating = false;
    private bool isInAlertState = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animatorEntity = GetComponent<Animator>();
        patrolCenter = transform.position;
        originalCamPos = entityCamera.transform.localPosition;
        originalCamRot = entityCamera.transform.localRotation;
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

    // ===== ALERTA =====
    public void OnDollScream(Vector3 dollPosition)
    {
        if (!isInAlertState)
            StartCoroutine(AlertSequence(dollPosition));
    }

    IEnumerator AlertSequence(Vector3 dollPosition)
    {
        isInAlertState = true;

        // 1. Configuración inicial
        PlaySound(entityDangerSFX);
        agent.isStopped = true;
        animatorEntity.SetBool("IsEntityAngry", true);
        PlaySound(entityRoarSFX);
        entityCamera.gameObject.SetActive(true);

        // 2. Vibración tipo onda
        float elapsed = 0f;
        Vector3 initialCamPos = entityCamera.transform.localPosition;
        Quaternion initialCamRot = entityCamera.transform.localRotation;

        while (elapsed < alertCameraTime)
        {
            // Cálculo de onda suavizada (sinusoidal)
            float wave = Mathf.Sin(elapsed * shakeFrequency * Mathf.PI * 2);
            float damp = 1 - (elapsed / alertCameraTime); // Reduce intensidad hacia el final

            // Posición
            Vector3 posOffset = new Vector3(
                wave * shakeIntensity * positionShakeAxis.x * damp,
                wave * shakeIntensity * positionShakeAxis.y * damp,
                wave * shakeIntensity * positionShakeAxis.z * damp
            );

            // Rotación
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

        // 3. Restaurar valores
        entityCamera.transform.localPosition = originalCamPos;
        entityCamera.transform.localRotation = originalCamRot;
        entityCamera.gameObject.SetActive(false);
        animatorEntity.SetBool("IsEntityAngry", false);

        // 4. Movimiento hacia la muñeca
        agent.isStopped = false;
        agent.SetDestination(dollPosition);
        agent.speed = chaseSpeed * 1.5f;
        isInvestigating = true;

        // 5. Tiempo de gracia
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

        while (timer < searchTime)
        {
            ChasePlayer();
            timer += Time.deltaTime;
            yield return null;
        }

        isInvestigating = false;
        PatrolNewPoint();
    }

    // ===== DETECCIÓN =====
    private void CheckForPlayer()
    {
        Vector3 directionToPlayer = player.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;

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
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}