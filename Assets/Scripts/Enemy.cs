using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float stopDistance = 1.5f;

    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("Combat")]
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float minDamage = 10f;
    [SerializeField] private float maxDamage = 20f;
    [SerializeField] private float attackCooldown = 1.5f;
    private float lastAttackTime = 0f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private HealthBar healthBar;

    private Rigidbody rb;
    private PlayerStats playerStats;
    private Animator animator;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // Skontroluj Animator
        if (animator == null)
        {
            Debug.LogWarning("Animator komponent nebol nájdený na Enemy!");
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                playerStats = playerObj.GetComponent<PlayerStats>();
            }
        }
        else
        {
            playerStats = player.GetComponent<PlayerStats>();
        }

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Ak je hráč v dosahu útoku
        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
        // Ak je hráč v dosahu detekcie ale nie v dosahu útoku
        else if (distanceToPlayer <= detectionRange && distanceToPlayer > stopDistance)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (rb != null)
        {
            Vector3 newPosition = rb.position + direction * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void AttackPlayer()
    {
        // Skontroluj cooldown
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            if (playerStats != null)
            {
                // DEBUG - skontroluj Animator
                if (animator != null)
                {
                    Debug.Log("✓ Animator existuje");

                    // Skontroluj či má parameter
                    bool hasParameter = false;
                    foreach (AnimatorControllerParameter param in animator.parameters)
                    {
                        if (param.name == "Attack")
                        {
                            hasParameter = true;
                            Debug.Log($"✓ Parameter 'Attack' nájdený! Typ: {param.type}");
                        }
                    }

                    if (!hasParameter)
                    {
                        Debug.LogError("✗ Parameter 'Attack' NEBOL nájdený v Animatore!");
                    }

                    // Trigger animáciu
                    animator.SetTrigger("Attack");
                    Debug.Log("→ SetTrigger('Attack') zavolaný!");
                }
                else
                {
                    Debug.LogError("✗ Animator je NULL!");
                }

                float damage = Random.Range(minDamage, maxDamage);
                playerStats.TakeDamage(damage);

                Debug.Log($"⚔️ Enemy útočí! Damage: {damage:F1}");

                lastAttackTime = Time.time;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}