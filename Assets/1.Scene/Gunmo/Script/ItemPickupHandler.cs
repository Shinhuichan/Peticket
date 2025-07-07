// ItemPickupHandler.cs (아이템에 부착)
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ItemPickupHandler : MonoBehaviour
{
    private void OnEnable()
    {
        var interactable = GetComponent<XRGrabInteractable>();
        interactable.selectExited.AddListener(OnGrab);
    }

    private void OnGrab(SelectExitEventArgs args)
    {
        transform.SetParent(null); // 부모에서 분리
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;
    }
}
