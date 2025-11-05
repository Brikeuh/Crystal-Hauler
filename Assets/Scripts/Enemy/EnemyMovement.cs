using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Terrain terrain;

    [Header("Movement Settings")]
    public float s1 = 3.5f; // Enemy speed

    [Header("Detection Distances")]
    public float d1 = 10f; // Chase distance
    public float d2 = 2f; // Attack distance
    public float crystalDetectionRange = 15f; // Range to detect crystals

    [Header("Attack Settings")]
    public float t1 = 1.5f; // Attack cooldown (interval between collisions)
    public float attackAcceleration = 10f; // Speed when lunging
    public float retreatDistance = 3f; // How far to retreat after attack

    [Header("Crystal Settings")]
    public float crystalConsumptionChance = 0.8f; // 80% chance to consume crystal
    public float crystalConsumptionRange = 2f; // How close to be to consume crystal
    public float consumptionTime = 1f; // Time to consume crystal

    [Header("Wandering Settings")]
    public float wanderRadius = 20f; // How far to wander
    public float wanderTimer = 5f; // Time before choosing new wander point

    [Header("State Colors")]
    public Color wanderingColor = Color.green;
    public Color chasingColor = Color.yellow;
    public Color attackingColor = Color.red;
    public Color consumingColor = Color.magenta;

    private NavMeshAgent navMeshAgent;
    private Renderer enemyRenderer;
    private float timer;
    private float attackCooldownTimer;
    private Vector3 wanderPoint;
    private Vector3 retreatPoint;
    private bool isLunging = false;
    private bool isRetreating = false;
    private GameObject targetCrystal;
    private float consumptionTimer = 0f;
    private bool isConsuming = false;

    private enum EnemyState { Wandering, Chasing, Attacking, ConsumingCrystal }
    private EnemyState currentState = EnemyState.Wandering;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        enemyRenderer = GetComponent<Renderer>();
        navMeshAgent.speed = s1;
        timer = wanderTimer;
        attackCooldownTimer = 0f;

        // Auto-find terrain if not assigned
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        // Auto-find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        SetNewWanderPoint();
        UpdateColor();
    }

    void Update()
    {
        if (player == null) return;

        // Update attack cooldown
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check for nearby crystals (only if not already consuming and no target)
        if (currentState != EnemyState.ConsumingCrystal && targetCrystal == null)
        {
            GameObject nearbyCrystal = FindNearbyCrystal();
            if (nearbyCrystal != null)
            {
                targetCrystal = nearbyCrystal;
            }
        }

        // State transitions
        EnemyState previousState = currentState;

        // Priority: Attacking > Chasing > Wandering > Crystal (lowest)
        if (distanceToPlayer <= d2)
        {
            currentState = EnemyState.Attacking;
            // Cancel crystal consumption if player gets too close
            if (targetCrystal != null)
            {
                targetCrystal = null;
                isConsuming = false;
            }
        }
        else if (distanceToPlayer <= d1)
        {
            currentState = EnemyState.Chasing;
            // Cancel crystal consumption if player is in chase range
            if (targetCrystal != null)
            {
                targetCrystal = null;
                isConsuming = false;
            }
        }
        else if (targetCrystal != null)
        {
            // Go for crystal only if player is far away
            float distanceToCrystal = Vector3.Distance(transform.position, targetCrystal.transform.position);
            if (distanceToCrystal <= crystalConsumptionRange)
            {
                currentState = EnemyState.ConsumingCrystal;
                isConsuming = true;
                consumptionTimer = 0f;
            }
            else
            {
                // Move towards crystal if decided to consume it
                currentState = EnemyState.ConsumingCrystal;
            }
        }
        else
        {
            currentState = EnemyState.Wandering;
        }

        // Update color if state changed
        if (previousState != currentState)
        {
            UpdateColor();
        }

        // State behaviors
        switch (currentState)
        {
            case EnemyState.Wandering:
                Wander();
                break;
            case EnemyState.Chasing:
                Chase();
                break;
            case EnemyState.Attacking:
                Attack();
                break;
            case EnemyState.ConsumingCrystal:
                ConsumeCrystal();
                break;
        }
    }

    void Wander()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer || navMeshAgent.remainingDistance < 0.5f)
        {
            SetNewWanderPoint();
            timer = 0;
        }

        navMeshAgent.SetDestination(wanderPoint);
    }

    void Chase()
    {
        navMeshAgent.SetDestination(player.position);
    }

    void Attack()
    {
        // Look at player
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

        // Handle lunging animation
        if (isLunging)
        {
            // Accelerate towards player
            navMeshAgent.speed = attackAcceleration;
            navMeshAgent.SetDestination(player.position);

            // Check if reached player
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.1f)
            {
                PerformAttack();
                isLunging = false;
                isRetreating = true;

                // Calculate retreat point
                Vector3 retreatDirection = (transform.position - player.position).normalized;
                retreatPoint = transform.position + retreatDirection * retreatDistance;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(retreatPoint, out hit, retreatDistance, NavMesh.AllAreas))
                {
                    retreatPoint = hit.position;
                }
            }
        }
        else if (isRetreating)
        {
            // Retreat back
            navMeshAgent.speed = s1;
            navMeshAgent.SetDestination(retreatPoint);

            // Check if reached retreat point
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance + 0.1f)
            {
                isRetreating = false;
                attackCooldownTimer = t1;
            }
        }
        else
        {
            // Wait for cooldown
            navMeshAgent.SetDestination(transform.position);
            navMeshAgent.speed = s1;

            // Start new attack if cooldown is ready
            if (attackCooldownTimer <= 0)
            {
                isLunging = true;
            }
        }
    }

    void PerformAttack()
    {
        // Perform attack logic here (e.g., deal damage, play animation)
        Debug.Log("Enemy collides with player!");

        // Example: You can add damage to player here
        // player.GetComponent<PlayerHealth>()?.TakeDamage(damageAmount);
    }

    void SetNewWanderPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            wanderPoint = hit.position;

            // Ensure the point is within terrain bounds if terrain exists
            if (terrain != null)
            {
                Vector3 terrainPos = terrain.transform.position;
                TerrainData terrainData = terrain.terrainData;

                wanderPoint.x = Mathf.Clamp(wanderPoint.x, terrainPos.x, terrainPos.x + terrainData.size.x);
                wanderPoint.z = Mathf.Clamp(wanderPoint.z, terrainPos.z, terrainPos.z + terrainData.size.z);
                wanderPoint.y = terrain.SampleHeight(wanderPoint) + terrainPos.y;
            }
        }
    }

    GameObject FindNearbyCrystal()
    {
        GameObject[] crystals = GameObject.FindGameObjectsWithTag("Crystal");
        GameObject nearestCrystal = null;
        float nearestDistance = crystalDetectionRange;

        foreach (GameObject crystal in crystals)
        {
            float distance = Vector3.Distance(transform.position, crystal.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestCrystal = crystal;
            }
        }

        return nearestCrystal;
    }

    void ConsumeCrystal()
    {
        if (targetCrystal == null)
        {
            // Crystal was destroyed or disappeared, return to wandering
            currentState = EnemyState.Wandering;
            isConsuming = false;
            return;
        }

        float distanceToCrystal = Vector3.Distance(transform.position, targetCrystal.transform.position);

        // Move towards crystal
        navMeshAgent.speed = s1;
        navMeshAgent.SetDestination(targetCrystal.transform.position);

        // Consume immediately when in range
        if (distanceToCrystal <= crystalConsumptionRange)
        {
            Debug.Log("Enemy consumed crystal!");
            Destroy(targetCrystal);
            targetCrystal = null;
            isConsuming = false;
            currentState = EnemyState.Wandering;
        }
    }

    void UpdateColor()
    {
        if (enemyRenderer == null) return;

        switch (currentState)
        {
            case EnemyState.Wandering:
                enemyRenderer.material.color = wanderingColor;
                break;
            case EnemyState.Chasing:
                enemyRenderer.material.color = chasingColor;
                break;
            case EnemyState.Attacking:
                enemyRenderer.material.color = attackingColor;
                break;
            case EnemyState.ConsumingCrystal:
                enemyRenderer.material.color = consumingColor;
                break;
        }
    }

    // Visualize detection ranges in editor
    private void OnDrawGizmosSelected()
    {
        // Chase distance (d1) - Yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, d1);

        // Attack distance (d2) - Red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, d2);

        // Crystal detection range - Magenta
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, crystalDetectionRange);

        // Wander radius - Blue
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }
}
