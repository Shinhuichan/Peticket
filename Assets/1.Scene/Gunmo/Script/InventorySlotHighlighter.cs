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
        for (int i = 0; i < buttons.Length; i++)
        {
            defaultButtonColors[i] = buttons[i].GetComponent<Image>().color;
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
