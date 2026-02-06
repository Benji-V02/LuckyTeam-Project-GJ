using UnityEngine;

public class TimeCard : ItemCard
{
    public override void Use()
    {
        Debug.Log("⏱️ Enemies freezed!");
        EnemyFreezeManager.Instance.FreezeEnemies(10f);
    }
}
