using UnityEngine;

public class FallingRockSpawner : MonoBehaviour
{
    public GameObject rockPrefab;
    public float spawnInterval = 5f;
    public Vector2 spawnAreaSize = new Vector2(20f, 20f);

    void Start()
    {
        InvokeRepeating(nameof(SpawnRock), spawnInterval, spawnInterval);
    }

    void SpawnRock()
    {
        Vector3 offset = new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            0f,
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2)
        );

        Vector3 spawnPos = transform.position + offset;
        Instantiate(rockPrefab, spawnPos, Quaternion.identity);
    }
}
