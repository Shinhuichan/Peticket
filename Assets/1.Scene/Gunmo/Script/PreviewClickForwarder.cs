using UnityEngine;
using UnityEngine.EventSystems;

public class PreviewClickForwarder : MonoBehaviour, IPointerClickHandler
{
    public InventorySlot parentSlot;

    private void Awake()
    {
        // 자동으로 상위에서 InventorySlot 찾기
        if (parentSlot == null)
        {
            parentSlot = GetComponentInParent<InventorySlot>();
        }

        if (parentSlot == null)
        {
            Debug.LogWarning($"[PreviewClickForwarder] 부모 슬롯을 찾지 못했습니다: {gameObject.name}");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (parentSlot != null)
        {
            parentSlot.RemoveItemToHand();
        }
        else
        {
            Debug.LogWarning($"[PreviewClickForwarder] parentSlot이 설정되지 않았습니다: {gameObject.name}");
        }
    }
}
