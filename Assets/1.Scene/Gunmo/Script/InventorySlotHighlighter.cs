using UnityEngine;
using UnityEngine.UI;

public class InventorySlotHighlighter : MonoBehaviour
{
    public InventorySlot[] slots;
    public Button[] buttons;

    private int currentIndex = 0;
    private Color[] defaultButtonColors;

    [Header("하이라이트 이동 딜레이")]
    public float moveCooldown = 0.3f; // 0.3초마다 1칸 이동 가능
    private float lastMoveTime = 0f;

    private void Start()
    {
        defaultButtonColors = new Color[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            defaultButtonColors[i] = buttons[i].GetComponent<Image>().color;
        }

        UpdateHighlight();
    }

    public void TryMoveHighlight(int direction)
    {
        // 쿨타임 확인
        if (Time.time - lastMoveTime >= moveCooldown)
        {
            MoveHighlight(direction);
            lastMoveTime = Time.time;
        }
    }

    public void MoveHighlight(int direction)
    {
        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, slots.Length + buttons.Length - 1);
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].SetHighlight(i == currentIndex);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            Image img = buttons[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = (slots.Length + i == currentIndex) ? Color.white : defaultButtonColors[i];
            }
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
            buttons[buttonIndex].onClick.Invoke();
        }
    }
}
