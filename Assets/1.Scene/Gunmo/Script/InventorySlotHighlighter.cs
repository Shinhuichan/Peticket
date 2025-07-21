using UnityEngine;
using UnityEngine.UI;

public class InventorySlotHighlighter : MonoBehaviour
{
    [Header("하이라이트 대상")]
    public InventorySlot[] slots;       // 인벤토리 슬롯들
    public Button[] buttons;            // 버튼들 (설정, 홈, 종료 등)
    public GameObject highlightEffect;  // 하이라이트 이펙트

    [Header("입력 쿨타임 설정")]
    public float inputCooldown = 0.3f;  // VR 입력 간격 제한

    private int currentIndex = 0;
    private float lastInputTime = 0f;
    private Color[] defaultButtonColors;

    // 총 선택 가능 수: 슬롯 + 버튼
    private int totalSelectableCount => slots.Length + buttons.Length;

    private void Start()
    {
        // 버튼 원래 색상 저장
        defaultButtonColors = new Color[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].TryGetComponent(out Image img))
                defaultButtonColors[i] = img.color;
        }

        UpdateHighlight();
    }

    /// <summary>
    /// VR 입력 시 호출: 방향 입력 (예: +1, -1)
    /// </summary>
    public void TryMoveHighlight(int direction)
    {
        if (Time.time - lastInputTime < inputCooldown)
            return;

        MoveHighlight(direction);
        lastInputTime = Time.time;
    }

    /// <summary>
    /// 하이라이트 커서 이동
    /// </summary>
    public void MoveHighlight(int direction)
    {
        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, totalSelectableCount - 1);
        UpdateHighlight();
    }

    /// <summary>
    /// 하이라이트 UI 상태 갱신
    /// </summary>
    private void UpdateHighlight()
    {
        // 슬롯 하이라이트
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlight(i == currentIndex);
        }

        // 버튼 하이라이트 (색상 변경)
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].TryGetComponent(out Image img))
            {
                img.color = (slots.Length + i == currentIndex) ? Color.white : defaultButtonColors[i];
            }
        }

        // 하이라이트 이펙트 위치 설정
        if (highlightEffect != null && currentIndex < slots.Length)
        {
            highlightEffect.transform.SetParent(slots[currentIndex].transform, false);
            highlightEffect.transform.localPosition = Vector3.zero;
        }
        else if (highlightEffect != null)
        {
            highlightEffect.transform.SetParent(null); // 버튼이면 숨기기
        }
    }

    /// <summary>
    /// 현재 선택된 슬롯 or 버튼 실행
    /// </summary>
    public void SelectCurrent()
    {
        if (currentIndex < slots.Length)
        {
            slots[currentIndex].RemoveItemToHand();
        }
        else
        {
            int buttonIndex = currentIndex - slots.Length;
            buttons[buttonIndex].onClick.Invoke(); // 버튼 작동
        }
    }
}
