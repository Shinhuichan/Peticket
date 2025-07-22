using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InventorySlot : MonoBehaviour
{
    [Header("아이템 요소")]
    public GameObject currentItem;
    public Transform previewRoot;
    public GameObject currentPreview;
    public Transform handTransform;
    public float checkRadius = 0.2f;

    [Header("피드백 요소")]
    public Image slotBackgroundImage;
    public AudioSource audioSource;
    public AudioClip errorSound;
    public TextMeshProUGUI warningText;

    [SerializeField] private Color originalColor = Color.white; // ✅ 수동 설정
    private bool isColorInitialized = false;

    public bool IsEmpty => currentItem == null;

    public void InitializeSlotColor()
    {
        if (!isColorInitialized && slotBackgroundImage != null)
        {
            originalColor = slotBackgroundImage.color;
            isColorInitialized = true;
        }
    }

    public void StoreItem(GameObject item)
    {
        Debug.Log($"[InventorySlot] StoreItem 호출됨 - 전달된 item: {(item != null ? item.name : "null")}");

        if (!IsEmpty)
        {
            Debug.LogWarning("[InventorySlot] 슬롯이 이미 차있습니다. 저장 실패");
            return;
        }

        if (item == null)
        {
            Debug.LogError("[InventorySlot] 전달된 item이 null입니다!");
            return;
        }

        currentItem = item;
        item.SetActive(false);

        var previewData = item.GetComponent<ItemPreviewProvider>();
        if (previewData != null && previewData.previewModelPrefab != null)
        {
            currentPreview = Instantiate(previewData.previewModelPrefab, previewRoot);
            currentPreview.transform.localPosition = previewData.previewOffset;
            currentPreview.transform.localRotation = Quaternion.Euler(previewData.previewRotationEuler);
            currentPreview.transform.localScale = Vector3.Scale(previewData.previewModelPrefab.transform.localScale, Vector3.one * previewData.previewScale);

            Debug.Log($"[InventorySlot] 미리보기 프리팹 생성 완료: {currentPreview.name}");

            var interactor = currentPreview.GetComponent<InventoryPreviewInteractor>();
            if (interactor != null)
            {
                interactor.Initialize(this);
                Debug.Log("[InventorySlot] InventoryPreviewInteractor 이벤트 연결 완료");
            }
            else
            {
                Debug.LogWarning("[InventorySlot] InventoryPreviewInteractor 컴포넌트 없음 (미리보기)");
            }
        }
        else
        {
            Debug.LogWarning("[InventorySlot] previewData 또는 previewModelPrefab이 없습니다.");
        }
    }

    public void ClearSlot()
    {
        Debug.Log("[InventorySlot] ClearSlot 호출됨");

        currentItem = null;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
            Debug.Log("[InventorySlot] 미리보기 삭제 완료");
        }
    }

    public void RemoveItemToHand()
    {
        Debug.Log("📤 RemoveItemToHand 호출됨");

        Collider[] colliders = Physics.OverlapSphere(handTransform.position, checkRadius);
        Debug.Log($"🔍 손 주변 감지된 오브젝트 수: {colliders.Length}");

        // 주석 처리된 손 검사 로직
        // if (colliders.Length > 0)
        // {
        //     Debug.LogWarning("손 위에 이미 아이템이 있습니다. 꺼낼 수 없습니다.");
        //     ShowSlotBlockedFeedback();
        //     return;
        // }

        if (currentItem != null)
        {
            // Inventory에서 Item 꺼낼 때, Data 최신화(추가된 부분)
            string objName = currentItem.name.Replace("(Preview)", "").Trim();
            InputManager.Instance.currentHasItem.Add(objName);
            string combinedString = string.Join(", ", InputManager.Instance.currentHasItem);
            Debug.Log($"currentHasItem : [{combinedString}]");
            //

            Debug.Log($"✅ 아이템 꺼내기 성공: {currentItem.name}");
            currentItem.SetActive(true);
            currentItem.transform.position = handTransform.position;
            currentItem.transform.rotation = handTransform.rotation;

            currentItem = null;

            if (currentPreview != null)
            {
                Destroy(currentPreview);
                Debug.Log("[InventorySlot] 프리뷰 제거됨 (꺼내기 후)");
            }
        }
        else
        {
            Debug.LogWarning("❌ currentItem이 null입니다. 슬롯이 비어 있음");
        }
    }

    private bool isBlinking = false;

    private void ShowSlotBlockedFeedback()
    {
        Debug.Log("[InventorySlot] 슬롯 차단 피드백 실행");

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
                Debug.Log("[InventorySlot] 프리뷰 흔들림 실행");
            }
        }
    }

    private IEnumerator BlinkSlot()
    {
        isBlinking = true;

        Color beforeBlinkColor = slotBackgroundImage.color;

        for (int i = 0; i < 2; i++)
        {
            slotBackgroundImage.color = Color.red;
            yield return new WaitForSeconds(0.15f);
            slotBackgroundImage.color = beforeBlinkColor;
            yield return new WaitForSeconds(0.15f);
        }

        slotBackgroundImage.color = beforeBlinkColor;
        isBlinking = false;
    }

    private void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
            Debug.Log("[InventorySlot] 에러 사운드 재생됨");
        }
    }

    private Coroutine warningCoroutine;

    private void ShowWarningMessage(string message)
    {
        if (warningCoroutine != null)
            StopCoroutine(warningCoroutine);

        warningText.text = message;
        warningText.gameObject.SetActive(true);
        warningCoroutine = StartCoroutine(HideWarningMessage());

        Debug.Log($"[InventorySlot] 경고 메시지 표시: {message}");
    }

    private IEnumerator HideWarningMessage()
    {
        yield return new WaitForSeconds(2f);
        warningText.gameObject.SetActive(false);
        warningCoroutine = null;
        Debug.Log("[InventorySlot] 경고 메시지 숨김");
    }

    public void SetHighlight(bool isOn)
    {
        if (slotBackgroundImage != null)
        {
            slotBackgroundImage.color = isOn ? Color.white : originalColor;
        }
    }
}
