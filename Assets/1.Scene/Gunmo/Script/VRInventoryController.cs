using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class VRInventoryController : MonoBehaviour
{
    public InventorySlotHighlighter highlighter;

    public InputActionReference moveAction;   // 휠이나 스틱 좌우
    public InputActionReference selectAction; // 버튼 (예: 트리거)

    [Header("Toggle Inventory Setting")]
    public InputActionAsset playerInputActions;
    private InputActionMap leftInteractionActionMap;
    private InputActionMap rightInteractionActionMap;
    private InputActionMap leftLocomotionActionMap;
    private InputActionMap rightLocomotionActionMap;
    [SerializeField] InputActionReference leftHandMoveAction;
    private InputActionMap inventoryActionMap;
    public GameObject inventoryPanel;
    public InputActionReference toggleInventoryAction;
    DynamicMoveProvider moveProvider;

    private void Awake()
    {
        moveAction.action.performed += OnMove;
        selectAction.action.performed += OnSelect;
        moveAction.action.Enable();
        selectAction.action.Enable();

        toggleInventoryAction.action.Enable();
        toggleInventoryAction.action.started += ToggleInventory;

        leftInteractionActionMap = playerInputActions.FindActionMap("XRI LeftHand Interaction");
        rightInteractionActionMap = playerInputActions.FindActionMap("XRI RightHand Interaction");
        leftLocomotionActionMap = playerInputActions.FindActionMap("XRI LeftHand Locomotion");
        rightLocomotionActionMap = playerInputActions.FindActionMap("XRI RightHand Locomotion");
        inventoryActionMap = playerInputActions.FindActionMap("XRI Inventory");
        moveProvider = FindFirstObjectByType<DynamicMoveProvider>();
    }

    private void OnDestroy()
    {
        moveAction.action.performed -= OnMove;
        selectAction.action.performed -= OnSelect;
        moveAction.action.Disable();
        selectAction.action.Disable();

        toggleInventoryAction.action.Disable();
        toggleInventoryAction.action.started -= ToggleInventory;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        float value = context.ReadValue<Vector2>().x;

        if (value > 0.5f)
        {
            highlighter.MoveHighlight(1);
        }
        else if (value < -0.5f)
        {
            highlighter.MoveHighlight(-1);
        }
    }

    private void OnSelect(InputAction.CallbackContext context)
    {
        highlighter.SelectCurrentSlot();
    }
    #region OpenInventory
    private void ToggleInventory(InputAction.CallbackContext context)
    {
        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);

        if (isActive)
        {
            // 인벤토리 열림: 이동과 상호작용 모두 비활성화, 인벤토리만 활성화
            leftInteractionActionMap?.Disable();
            rightInteractionActionMap?.Disable();
            leftLocomotionActionMap?.Disable();
            rightLocomotionActionMap?.Disable();
            inventoryActionMap?.Enable();

            // 이동 InputAction 자체를 Disable
            leftHandMoveAction?.action.Disable();
            if (moveProvider != null) moveProvider.enabled = false;

            Debug.Log("인벤토리 열림: 이동/상호작용 비활성화, 인벤토리 액션맵 활성화");
        }
        else
        {
            // 인벤토리 닫힘: 다시 이동/상호작용 활성화
            inventoryActionMap?.Disable();
            leftInteractionActionMap?.Enable();
            rightInteractionActionMap?.Enable();
            leftLocomotionActionMap?.Enable();
            rightLocomotionActionMap?.Enable();

            // 이동 InputAction 자체를 Enable
            leftHandMoveAction?.action.Enable();
            if (moveProvider != null) moveProvider.enabled = true;

            Debug.Log("인벤토리 닫힘: 인벤토리 액션맵 비활성화, 이동/상호작용 활성화");
        }
    }
    #endregion
}
