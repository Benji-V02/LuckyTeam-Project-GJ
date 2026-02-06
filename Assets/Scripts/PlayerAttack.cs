using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 0.5f;

    [Header("Input")]
    [SerializeField] private KeyCode attackKey = KeyCode.Mouse0;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private bool showDebugGizmos = true;

    private float lastAttackTime;
    private bool isAttacking = false;

    private void Start()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            isAttacking = stateInfo.IsName("MeleeAttack_OneHanded");
        }

        if (Input.GetKeyDown(attackKey) && !isAttacking)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                TriggerAttack();
                lastAttackTime = Time.time;
            }
        }
    }

    private void TriggerAttack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log("Attack animation triggered!");

            // Delay útoku podľa animácie
            StartCoroutine(DelayedAttack(0.3f));
        }
        else
        {
            DealDamage();
        }
    }

    private System.Collections.IEnumerator DelayedAttack(float delay)
    {
        yield return new WaitForSeconds(delay);
        DealDamage();
    }

    public void DealDamage()
    {
        Debug.Log("DealDamage called!");
        Attack();
    }

    private void Attack()
    {
        // Nájdi všetky objekty v dosahu
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, attackRange);

        Debug.Log($"Počet objektov v dosahu: {nearbyObjects.Length}");

        Enemy closestEnemy = null;
        float closestDistance = attackRange;

        foreach (Collider col in nearbyObjects)
        {
            Debug.Log($"Nájdený objekt: {col.name}, Tag: {col.tag}");

            // Kontrola tagu "Enemy"
            if (col.CompareTag("Enemy"))
            {
                Debug.Log($"Objekt {col.name} má správny tag!");

                Enemy enemy = col.GetComponent<Enemy>();
                if (enemy != null)
                {
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);
                    Debug.Log($"Vzdialenosť k {col.name}: {distance}");

                    // Len vzdialenosť, žiadne uhly
                    if (distance <= attackRange && distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
                else
                {
                    Debug.LogWarning($"Objekt {col.name} má tag Enemy, ale nemá Enemy komponent!");
                }
            }
        }

        // Útok na najbližšieho nepriateľa
        if (closestEnemy != null)
        {
            Debug.Log($"Útočím na: {closestEnemy.name}");
            closestEnemy.TakeDamage(damage);

            // DAMAGE POPUP
            if (DamagePopupManager.Instance != null)
            {
                DamagePopupManager.Instance.ShowDamage(
                    closestEnemy.transform.position,
                    damage,
                    Color.red
                );
                CameraShake.Instance.Shake(0.15f, 0.2f);
            }
            else
            {
                Debug.LogWarning("DamagePopupManager.Instance je NULL!");
            }

            Debug.Log($"HIT {closestEnemy.name} - Damage: {damage}");
        }
        else
        {
            Debug.Log("Žiadny nepriateľ v dosahu!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // Dosah útoku - sféra
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}