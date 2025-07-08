using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InventorySlot : MonoBehaviour
{
    [Header("아이템 요소")]
    public GameObject currentItem;
    public Transform previewRoot; // 미리보기 모델 위치
    public GameObject currentPreview; // 슬롯에 보이는 미리보기
    public Transform handTransform; // 아이템을 꺼낼 위치 (예: 플레이어 손)
    public float checkRadius = 0.2f; // 손 주위 검사 범위
    [Header("피드백 요소")]
public Image slotBackgroundImage;           // 점멸용 이미지 (슬롯 UI의 배경)
public AudioSource audioSource;             // 에러 사운드 재생기
public AudioClip errorSound;                // 에러 사운드 클립
public TextMeshProUGUI warningText;         // 경고 메시지 표시 UI

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
        // 주변에 오브젝트가 있는지 검사
        Collider[] colliders = Physics.OverlapSphere(handTransform.position, checkRadius);
        if (colliders.Length > 0)
        {
            Debug.LogWarning("손 위에 이미 아이템이 있습니다. 꺼낼 수 없습니다.");
            ShowSlotBlockedFeedback(); // 피드백 실행
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
    private bool isBlinking = false;
    private void ShowSlotBlockedFeedback()
    {
        if (!isBlinking)
            StartCoroutine(BlinkSlot());
        PlayErrorSound();
        ShowWarningMessage("손이 비어 있어야 아이템을 꺼낼 수 있습니다.");
        if (currentPreview != null)
{
    var shaker = currentPreview.GetComponent<ItemPreviewRotator>();
    if (shaker != null)
    {
        StartCoroutine(shaker.Shake());
    }
}
    }
    private IEnumerator BlinkSlot()
    {
        isBlinking = true;

    Color originalColor = slotBackgroundImage.color;
    for (int i = 0; i < 2; i++)
    {
        slotBackgroundImage.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        slotBackgroundImage.color = originalColor;
        yield return new WaitForSeconds(0.15f);
    }

    slotBackgroundImage.color = originalColor;
    isBlinking = false;
    }
private void PlayErrorSound()
{
    if (audioSource != null && errorSound != null)
        audioSource.PlayOneShot(errorSound);
}

private Coroutine warningCoroutine;

private void ShowWarningMessage(string message)
{
    if (warningCoroutine != null)
        StopCoroutine(warningCoroutine);

    warningText.text = message;
    warningText.gameObject.SetActive(true);
    warningCoroutine = StartCoroutine(HideWarningMessage());
}

private IEnumerator HideWarningMessage()
{
    yield return new WaitForSeconds(2f);
    warningText.gameObject.SetActive(false);
    warningCoroutine = null;
}
}