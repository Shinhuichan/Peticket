using UnityEngine;
using UnityEngine.UI;

public class InventorySlotHighlighter : MonoBehaviour
{
    public InventorySlot[] slots;             // 기존 슬롯
    public Button[] buttons;                  // 새로 추가할 버튼
    
    public GameObject highlightEffect;

    private int currentIndex = 0;
    private Color[] defaultButtonColors;
    private int totalSelectableCount => slots.Length + buttons.Length;

    private void Start()
    {
        // 각 버튼의 원래 색상을 저장
        defaultButtonColors = new Color[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            defaultButtonColors[i] = buttons[i].GetComponent<Image>().color;
        }
        UpdateHighlight();
    }

    public void MoveHighlight(int direction)
    {
        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, slots.Length + buttons.Length - 1); // 버튼까지 포함

        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        // 슬롯 하이라이트
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlight(i == currentIndex);
        }

        // 버튼 하이라이트 색상
        for (int i = 0; i < buttons.Length; i++)
        {
            Image img = buttons[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = (slots.Length + i == currentIndex) ? Color.white : defaultButtonColors[i];
            }
        }

        // 슬롯용 이펙트 위치 갱신
        if (highlightEffect != null && currentIndex < slots.Length)
        {
            highlightEffect.transform.SetParent(slots[currentIndex].transform, false);
            highlightEffect.transform.localPosition = Vector3.zero;
        }
        else if (highlightEffect != null)
        {
            highlightEffect.transform.SetParent(null); // 버튼 쪽일 때는 이펙트 숨기기
        }
    }

    public void SelectCurrent()
    {
        if (currentIndex < slots.Length)
        {
            slots[currentIndex].RemoveItemToHand();
        }
        else
        {
            int buttonIndex = currentIndex - slots.Length;
            buttons[buttonIndex].onClick.Invoke(); // 버튼 클릭 처리
        }
    }
}
