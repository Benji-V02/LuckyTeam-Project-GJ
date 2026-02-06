using UnityEngine;

public class KonvickaCard : ItemCard
{
    public override void Use()
    {
        Debug.Log("🫗 WaterBoiling!");
        FindObjectOfType<FogObjectController>().FogForSeconds(10f);
    }
}
