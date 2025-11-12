using UnityEngine;
using System.Collections.Generic;

public class CrystalSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [Tooltip("Crystal prefab to spawn")]
    public GameObject crystalPrefab;

    [Tooltip("Maximum number of crystals that can exist at once")]
    public int maxCrystals = 10;

    [Tooltip("Time in seconds between spawn attempts")]
    public float spawnInterval = 5f;

    [Header("Spawn Area")]
    [Tooltip("Terrain component that defines the spawn area")]
    public Terrain terrain;
    
    [Header("Player")]
    [Tooltip("Reference to the player transform")]
    public Transform player;

    // Private variables
    private List<GameObject> spawnedCrystals = new List<GameObject>();
    private float spawnTimer = 0f;
    private Vector3 terrainSize;
    private Vector3 terrainPosition;
    private bool hasSpawnedFirst = false;

    // Minimum distance between crystals
    private float minDistanceBetweenCrystals = 8f;

    // first crystal is set to be close to player, set distance
    private float firstCrystalMaxDistance = 12f;
    private float firstCrystalMinDistance = 5f;

    // Minimum distance from player to spawn crystals
    private float minDistanceFromPlayer = 12f;

    // Preferred distance from player (weighted towards this)
    private float preferredDistanceFromPlayer = 20f;

    // Height offset above terrain to spawn crystals
    private float spawnHeightOffset = 0.5f;

    // Maximum attempts to find a valid spawn position
    private int maxSpawnAttempts = 30;

    // spawn location buffer to terrain edge
    private float edgeBuffer = 0.25f;

    void Start()
    {
        // 1. locate terrain and find terrain parameters
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        if (terrain != null)
        {
            terrainSize = terrain.terrainData.size;
            terrainPosition = terrain.transform.position;
        }
        else
        {
            Debug.LogError("CrystalSpawner: No terrain found! Please assign a terrain.");
        }

        // 2. locate player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogWarning("CrystalSpawner: No player found! Spawning without player avoidance.");
            }
        }

        // 3. locate crystal prefab
        if (crystalPrefab == null)
        {
            Debug.LogError("CrystalSpawner: No crystal prefab assigned!");
        }

        // 4. Start spawn timer at random offset to avoid all spawning at once
        spawnTimer = Random.Range(0f, spawnInterval);

        // 5. Spawn first crystal immediately 
        SpawnFirstCrystal();
    }

    void Update()
    {
        // Clean up null references (crystals that were picked up)
        spawnedCrystals.RemoveAll(crystal => crystal == null || !crystal.activeInHierarchy);

        // Increment spawn timer
        spawnTimer += Time.deltaTime;

        // Check if we should spawn a new crystal
        if (spawnTimer >= spawnInterval && spawnedCrystals.Count < maxCrystals)
        {
            TrySpawnCrystal(false);
            spawnTimer = 0f;
        }
    }

    void SpawnFirstCrystal()
    {
        if (crystalPrefab == null || terrain == null || player == null)
        {
            Debug.LogWarning("CrystalSpawner: Cannot spawn first crystal - missing references");
            return;
        }

        hasSpawnedFirst = true;
        Vector3 spawnPosition;
        bool foundValidPosition = false;

        // Try to find a valid position within visible range
        for (int attempt = 0; attempt < maxSpawnAttempts * 2; attempt++) // More attempts for first crystal
        {
            // Generate position in a ring around player
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(firstCrystalMinDistance, firstCrystalMaxDistance);

            Vector3 offset = new Vector3(Mathf.Cos(angle) * distance, 0f, Mathf.Sin(angle) * distance);
            spawnPosition = new Vector3(player.position.x + offset.x, 0f, player.position.z + offset.z);

            // Check if position is within terrain bounds
            if (IsWithinTerrainBounds(spawnPosition) && IsValidSpawnPosition(spawnPosition, true))
            {
                // Adjust height to terrain
                float terrainHeight = terrain.SampleHeight(spawnPosition);
                spawnPosition.y = terrainPosition.y + terrainHeight + spawnHeightOffset;

                // Spawn the crystal
                GameObject crystal = Instantiate(crystalPrefab, spawnPosition, Quaternion.identity);
                spawnedCrystals.Add(crystal);
                foundValidPosition = true;
                Debug.Log("CrystalSpawner: First crystal spawned at distance " + distance + " from player");
                break;
            }
        }

        if (!foundValidPosition)
        {
            Debug.LogWarning("CrystalSpawner: Could not find valid position for first crystal");
        }
    }

    void TrySpawnCrystal(bool isFirstCrystal = false)
    {
        if (crystalPrefab == null || terrain == null)
        {
            return;
        }

        Vector3 spawnPosition;
        bool foundValidPosition = false;

        // Try to find a valid spawn position
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            spawnPosition = GetRandomSpawnPosition();

            // Check if position is valid
            if (IsValidSpawnPosition(spawnPosition, isFirstCrystal))
            {
                // Adjust height to terrain
                float terrainHeight = terrain.SampleHeight(spawnPosition);
                spawnPosition.y = terrainPosition.y + terrainHeight + spawnHeightOffset;

                // Spawn the crystal
                GameObject crystal = Instantiate(crystalPrefab, spawnPosition, Quaternion.identity);
                spawnedCrystals.Add(crystal);
                foundValidPosition = true;
                break;
            }
        }

        if (!foundValidPosition)
        {
            Debug.LogWarning("CrystalSpawner: Could not find valid spawn position after " + maxSpawnAttempts + " attempts.");
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Calculate spawn bounds with edge buffer
        float minX = terrainPosition.x + (terrainSize.x * edgeBuffer);
        float maxX = terrainPosition.x + (terrainSize.x * (1f - edgeBuffer));
        float minZ = terrainPosition.z + (terrainSize.z * edgeBuffer);
        float maxZ = terrainPosition.z + (terrainSize.z * (1f - edgeBuffer));

        // If player exists, bias spawn position away from player
        if (player != null)
        {
            // Generate multiple random positions and pick the one furthest from player
            Vector3 bestPosition = Vector3.zero;
            float bestScore = float.MinValue;

            for (int i = 0; i < 3; i++)
            {
                Vector3 candidatePos = new Vector3(
                    Random.Range(minX, maxX),
                    0f,
                    Random.Range(minZ, maxZ)
                );

                float distanceToPlayer = Vector3.Distance(new Vector3(candidatePos.x, player.position.y, candidatePos.z), player.position);

                // Score based on how close to preferred distance
                float score = -Mathf.Abs(distanceToPlayer - preferredDistanceFromPlayer);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestPosition = candidatePos;
                }
            }

            return bestPosition;
        }
        else
        {
            // No player reference, just pick random position
            return new Vector3(
                Random.Range(minX, maxX),
                0f,
                Random.Range(minZ, maxZ)
            );
        }
    }

    bool IsValidSpawnPosition(Vector3 position, bool isFirstCrystal = false)
    {
        // Check distance from player
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(new Vector3(position.x, player.position.y, position.z), player.position);
            if (distanceToPlayer < minDistanceFromPlayer)
            {
                return false;
            }
        }

        // Check if too close to existing crystals
        foreach (GameObject crystal in spawnedCrystals)
        {
            if (crystal != null && crystal.activeInHierarchy)
            {
                float distanceToCrystal = Vector3.Distance(new Vector3(position.x, 0, position.z), new Vector3(crystal.transform.position.x, 0, crystal.transform.position.z));
                if (distanceToCrystal < minDistanceBetweenCrystals)
                {
                    return false;
                }
            }
        }

        return true;
    }

    bool IsWithinTerrainBounds(Vector3 position)
    {
        float minX = terrainPosition.x + (terrainSize.x * edgeBuffer);
        float maxX = terrainPosition.x + (terrainSize.x * (1f - edgeBuffer));
        float minZ = terrainPosition.z + (terrainSize.z * edgeBuffer);
        float maxZ = terrainPosition.z + (terrainSize.z * (1f - edgeBuffer));

        return position.x >= minX && position.x <= maxX && position.z >= minZ && position.z <= maxZ;
    }

    // Optional: Visualize spawn area in editor
    void OnDrawGizmosSelected()
    {
        if (terrain != null)
        {
            Vector3 size = terrain.terrainData.size;
            Vector3 pos = terrain.transform.position;

            // Draw full terrain bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(pos + new Vector3(size.x / 2, 0, size.z / 2), new Vector3(size.x, 1, size.z));

            // Draw valid spawn area (with edge buffer)
            Gizmos.color = Color.green;
            float bufferedWidth = size.x * (1f - 2f * edgeBuffer);
            float bufferedDepth = size.z * (1f - 2f * edgeBuffer);
            Vector3 bufferedCenter = pos + new Vector3(size.x / 2, 0, size.z / 2);
            Gizmos.DrawWireCube(bufferedCenter, new Vector3(bufferedWidth, 1, bufferedDepth));

            // Draw player avoidance radius
            if (player != null && Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(player.position, preferredDistanceFromPlayer);
            }
        }
    }
}
