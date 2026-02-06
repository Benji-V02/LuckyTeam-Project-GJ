using System.Collections;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionDelay = 3f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private int damage = 100;

    [Header("Optional")]
    [SerializeField] private bool destroyAfterExplosion = true;

    private void Start()
    {
        StartCoroutine(ExplodeAfterDelay());
    }

    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(explosionDelay);
        Explode();
    }

    private void Explode()
    {
        Debug.Log("💣 Bomb exploded!");
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (!hit) continue;

            // zober root objekt (aby si netrafil len collider na child objekte)
            GameObject target = hit.attachedRigidbody ? hit.attachedRigidbody.gameObject : hit.gameObject;

            if (target.CompareTag("Player"))
            {
                ApplyDamageToPlayer();
            }

            if (target.CompareTag("Enemy"))
            {
                Enemy enemy = target.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    Debug.Log($"💣 Explosion: {enemy.name} took {damage} damage!");
                }
            }
        }

        if (destroyAfterExplosion)
            Destroy(gameObject);
    }

    private void ApplyDamageToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerStats stats =
            player.GetComponent<PlayerStats>() ??
            player.GetComponentInChildren<PlayerStats>() ??
            player.GetComponentInParent<PlayerStats>();

        if (stats == null) return;

        stats.TakeDamage(damage);

        Debug.Log($"💣 Explosion: -{damage} HP");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
