using UnityEngine;

public class ItemPickupButton : MonoBehaviour
{
    public GameObject itemToPickup;

    public void OnPickupButtonClicked()
    {
        bool success = InventoryManager.Instance.AddItemToInventory(itemToPickup);
        if (success)
        {
            gameObject.SetActive(false); // 아이템 오브젝트 숨기기 or 파괴
        }
    }
}
