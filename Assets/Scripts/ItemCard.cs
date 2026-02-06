using UnityEngine;

public class ItemCard : MonoBehaviour
{
    public virtual void Use()
    {
        Debug.Log($"{name} -> ItemCard.Use()");
    }
}
