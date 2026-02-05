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

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private HealthBar healthBar;

    private Rigidbody rb;

    private void Start()
    {
        // Inicializ·cia zdravia
        currentHealth = maxHealth;

        // ZÌskaj Rigidbody komponent
        rb = GetComponent<Rigidbody>();

        // Ak player nie je nastaven˝, n·jdi ho automaticky
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        // Aktualizuj healthbar
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    private void Update()
    {
        if (player == null) return;

        // VypoËÌtaj vzdialenosù k hr·Ëovi
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Ak je hr·Ë v dosahu a nie je prÌliö blÌzko
        if (distanceToPlayer <= detectionRange && distanceToPlayer > stopDistance)
        {
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        // VypoËÌtaj smer k hr·Ëovi
        Vector3 direction = (player.position - transform.position).normalized;

        // Pohyb len v X a Z osi (2D horizont·lny pohyb)
        direction.y = 0;

        // PosuÚ enemy pomocou Rigidbody (spr·vny spÙsob s fyzikou)
        if (rb != null)
        {
            Vector3 newPosition = rb.position + direction * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
        else
        {
            // Fallback ak nieje Rigidbody
            transform.position += direction * moveSpeed * Time.deltaTime;
        }

        // OtoËenie smerom k hr·Ëovi
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Aktualizuj healthbar
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        // Ak zdravie je 0, zniËÌ enemy
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Tu mÙûeö pridaù anim·ciu smrti, efekty, atÔ.
        Destroy(gameObject);
    }

    // Vizualiz·cia dosahu v editore
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}