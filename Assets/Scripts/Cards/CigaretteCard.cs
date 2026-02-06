using System.Collections;
using UnityEngine;

public class CigaretteCard : ItemCard
{
    [Header("Damage")]
    [SerializeField] private int minDamage = 20;
    [SerializeField] private int maxDamage = 60;

    [Header("Monochrome Effect")]
    [SerializeField] private float filterDuration = 10f;
    [SerializeField] private string filterObjectName = "Vignette filter";

    public override void Use()
    {
        Debug.Log($"🚬 Cigarette called");
        ApplyDamageToPlayer();
        StartCoroutine(ApplyMonochromeEffect());
    }

    private void ApplyDamageToPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("CigaretteCard: Player tag not found!");
            return;
        }

        PlayerStats health =
            player.GetComponent<PlayerStats>() ??
            player.GetComponentInChildren<PlayerStats>() ??
            player.GetComponentInParent<PlayerStats>();

        if (health == null)
        {
            Debug.LogWarning("CigaretteCard: PlayerStats not found on Player (or children/parents)!");
            return;
        }

        int damage = Random.Range(minDamage, maxDamage + 1);
        health.TakeDamage(damage);

        Debug.Log($"🚬 Cigarette: Player lost {damage} HP. Now HP = {health.currentHealth}");
    }

    private IEnumerator ApplyMonochromeEffect()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (cam == null) yield break;

        Transform filter = FindDeepChild(cam.transform, filterObjectName);
        if (filter == null) yield break;

        filter.gameObject.SetActive(true);
        yield return new WaitForSeconds(filterDuration);
        if (filter != null)
            filter.gameObject.SetActive(false);
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform result = FindDeepChild(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
