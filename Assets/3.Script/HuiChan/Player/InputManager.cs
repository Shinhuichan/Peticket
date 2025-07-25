using CustomInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class InputManager : SingletonBehaviour<InputManager>
{
    protected override bool IsDontDestroy() => false;
    [Title("Input Setting", underlined: true, fontSize = 18, alignment = TextAlignment.Center)]

    public InputActionAsset playerInputActions;
    private InputActionMap leftInteractionActionMap;
    private InputActionMap rightInteractionActionMap;
    private InputActionMap leftLocomotionActionMap;
    private InputActionMap rightLocomotionActionMap;
    [SerializeField] InputActionReference leftHandMoveAction;
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

    [Header("Canvas Setting")]
    public Canvas canvas;
    void Start()
    {
        /////
        // Canvas의 EventCamera 초기 Setting
        canvas = GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
        /////
        leftInteractionActionMap = playerInputActions.FindActionMap("XRI LeftHand Interaction");
        rightInteractionActionMap = playerInputActions.FindActionMap("XRI RightHand Interaction");
        leftLocomotionActionMap = playerInputActions.FindActionMap("XRI LeftHand Locomotion");
        rightLocomotionActionMap = playerInputActions.FindActionMap("XRI RightHand Locomotion");
        inventoryActionMap = playerInputActions.FindActionMap("XRI Inventory");

        if (leftDirect == null || rightDirect == null)
        {
            Debug.LogWarning($"Input Setting | XRDirectInteractor가 Null입니다.");
            return;
        }

        InitializeMoveProvider();
        InitializeInputActions();
        InitialGrabEvents();

        getItemAction.action.Disable();
        inventoryActionMap.Disable();
    }

    void Update()
    {
        if (leftDirect == null || rightDirect == null)
        {
            FindDirectInteractorsByHierarchy();
        }
        if (moveProvider == null)
            moveProvider = FindFirstObjectByType<DynamicMoveProvider>();
    }
    protected override void OnDestroy()
    {
        // 모든 액션 비활성화 및 이벤트 구독 해제
        toggleInventoryAction.action.Disable();
        dashAction.action.Disable();
        getItemAction.action.Disable();

        toggleInventoryAction.action.performed -= ToggleInventory;
        dashAction.action.performed -= Dash;
        dashAction.action.canceled -= DashStop;

        leftDirect.selectEntered.RemoveListener(OnGrabStart);
        leftDirect.selectExited.RemoveListener(OnGrabEnd);
        rightDirect.selectEntered.RemoveListener(OnGrabStart);
        rightDirect.selectExited.RemoveListener(OnGrabEnd);

        getItemAction.action.performed -= GetItem;
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
        // 모든 Event 활성화
        toggleInventoryAction.action.Enable();
        dashAction.action.Enable();

        // Inventory Toggle Event 삽입
        toggleInventoryAction.action.performed += ToggleInventory;

        // Dash Event 삽입
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

            // 🔊 인벤토리 열기 사운드 재생
            AudioManager.Instance?.PlaySFXByKey("Inventory_open");

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

            // 🔊 인벤토리 닫기 사운드 재생 (같은 소리 재사용)
            AudioManager.Instance?.PlaySFXByKey("Inventory_open");

            Debug.Log("인벤토리 닫힘: 인벤토리 액션맵 비활성화, 이동/상호작용 활성화");
        }
    }

    #endregion
    #region GrabItem
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
    #endregion
    #region GetInventory
    private void GetItem(InputAction.CallbackContext context)
    {
        GameManager.Instance.currentHasItem.Add(selectedItem.GetComponent<ObjectInteraction>().objType);

        string combinedString = string.Join(", ", GameManager.Instance.currentHasItem);
        Debug.Log($"currentHasItem : [{combinedString}]");

        InventoryManager.Instance.AddItemToInventory(selectedItem);
        Debug.Log("Inventory로 진입 성공!");
    }
    #endregion
    #region FindDirectController
    public XRDirectInteractor[] allDirects;
    void FindDirectInteractorsByHierarchy()
    {
        allDirects = FindObjectsByType<XRDirectInteractor>(FindObjectsSortMode.None);

        foreach (var direct in allDirects)
        {
            if (direct.name == "Direct Interactor" && direct.transform.parent != null)
            {
                
                string parentName = direct.transform.parent.name;

                if (parentName == "Left Controller")
                {
                    direct.transform.parent.tag = "Hand_Left";
                    leftDirect = direct;
                }
                else if (parentName == "Right Controller")
                {
                    direct.transform.parent.tag = "Hand_Right";
                    rightDirect = direct;
                } 
            }
        }

        if (leftDirect == null) Debug.LogWarning("LeftDirect를 찾지 못했습니다!");
        if (rightDirect == null) Debug.LogWarning("RightDirect를 찾지 못했습니다!");
    }
    #endregion
}