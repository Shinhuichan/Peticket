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

    [SerializeField] private Color originalColor = Color.white;
    private bool isColorInitialized = false;

    public bool IsEmpty => currentItem == null;

    private void Start()
    {
        InitializeHandTransform();
        InitializeSlotColor();
    }

    private void InitializeHandTransform()
    {
        if (handTransform != null) return;

        if (Player.Instance != null && Player.Instance.itemPosition != null)
        {
            handTransform = Player.Instance.itemPosition;
            Debug.Log($"handTransform을 Player.Instance.itemPosition으로 강제 연결: {handTransform.name}");
        }
        else
        {
            Debug.LogError("❌ Player.Instance 또는 itemPosition이 존재하지 않습니다. handTransform 연결 실패.");
        }
    }

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

            var interactor = currentPreview.GetComponent<InventoryPreviewInteractor>();
            if (interactor != null)
            {
                interactor.Initialize(this);
            }
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
        Debug.Log("📤 RemoveItemToHand 호출됨");

        if (currentItem == null)
        {
            Debug.LogWarning("❌ currentItem이 null입니다. 슬롯이 비어 있음");
            return;
        }

        if (handTransform == null)
        {
            InitializeHandTransform();
            if (handTransform == null)
            {
                Debug.LogError("❌ handTransform이 여전히 null입니다. 아이템을 손으로 꺼낼 수 없습니다!");
                ShowSlotBlockedFeedback("아이템을 꺼낼 손 위치를 찾을 수 없습니다.");
                return;
            }
        }

        if (!ItemUseZoneManager.Instance.IsPrefabAllowedInZone(handTransform.position, currentItem))
        {
            ShowSlotBlockedFeedback("이 위치에서는 이 아이템을 꺼낼 수 없습니다.");
            return;
        }

        // ✅ 안전하게 ObjectInteraction 접근
        var interaction = currentItem.GetComponent<ObjectInteraction>();
        if (interaction != null && GameManager.Instance != null && GameManager.Instance.currentHasItem != null)
        {
            GameManager.Instance.currentHasItem.Remove(interaction.objType);
            Debug.Log($"currentHasItem : [{string.Join(", ", GameManager.Instance.currentHasItem)}]");
        }
        else
        {
            Debug.LogWarning("❗ ObjectInteraction 또는 GameManager가 null입니다. currentHasItem에서 제거 실패");
        }

        currentItem.SetActive(true);
        currentItem.transform.position = handTransform.position;
        currentItem.transform.rotation = handTransform.rotation;

        currentItem = null;

        if (currentPreview != null)
        {
            Destroy(currentPreview);
        }

        AudioManager.Instance?.PlaySFXByKey("Drop_item");
    }

    private bool isBlinking = false;

    private void ShowSlotBlockedFeedback(string message)
    {
        if (!isBlinking) StartCoroutine(BlinkSlot());
        PlayErrorSound();
        ShowWarningMessage(message);

        if (currentPreview != null)
        {
            var shaker = currentPreview.GetComponent<ItemPreviewRotator>();
            if (shaker != null) StartCoroutine(shaker.Shake());
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
    }

    private IEnumerator HideWarningMessage()
    {
        yield return new WaitForSeconds(2f);
        warningText.gameObject.SetActive(false);
        warningCoroutine = null;
    }

    public void SetHighlight(bool isOn)
    {
        if (slotBackgroundImage != null)
        {
            slotBackgroundImage.color = isOn ? Color.white : originalColor;
        }
    }
}
