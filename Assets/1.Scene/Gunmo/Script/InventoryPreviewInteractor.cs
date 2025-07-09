using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class InventoryPreviewInteractor : MonoBehaviour
{
    private InventorySlot parentSlot;

    public void Initialize(InventorySlot slot)
    {
        parentSlot = slot;

        var interactable = GetComponent<XRSimpleInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelected);
            interactable.selectEntered.AddListener(OnSelected);
        }
        else
        {
            Debug.LogWarning("[InventoryPreviewInteractor] XRSimpleInteractable이 없습니다.");
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Debug.Log("✅ 프리뷰 선택됨");

        if (parentSlot != null)
        {
            parentSlot.RemoveItemToHand();
        }
        else
        {
            Debug.LogWarning("[InventoryPreviewInteractor] InventorySlot 연결 안됨");
        }
    }
}
