using UnityEngine;

public class WhiteFlag : ItemCard
{
    public override void Use()
    {

        // Nájdi EnemySpawner v scéne
        EnemySpawner spawner = FindObjectOfType<EnemySpawner>();

        if (spawner != null)
        {
            spawner.TriggerVictory();
        }
        else
        {
            Debug.LogError("EnemySpawner not found in scene!");
        }
    }
}