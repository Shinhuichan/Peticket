using Unity.VisualScripting;
using UnityEngine;

public class InventoryManager : SingletonBehaviour<InventoryManager>
{
    protected override bool IsDontDestroy() => true;

    public InventorySlot[] slots;

    public bool AddItemToInventory(GameObject item)
    {
        Debug.Log($"🧪 AddItemToInventory: {item?.name}");

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
