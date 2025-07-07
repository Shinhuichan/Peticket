using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public GameObject currentItem;
    public Transform previewRoot; // 미리보기 모델 위치
    public GameObject currentPreview; // 슬롯에 보이는 미리보기
    public Transform handTransform; // 아이템을 꺼낼 위치 (예: 플레이어 손)
    public float checkRadius = 0.2f; // 손 주위 검사 범위

    public bool IsEmpty => currentItem == null;

    public void StoreItem(GameObject item)
    {
        if (!IsEmpty) return;

        currentItem = item;
        item.SetActive(false); // 실제 아이템 숨김

        // 3D 미리보기 생성
        var previewData = item.GetComponent<ItemPreviewProvider>();
        if (previewData != null && previewData.previewModelPrefab != null)
        {
            currentPreview = Instantiate(previewData.previewModelPrefab, previewRoot);
            currentPreview.transform.localPosition = previewData.previewOffset;
            currentPreview.transform.localRotation = Quaternion.Euler(previewData.previewRotationEuler);
            currentPreview.transform.localScale = Vector3.one * previewData.previewScale;
        }
    }

    public void ClearSlot()
    {
        currentItem = null;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }
    }
    public void RemoveItemToHand()
{
    // 🔍 1. 주변에 오브젝트가 있는지 검사
    Collider[] colliders = Physics.OverlapSphere(handTransform.position, checkRadius);
    if (colliders.Length > 0)
    {
        Debug.LogWarning("❗ 손 위에 이미 아이템이 있습니다. 꺼낼 수 없습니다.");
        return;
    }

    if (currentItem != null)
    {
        currentItem.SetActive(true);
        currentItem.transform.position = handTransform.position;
        currentItem.transform.rotation = handTransform.rotation;

        currentItem = null;

        if (currentPreview != null)
            Destroy(currentPreview);
    }
}
}
