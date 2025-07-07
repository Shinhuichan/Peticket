using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public InventorySlot[] slots;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public bool AddItemToInventory(GameObject item)
    {
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                slot.StoreItem(item);
                return true;
            }
        }

        Debug.LogWarning("인벤토리에 빈 슬롯이 없습니다!");
        return false;
    }
}
