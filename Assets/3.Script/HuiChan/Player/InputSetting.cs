using CustomInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class InputSetting : MonoBehaviour
{
    [Title("Input Setting", underlined: true, fontSize = 18, alignment = TextAlignment.Center)]
    public InputActionAsset playerInputActions;
    private InputActionMap interactionActionMap;
    private InputActionMap inventoryActionMap;


    [Header("OpenInventory Setting")]
    public GameObject inventoryPanel;
    public InputActionReference toggleInventoryAction;

    [Header("GetItem Setting")]
    public InputActionReference getItemAction;
    public GameObject selectedItem;

    [Header("Dash Setting")]
    [SerializeField, ReadOnly] float baseMoveSpeed;
    [SerializeField] float dashIncrease = 1.5f;
    
    public InputActionReference dashAction;
    [SerializeField] XRDirectInteractor leftDirect;
    [SerializeField] XRDirectInteractor rightDirect;
    [SerializeField, ReadOnly] DynamicMoveProvider moveProvider;

    void Start()
    {
        // leftInteractionActionMap = playerInputActions.FindActionMap("XRI LeftHand Interaction");
        // rightInteractionActionMap = playerInputActions.FindActionMap("XRI RightHand Interaction");
        // inventoryActionMap = playerInputActions.FindActionMap("XRI Inventory");
        if (leftDirect == null || rightDirect == null)
        {
            Debug.LogWarning($"Input Setting | XRDirectInteractor가 Null입니다.");
            return;
        }

        InitializeMoveProvider();
        InitializeInputActions();
        InitialGrabEvents();
        
        InputSystem.onDeviceChange += OnDeviceChange;

        getItemAction.action.Disable();
    }

    void OnDestroy()
    {
        // 모든 액션 비활성화 및 이벤트 구독 해제
        toggleInventoryAction.action.Disable();
        dashAction.action.Disable();
        getItemAction.action.Disable();

        toggleInventoryAction.action.performed -= ToggleInventory;
        dashAction.action.performed -= Dash;
        dashAction.action.canceled -= DashStop;

        // 굳건희! Grab/Ungrab 이벤트도 해제해줘.
        leftDirect.selectEntered.RemoveListener(OnGrabStart);
        leftDirect.selectExited.RemoveListener(OnGrabEnd);
        rightDirect.selectEntered.RemoveListener(OnGrabStart);
        rightDirect.selectExited.RemoveListener(OnGrabEnd);

        // GetItem 액션 이벤트도 해제
        getItemAction.action.performed -= GetItem;
        
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    #region Initial Setting
    void InitializeMoveProvider()
    {
        moveProvider = FindFirstObjectByType<DynamicMoveProvider>();
        if (moveProvider == null)
        {
            Debug.LogWarning($"Input Setting | moveProvider가 Null입니다.");
            return;
        }
        baseMoveSpeed = moveProvider.moveSpeed;
    }
    void InitializeInputActions()
    {
        // Inventory Toggle Event 및 Dash Event 활성화
        toggleInventoryAction.action.Enable();
        dashAction.action.Enable();

        // Inventory Toggle Event 및 Dash Event 삽입
        toggleInventoryAction.action.performed += ToggleInventory;

        dashAction.action.performed += Dash;
        dashAction.action.canceled += DashStop;
    }
    void InitialGrabEvents()
    {
        leftDirect.selectEntered.AddListener(OnGrabStart); // 잡기 시작했을 때
        leftDirect.selectExited.AddListener(OnGrabEnd);   // 놓았을 때

        rightDirect.selectEntered.AddListener(OnGrabStart);
        rightDirect.selectExited.AddListener(OnGrabEnd);
    }
    #endregion

    #region Dash
    private void Dash(InputAction.CallbackContext context)
    {
        // Dash Start 기능
        moveProvider.moveSpeed = baseMoveSpeed * dashIncrease;
    }
    private void DashStop(InputAction.CallbackContext context)
    {
        // Dash Stop 기능
        moveProvider.moveSpeed = baseMoveSpeed;
    }
    #endregion

    #region OpenInventory
    private void ToggleInventory(InputAction.CallbackContext context)
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
    #endregion

    #region GetInventory
    private void OnGrabStart(SelectEnterEventArgs args)
    {
        Debug.Log($"'{args.interactableObject}'을(를) 잡기 시작함. GetItem 액션 활성화!");
        selectedItem = args.interactableObject.transform.gameObject;
        getItemAction.action.Enable();
        getItemAction.action.performed += GetItem;
    }

    private void OnGrabEnd(SelectExitEventArgs args)
    {
        Debug.Log($"'{args.interactableObject}'을(를) 잡기 해제함. GetItem 액션 비활성화...");
        selectedItem = null;
        getItemAction.action.Disable();
        getItemAction.action.performed -= GetItem;
    }

    private void GetItem(InputAction.CallbackContext context)
    {
        InventoryManager.Instance.AddItemToInventory(selectedItem);
        Debug.Log("Inventory로 진입 성공!");
    }
    #endregion

    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
                toggleInventoryAction.action.Disable();

                // toggleInventoryAction에 ToggleInventory Event가 이미 제거되어있는 지 방어 코드가 필요함.
                toggleInventoryAction.action.performed -= ToggleInventory;
                break;
            case InputDeviceChange.Reconnected:
                toggleInventoryAction.action.Enable();

                // toggleInventoryAction에 ToggleInventory Event가 이미 추가되어있는 지 방어 코드가 필요함.
                toggleInventoryAction.action.performed += ToggleInventory;
                break;
        }
    }
}