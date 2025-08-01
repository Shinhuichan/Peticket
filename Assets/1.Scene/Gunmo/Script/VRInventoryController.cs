using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class VRInventoryController : MonoBehaviour
{
    public InventorySlotHighlighter highlighter;

    public InputActionReference moveAction;   // 휠이나 스틱 좌우
    public InputActionReference selectAction; // 버튼 (예: 트리거)

    private void Awake()
    {
        moveAction.action.performed += OnMove;
        selectAction.action.performed += OnSelect;
        moveAction.action.Enable();
        selectAction.action.Enable();
    }

    private void OnDestroy()
    {
        moveAction.action.performed -= OnMove;
        selectAction.action.performed -= OnSelect;
        moveAction.action.Disable();
        selectAction.action.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<Vector2>().x;

        if (value > 0.5f)
        {
            highlighter.TryMoveHighlight(1); // ✅ 딜레이 적용
        }
        else if (value < -0.5f)
        {
            highlighter.TryMoveHighlight(-1); // ✅ 딜레이 적용
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        highlighter.SelectCurrent();
    }
}
