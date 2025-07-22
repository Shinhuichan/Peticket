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
        // null 체크 추가
        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogWarning("⚠ buttons 배열이 비어 있습니다.");
            return;
        }

        defaultButtonColors = new Color[buttons.Length];

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
                slots[i].SetHighlight(i == currentIndex);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == null) continue;

            Image img = buttons[i].GetComponent<Image>();
            if (img == null) continue;

            bool isHighlighted = (slots.Length + i == currentIndex);
            if (defaultButtonColors != null && i < defaultButtonColors.Length)
                img.color = isHighlighted ? Color.white : defaultButtonColors[i];
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
