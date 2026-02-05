using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damage = 20f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private float attackCooldown = 0.5f;
    [SerializeField] private float attackAngle = 90f; // Uhol útoku (90° = pred hráčom)

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

            // Delay útoku podľa animácie (alebo pridaj Animation Event)
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
        // Nájdi všetkých nepriateľov v dosahu
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, attackRange);

        Enemy closestEnemy = null;
        float closestDistance = attackRange;

        foreach (Collider col in nearbyEnemies)
        {
            Enemy enemy = col.GetComponent<Enemy>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                Vector3 directionToEnemy = (enemy.transform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward, directionToEnemy);

                if (distance <= attackRange && angle <= attackAngle / 2f)
                {
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestEnemy = enemy;
                    }
                }
            }
        }

        // Útok na najbližšieho nepriateľa
        if (closestEnemy != null)
        {
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

            Debug.Log($"HIT {closestEnemy.name} - Damage: {damage}");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showDebugGizmos) return;

        // Dosah útoku
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Kužeľ útoku (smer)
        Gizmos.color = Color.yellow;
        Vector3 forward = transform.forward * attackRange;

        // Ľavá a pravá strana kužeľa
        Vector3 leftBoundary = Quaternion.Euler(0, -attackAngle / 2f, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, attackAngle / 2f, 0) * forward;

        Gizmos.DrawLine(transform.position, transform.position + forward);
        Gizmos.DrawLine(transform.position, transform.position + leftBoundary);
        Gizmos.DrawLine(transform.position, transform.position + rightBoundary);
    }
}