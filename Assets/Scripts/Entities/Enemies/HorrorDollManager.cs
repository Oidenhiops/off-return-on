using UnityEngine;
using System.Collections;

public class HorrorDollManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float spawnInterval = 60f; // Tiempo entre apariciones
    [SerializeField] private float reactionTime = 5f; // Segundos para reaccionar
    [SerializeField] private float spawnDistanceBehind = 2f; // Distancia detrás del jugador

    [Header("References")]
    [SerializeField] private GameObject dollPrefab;
    [SerializeField] private GameObject evilEntityPrefab;
    [SerializeField] private AudioClip dollLaughSFX;
    [SerializeField] private Light flashlight;


    private GameObject currentDoll;
    private bool isDollActive = false;

    void Start()
    {
        StartCoroutine(SpawnDollRoutine());
    }

    IEnumerator SpawnDollRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            // Spawn de la muñeca detrás del jugador
            Vector3 spawnPos = transform.position - transform.forward * spawnDistanceBehind;
            currentDoll = Instantiate(dollPrefab, spawnPos, Quaternion.identity);
            currentDoll.transform.LookAt(transform); // La muñeca mira al jugador

            // Sonido de risa
            GameManager.Instance.PlayASound(dollLaughSFX);

            isDollActive = true;
            StartCoroutine(DollThreatRoutine());
        }
    }

    IEnumerator DollThreatRoutine()
    {
        float timer = 0f;
        bool isEntitySpawned = false;

        while (timer < reactionTime && !isEntitySpawned)
        {
            // Verificar si el jugador apunta a la muñeca con la linterna
            if (flashlight.enabled && IsDollHitByLight())
            {
                Destroy(currentDoll);
                isDollActive = false;
                yield break; // Terminar la corrutina
            }

            timer += Time.deltaTime;
            yield return null;
        }

        // Si no se destruyó la muñeca en 5 segundos
        if (isDollActive)
        {
            SpawnEvilEntity();
            Destroy(currentDoll);
            isDollActive = false;
        }
    }

    private bool IsDollHitByLight()
    {
        RaycastHit hit;
        Vector3 rayOrigin = flashlight.transform.position;
        Vector3 rayDirection = flashlight.transform.forward;

        if (Physics.Raycast(rayOrigin, rayDirection, out hit, flashlight.range))
        {
            if (hit.collider.CompareTag("Doll"))
            {
                return true;
            }
        }
        return false;
    }

    private void SpawnEvilEntity()
    {
        // Spawn en un punto aleatorio alrededor del jugador
        Vector3 randomPos = transform.position + Random.insideUnitSphere * 5f;
        randomPos.y = transform.position.y; // Mantener altura del jugador
        Instantiate(evilEntityPrefab, randomPos, Quaternion.identity);
    }
}