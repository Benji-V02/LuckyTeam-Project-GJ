using UnityEngine;

public class FurikCard : ItemCard
{
    public override void Use()
    {
        Debug.Log("⏱️ Weeeeeeeeeee!");
        SimplePlayerMovement player = FindObjectOfType<SimplePlayerMovement>();

        if (player == null)
        {
            Debug.LogError("PlayerMovement not found!");
            return;
        }

        player.BoostSpeed(5.8f, 10f);
    }
}
