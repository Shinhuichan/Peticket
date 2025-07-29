using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InventorySlotHighlighter : MonoBehaviour
{
    [Header("UI 연결")]
    public InventorySlot[] slots;
    public Button[] buttons;

    [Header("조이스틱 입력 액션")]
    public InputActionReference moveAction; // Vector2 타입 액션

    [Header("하이라이트 이동 설정")]
    public float moveCooldown = 0.3f;

    private int currentIndex = 0;
    private float lastMoveTime = 0f;
    private Color[] defaultButtonColors;

    private bool canMoveRight = true;
    private bool canMoveLeft = true;


    
    private void OnEnable()
    {
        if (moveAction != null) moveAction.action.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null) moveAction.action.Disable();
    }

    private void Start()
{
    defaultButtonColors = new Color[buttons.Length];

    // 👉 슬롯 색 먼저 초기화
    foreach (var slot in slots)
    {
        if (slot != null)
        {
            slot.InitializeSlotColor(); // originalColor 초기화
            slot.SetHighlight(false);   // 흰색으로 시작하지 않도록
        }
    }

    for (int i = 0; i < buttons.Length; i++)
    {
        if (buttons[i] == null)
        {
            Debug.LogError($"❌ 버튼 {i}이 null입니다.");
            continue;
        }

        Image img = buttons[i].GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError($"❌ 버튼 {i}에 Image 컴포넌트가 없습니다.");
            continue;
        }

        defaultButtonColors[i] = img.color;
    }

    // 👉 하이라이트는 마지막에
    UpdateHighlight();
}

    private void Update()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        if (input.x > 0.5f)
        {
            if (canMoveRight && Time.time - lastMoveTime >= moveCooldown)
            {
                MoveHighlight(1);
                lastMoveTime = Time.time;
                canMoveRight = false;
            }
        }
        else
        {
            canMoveRight = true;
        }

        if (input.x < -0.5f)
        {
            if (canMoveLeft && Time.time - lastMoveTime >= moveCooldown)
            {
                MoveHighlight(-1);
                lastMoveTime = Time.time;
                canMoveLeft = false;
            }
        }
        else
        {
            canMoveLeft = true;
        }
    }
    public void TryMoveHighlight(int direction)
    {
        if (Time.time - lastMoveTime >= moveCooldown)
        {
            MoveHighlight(direction);
            lastMoveTime = Time.time;
        }
    }

    private void MoveHighlight(int direction)
    {
        currentIndex += direction;
        currentIndex = Mathf.Clamp(currentIndex, 0, slots.Length + buttons.Length - 1);
        UpdateHighlight();
    }

    private void UpdateHighlight()
{
    for (int i = 0; i < slots.Length; i++)
    {
        if (slots[i] != null)
        {
            bool isHighlighted = (i == currentIndex);
            slots[i].SetHighlight(isHighlighted);
        }
    }

    for (int i = 0; i < buttons.Length; i++)
    {
        if (buttons[i] == null) continue;

        Image img = buttons[i].GetComponent<Image>();
        if (img == null) continue;

        bool isHighlighted = (slots.Length + i == currentIndex);
        if (defaultButtonColors != null && i < defaultButtonColors.Length)
        {
            img.color = isHighlighted ? Color.white : defaultButtonColors[i];
        }
    }
}

    public void SelectCurrent()
    {
        if (currentIndex < slots.Length)
        {
            if (slots[currentIndex] != null)
                slots[currentIndex].RemoveItemToHand();
        }
        else
        {
            int buttonIndex = currentIndex - slots.Length;
            if (buttonIndex >= 0 && buttonIndex < buttons.Length && buttons[buttonIndex] != null)
                buttons[buttonIndex].onClick.Invoke();
        }
    }
}
