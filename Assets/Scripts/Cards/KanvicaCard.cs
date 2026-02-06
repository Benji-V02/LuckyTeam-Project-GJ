using UnityEngine;

public class KanvicaCard : ItemCard
{
    public override void Use()
    {
        Debug.Log("🫗 Water Boiled!");
        FindObjectOfType<FogController>().EnableFogForSeconds(10f);
    }
}