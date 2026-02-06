using System.Collections;
using UnityEngine;

public class AppleCard : ItemCard
{
    public override void Use()
    {
        ApplyDamageToPlayer();
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

        stats.Heal(30f);
    }
}
