using UnityEngine;

public class WhiteFlag : ItemCard
{
    public override void Use()
    {
        Debug.Log("🏳️ You surrender!");
        //Game Won As Pacifist
    }
}
