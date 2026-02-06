using UnityEngine;

public class BombCard : ItemCard
{
    private GameObject bombPrefab;

    public void Init(GameObject prefab)
    {
        bombPrefab = prefab;
    }

    public override void Use()
    {
        if (bombPrefab == null)
        {
            Debug.LogError("Bomb prefab not set!");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Vector3 spawnPos = player.transform.position + player.transform.forward;
        Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        Debug.Log("Bomb spawned");
    }
}
