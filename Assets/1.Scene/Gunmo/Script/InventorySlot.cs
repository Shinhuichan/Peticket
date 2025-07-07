using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public GameObject currentItem;
    
    public bool IsEmpty => currentItem == null;

    public void StoreItem(GameObject item)
    {
        if (!IsEmpty) return;

        currentItem = item;
        item.transform.position = transform.position;
        item.transform.rotation = transform.rotation;
        item.transform.SetParent(transform);
        var rb = item.GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }
}
