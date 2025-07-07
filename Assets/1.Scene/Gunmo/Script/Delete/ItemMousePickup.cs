using UnityEngine;
using UnityEngine.EventSystems;

public class ItemMousePickup : MonoBehaviour, IPointerClickHandler
{
    public GameObject itemToPickup;

    public void OnPointerClick(PointerEventData eventData)
    {
        bool success = InventoryManager.Instance.AddItemToInventory(itemToPickup);
        if (success)
        {
            gameObject.SetActive(false); // 또는 Destroy(gameObject)
        }
    }
}
