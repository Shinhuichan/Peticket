using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    public GameObject currentItem;
    public Transform previewRoot; // 미리보기 모델 위치
    public GameObject currentPreview; // 슬롯에 보이는 미리보기

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
}
