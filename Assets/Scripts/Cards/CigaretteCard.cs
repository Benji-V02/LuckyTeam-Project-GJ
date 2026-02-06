using System.Collections;
using UnityEngine;

public class CigaretteCard : ItemCard
{
    [Header("Damage")]
    [SerializeField] private int minDamage = 20;
    [SerializeField] private int maxDamage = 60;

    [Header("Monochrome Filter")]
    [SerializeField] private float filterDuration = 3f;
    [SerializeField] private string filterObjectName = "Vignette Filter";

    private Coroutine filterCoroutine;

    public override void Use()
    {
        ApplyDamageToPlayer();
        StartMonochromeEffect();
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

        int damage = Random.Range(minDamage, maxDamage + 1);
        stats.TakeDamage(damage);

        Debug.Log($"🚬 Cigarette: -{damage} HP");
    }

    private void StartMonochromeEffect()
    {
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (cam == null)
        {
            Debug.LogWarning("CigaretteCard: Camera not found (tag: Camera)");
            return;
        }

        Transform filter = FindDeepChild(cam.transform, filterObjectName);
        if (filter == null)
        {
            Debug.LogWarning($"CigaretteCard: '{filterObjectName}' not found under Camera");
            return;
        }

        filterObj.SetActive(true);
    }

    private Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
                return child;

            Transform found = FindDeepChild(child, name);
            if (found != null)
                return found;
        }
        return null;
    }
}
