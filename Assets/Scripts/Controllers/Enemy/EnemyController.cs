using JetBrains.Annotations;
using System.Collections;
using Unity.Services.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Terrain terrain;

    [Header("Character Stats")]
    [SerializeField] private float health = 100f;
    [SerializeField] private float attackDamage = 10f;

    [Header("Movement Settings")]
    [SerializeField] public float speed = 2f;
    public float stopRadius = 1.25f; // Stopping distance from target
    private float runSpeed;
    private float runModifier = 1.5f;
    public bool canMove = true;

    [Header("Detection Distances")]
    public float chaseDistance = 7.5f; // Chase distance
    public float crystalDetectionRange = 5f; // Range to detect crystals

    [Header("Attack Settings")]
    public float attackAcceleration = 5f; // Speed when lunging
    public float attackCooldown = 1f;

    [Header("Crystal Settings")]
    public float crystalConsumptionRange = 2f; // How close to be to consume crystal
    public float consumptionTime = 1f; // Time to consume crystal

    [Header("Wandering Settings")]
    public float wanderRadius = 20f; // How far to wander
    public float wanderTimer = 5f; // Time before choosing new wander point

    // Colors for different states
    private Color wanderingColor = Color.green;
    private Color chasingColor = Color.yellow;
    private Color attackingColor = Color.red;
    private Color consumingColor = Color.magenta;

    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private GameObject targetCrystal;
    private GameObject hurtBox;
    private Renderer stateIndicator;
    private Coroutine stunCoroutine;

    private float timer;
    private float attackTimer;
    private Vector3 wanderPoint;
    
    private float consumptionTimer = 0f;
    private bool isConsuming = false;

    private bool isStunned = false;

    private enum EnemyState { Wandering, Chasing, Attacking, ConsumingCrystal}
    private EnemyState currentState = EnemyState.Wandering;

    public float AttackDamage => attackDamage;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        hurtBox = this.transform.GetChild(0).gameObject;
        stateIndicator = this.transform.GetChild(1).GetComponent<Renderer>();

        runSpeed = speed * runModifier;
        navMeshAgent.speed = speed;
        timer = wanderTimer;
        attackTimer = attackCooldown;

        SetNewWanderPoint();
        UpdateColor();
    }

    void Update()
    {
        if (player == null) return;

        // Update attack cooldown
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
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

        if (!isStunned)
        {
            // Priority: Attacking > Chasing > Wandering > Crystal (lowest)
            if (distanceToPlayer <= stopRadius)
            {
                currentState = EnemyState.Attacking;
                // Cancel crystal consumption if player gets too close
                if (targetCrystal != null)
                {
                    targetCrystal = null;
                    isConsuming = false;
                }
            }
            else if (distanceToPlayer <= chaseDistance && canMove)
            {
                currentState = EnemyState.Chasing;
                // Cancel crystal consumption if player is in chase range
                if (targetCrystal != null)
                {
                    targetCrystal = null;
                    isConsuming = false;
                }
            }
            else if (targetCrystal != null && canMove)
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
            else if (canMove)
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
                default:
                    Wander();
                    break;
            }
        }
    }

    void Wander()
    {
        timer += Time.deltaTime;

        SetWalk();

        if (timer >= wanderTimer || navMeshAgent.remainingDistance < 0.5f)
        {
            SetNewWanderPoint();
            timer = 0;
        }
        navMeshAgent.stoppingDistance = 1f;
        AlignRotation(wanderPoint);
        navMeshAgent.SetDestination(wanderPoint);
    }

    void Chase()
    {
        SetRun();
        navMeshAgent.stoppingDistance = stopRadius;
        AlignRotation(player.position);
        navMeshAgent.SetDestination(player.position);
    }

    void Attack()
    {
        // Look at player
        navMeshAgent.stoppingDistance = stopRadius;
        AlignRotation(player.position);

        canMove = false;
        
        // Start new attack if cooldown is ready
        if (attackTimer <= 0)
        {
            PerformAttack();
            ClearHurtbox();
            canMove = true;
        }
    }

    void PerformAttack()
    {
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);
        attackTimer = attackCooldown; // Reset the attack timer
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
        SetWalk();
        AlignRotation(targetCrystal.transform.position);
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

    void SetWalk()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
        animator.SetFloat("moveSpeedModifier", 1f);
        navMeshAgent.speed = speed;
    }

    void SetRun()
    {
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
        animator.SetFloat("moveSpeedModifier", runModifier);
        navMeshAgent.speed = runSpeed;
    }

    public void ToggleAttack()
    {
        hurtBox.SetActive(!hurtBox.activeSelf);
    }

    public void TakeDamage(float damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
        {
            Die();
        }
        StartCoroutine(StunCountdown());
        Debug.Log($"{gameObject.name} took {damageAmount} damage. Current health: {health}");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Implement death logic (e.g., disable GameObject, play death animation)
        Destroy(gameObject);
    }

    void ClearHurtbox()
    {
        hurtBox.SetActive(false);
    }

    void AlignRotation(Vector3 objectPosition)
    {
        Vector3 direction = (objectPosition - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
    }

    void UpdateColor()
    {
        if (stateIndicator == null) return;

        switch (currentState)
        {
            case EnemyState.Wandering:
                stateIndicator.material.color = wanderingColor;
                break;
            case EnemyState.Chasing:
                stateIndicator.material.color = chasingColor;
                break;
            case EnemyState.Attacking:
                stateIndicator.material.color = attackingColor;
                break;
            case EnemyState.ConsumingCrystal:
                stateIndicator.material.color = consumingColor;
                break;
        }
    }

    // Visualize detection ranges in editor
    private void OnDrawGizmosSelected()
    {
        // Chase distance (d1) - Yellow
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        // Attack distance (d2) - Red
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopRadius);

        // Crystal detection range - Magenta
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, crystalDetectionRange);

        // Wander radius - Blue
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);
    }

    public void StunEnemy()
    {
        if (stunCoroutine != null)
        {
            StopCoroutine(StunCountdown());
        }

        stunCoroutine = StartCoroutine(StunCountdown());
    }

    IEnumerator StunCountdown()
    {
        isStunned = true;

        if (navMeshAgent != null) 
        {
            Stunned();
        }

        yield return new WaitForSeconds(3f);

        if (navMeshAgent != null)
        {
            EndStun();
        }

        isStunned = false;
        stunCoroutine = null;
    }
    
    public void Stunned()
    {
        isStunned = true;
        navMeshAgent.isStopped = true;
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.speed = 0f;
        animator.SetBool("isStunned", true);
    }

    public void EndStun()
    {
        isStunned = false;
        navMeshAgent.speed = speed;
        navMeshAgent.isStopped = false;
        animator.SetBool("isStunned", false);
    }
}
