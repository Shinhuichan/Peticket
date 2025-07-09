using UnityEngine;

public class ItemPickupButton : MonoBehaviour
{
    public GameObject itemToPickup;  // ✅ 반드시 public 이어야 합니다

    public void OnPickupButtonClicked()
    {
        bool success = InventoryManager.Instance.AddItemToInventory(itemToPickup);
        if (success)
        {
           itemToPickup.SetActive(false); //Destroy(itemToPickup)  itemToPickup.SetActive(false)
        }
    }
}
