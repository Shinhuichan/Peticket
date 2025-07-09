using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPreviewClickHandler : MonoBehaviour
{
    public InventorySlot linkedSlot;

    public void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (linkedSlot != null)
            linkedSlot.RemoveItemToHand();
    }
}
