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

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private AudioClip detectionSound;

    private NavMeshAgent agent;
    private Vector3 patrolCenter;
    private bool isChasing = false;
    private bool isInvestigating = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrolCenter = transform.position; // Punto central de patrulla
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
    }

    // ===== INVESTIGACIÓN (cuando la muñeca avisa) =====
    public void OnDollScream(Vector3 dollPosition)
    {
        isInvestigating = true;
        agent.speed = chaseSpeed;
        agent.SetDestination(dollPosition);
        PlaySound(detectionSound);
    }

    private void InvestigateArea()
    {
        if (agent.remainingDistance < 2f)
        {
            // Busca al jugador en la zona por 10 segundos
            StartCoroutine(SearchPlayerRoutine());
        }
    }

    IEnumerator SearchPlayerRoutine()
    {
        float searchTime = 10f;
        float timer = 0f;

        while (timer < searchTime)
        {
            ChasePlayer(); // Persigue al jugador si está en la zona
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

        // Detección por visión (ángulo + rango)
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
        // Aquí iría la lógica de Game Over.
    }

    private void PlaySound(AudioClip clip)
    {
        AudioSource.PlayClipAtPoint(clip, transform.position);
    }
}