using UnityEngine;
using System.Collections;

public class HorrorDollManager : MonoBehaviour
{
    [Header("Doll Settings")]
    [SerializeField] private float spawnInterval = 60f;
    [SerializeField] private float reactionTime = 5f;
    [SerializeField] private float spawnDistanceBehind = 2f;

    [Header("References")]
    [SerializeField] private GameObject dollPrefab;
    [SerializeField] private AudioClip dollLaughSFX;
    [SerializeField] private AudioClip dollScreamSFX;
    [SerializeField] private FlashLigth flashlight;
    [SerializeField] private EntityController entityController;
    [SerializeField] Transform playerModel;
    public float rangeDistToSpawn;
    private GameObject currentDoll;
    private bool isDollActive = false;

    void Start()
    {
        StartCoroutine(SpawnDollRoutine());
    }

    // Spawneamos la Muñeca detrás del player y activamos el metodo para poder destruirla por 5 segundos
    IEnumerator SpawnDollRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);
            
            Vector3 spawnPos = DollSpawnPos();
            
            currentDoll = Instantiate(dollPrefab, spawnPos, Quaternion.identity);
            currentDoll.transform.LookAt(transform);

            AudioSource.PlayClipAtPoint(dollLaughSFX, playerModel.transform.position);
            isDollActive = true;

            StartCoroutine(DollThreatRoutine());
        }
    }
    public Vector3 DollSpawnPos()
    {        
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -playerModel.transform.forward, out RaycastHit hit, rangeDistToSpawn))
        {
            return hit.point;
        }
        return playerModel.position - playerModel.forward * spawnDistanceBehind;
    }
    // Si apuntamos con la linterna y esta encendida, Destruimos la Muñeca y no llama a la entidad
    IEnumerator DollThreatRoutine()
    {
        float timer = 0f;

        while (timer < reactionTime)
        {
            if (flashlight._light.enabled && IsDollHitByLight())
            {
                Destroy(currentDoll);
                isDollActive = false;
                yield break;
            }

            timer += Time.deltaTime;
            yield return null;
        }

        if (isDollActive)
        {
            // Alertamos a la entidad que venga hacia nosotros
            AudioSource.PlayClipAtPoint(dollScreamSFX, playerModel.position);
            entityController.OnDollScream(currentDoll.transform.position);
            Destroy(currentDoll);
            isDollActive = false;
        }
    }

    //Raycast para apuntar a la muñeca con la linterna
    private bool IsDollHitByLight()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, flashlight._light.enabled ? flashlight._light.range : 0))
        {
            return hit.collider.CompareTag("Doll");
        }
        return false;
    }
    void OnDrawGizmos()
    {
        float rangeRay = flashlight._light.enabled ? flashlight._light.range : 0;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * rangeRay);

        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, -playerModel.transform.forward, out RaycastHit hit, rangeDistToSpawn))
        {
            Gizmos.color = Color.cyan;
        }
        else
        {
            Gizmos.color = Color.blue;
        }
        Gizmos.DrawRay(transform.position + Vector3.up * 0.5f, -playerModel.transform.forward * rangeDistToSpawn);
    }
}