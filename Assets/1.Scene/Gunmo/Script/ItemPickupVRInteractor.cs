using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ItemPickupVRInteractor : MonoBehaviour
{
    // XR Grab Interactable의 SelectEntered 이벤트에 연결되어야 함
    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        Debug.Log($"📦 VR 아이템 선택됨: {gameObject.name}");

        bool success = InventoryManager.Instance.AddItemToInventory(gameObject);

        if (success)
        {
            Destroy(gameObject); // ✅ 아이템을 완전히 삭제
        }
        else
        {
            Debug.LogWarning("❌ 인벤토리에 넣을 수 없습니다.");
        }
    }
}
