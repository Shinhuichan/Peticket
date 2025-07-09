using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PreviewClickForwarder : MonoBehaviour
{
    public InventorySlot parentSlot;

    private void Awake()
    {
        if (parentSlot == null)
        {
            parentSlot = GetComponentInParent<InventorySlot>();
        }

        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.selectEntered.AddListener(OnSelected); // XR 클릭 대응
        }
    }

    private void OnSelected(SelectEnterEventArgs args)
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
