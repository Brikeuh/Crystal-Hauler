using UnityEngine;

public class InteractiveRock : MonoBehaviour
{
    public Transform rockMesh;
    public GameObject bugPrefab;
    public Transform bugSpawnPoint;
    public float triggerDistance = 4f;
    public float shakeIntensity = 0.1f;
    public float shakeDuration = 0.5f;
    public Transform player;

    bool hasTriggered = false;
    Vector3 originalPos;

    void Start()
    {
        if (rockMesh == null) rockMesh = transform;
        originalPos = rockMesh.localPosition;
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (hasTriggered || player == null) return;

        float dist = Vector3.Distance(player.position, transform.position);
        if (dist <= triggerDistance)
        {
            hasTriggered = true;
            StartCoroutine(ShakeAndSpawn());
        }
    }

    System.Collections.IEnumerator ShakeAndSpawn()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector3 offset = Random.insideUnitSphere * shakeIntensity;
            rockMesh.localPosition = originalPos + offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        rockMesh.localPosition = originalPos;

        if (bugPrefab != null)
        {
            Vector3 spawnPos = bugSpawnPoint != null ? bugSpawnPoint.position : transform.position + Vector3.up * 0.2f;
            Instantiate(bugPrefab, spawnPos, Quaternion.identity);
        }
    }
}
