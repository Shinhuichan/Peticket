// InventoryTrigger.cs
using UnityEngine;

public class InventoryTrigger : MonoBehaviour
{
    public InventorySlot slot;

    private void OnTriggerEnter(Collider other)
    {
        if (slot.IsEmpty && other.CompareTag("Item"))
        {
            slot.StoreItem(other.gameObject);
        }
    }
}
