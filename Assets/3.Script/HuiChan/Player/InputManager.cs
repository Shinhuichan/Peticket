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
    private InputActionMap inventoryActionMap;

    [Header("OpenInventory Setting")]
    public GameObject inventoryPanel;
    public InputActionReference toggleInventoryAction;

    [Header("GetItem Setting")]
    public InputActionReference getItemAction;
    [SerializeField] public GameObject selectedItem;

    [Header("Dash Setting")]
    [SerializeField] float dashIncrease = 1.5f;
    [SerializeField, ReadOnly] float baseMoveSpeed; // 원래 이동 속도를 여기에 저장할 거야.
    public InputActionReference dashAction;

    // 플레이어 컨트롤러에 있는 XRDirectInteractor를 연결해줘.
    [SerializeField] XRDirectInteractor leftDirect;
    [SerializeField] XRDirectInteractor rightDirect;
    [SerializeField, ReadOnly] DynamicMoveProvider moveProvider;

    protected override void Awake()
    {
        base.Awake();
        if (playerInputActions == null)
        {
            Debug.LogError("InputManager: 'Player Input Actions' 에셋이 할당되지 않았습니다! Input 기능을 사용할 수 없습니다.");
            return;
        }

        leftInteractionActionMap = playerInputActions.FindActionMap("XRI LeftHand Interaction");
        rightInteractionActionMap = playerInputActions.FindActionMap("XRI RightHand Interaction");
        leftLocomotionActionMap = playerInputActions.FindActionMap("XRI LeftHand Locomotion");
        rightLocomotionActionMap = playerInputActions.FindActionMap("XRI RightHand Locomotion");
        inventoryActionMap = playerInputActions.FindActionMap("XRI Inventory");

        moveProvider = FindFirstObjectByType<DynamicMoveProvider>();
        if (moveProvider == null) Debug.LogWarning($"InputManager: 'DynamicMoveProvider'를 찾을 수 없습니다. 대쉬 기능이 제한될 수 있습니다.");
        else baseMoveSpeed = moveProvider.moveSpeed; // 초기 이동 속도 저장

        // XRDirectInteractor 참조 확인
        if (leftDirect == null) Debug.LogWarning($"InputManager: 'Left Direct Interactor'가 할당되지 않았습니다.");
        if (rightDirect == null) Debug.LogWarning($"InputManager: 'Right Direct Interactor'가 할당되지 않았습니다.");

        getItemAction.action.Disable();
        inventoryActionMap.Disable();
    }

    void OnEnable()
    {

        playerInputActions?.Enable(); // playerInputActions가 null이 아닐 때만 Enable()

        // Inventory Toggle
        if (toggleInventoryAction != null && toggleInventoryAction.action != null)
        {
            toggleInventoryAction.action.Enable();
            toggleInventoryAction.action.performed += ToggleInventory;
        }

        // Dash Actions
        if (dashAction != null && dashAction.action != null)
        {
            dashAction.action.Enable();
            dashAction.action.performed += Dash;
            dashAction.action.canceled += DashStop;
        }

        // Grab Events (XRDirectInteractor)
        if (leftDirect != null)
        {
            leftDirect.selectEntered.AddListener(OnGrabStart);
            leftDirect.selectExited.AddListener(OnGrabEnd);
        }
        if (rightDirect != null)
        {
            rightDirect.selectEntered.AddListener(OnGrabStart);
            rightDirect.selectExited.AddListener(OnGrabEnd);
        }

    }

    void OnDisable()
    {
        // 굳건희! 플레이어 Input Actions 에셋 전체를 비활성화해줘.
        playerInputActions?.Disable();

        // Inventory Toggle
        if (toggleInventoryAction != null && toggleInventoryAction.action != null)
        {
            toggleInventoryAction.action.performed -= ToggleInventory;
            toggleInventoryAction.action.Disable(); // 액션 비활성화
        }

        // Dash Actions
        if (dashAction != null && dashAction.action != null)
            {
            dashAction.action.performed -= Dash;
            dashAction.action.canceled -= DashStop;
            dashAction.action.Disable(); // 액션 비활성화
        }

        // Grab Events (XRDirectInteractor)
        if (leftDirect != null)
        {
            leftDirect.selectEntered.RemoveListener(OnGrabStart);
            leftDirect.selectExited.RemoveListener(OnGrabEnd);
        }
        if (rightDirect != null)
        {
            rightDirect.selectEntered.RemoveListener(OnGrabStart);
            rightDirect.selectExited.RemoveListener(OnGrabEnd);
        }

        // getItemAction은 OnGrabEnd에서 비활성화됨.
        // 혹시 여기에 연결된 이벤트가 있다면 해제 (중복 구독 방지)
        if (getItemAction != null && getItemAction.action != null)
        {
            getItemAction.action.performed -= GetItem;
            getItemAction.action.Disable(); // 액션 비활성화
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy(); // 부모 OnDestroy 호출
        Debug.Log("InputManager: 애플리케이션 종료로 인한 OnDestroy 호출.");
    }

    #region Dash
    private void Dash(InputAction.CallbackContext context)
    {
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = baseMoveSpeed * dashIncrease;
            Debug.Log($"InputManager: 대쉬 시작! 현재 속도: {moveProvider.moveSpeed}");
        }
    }

    private void DashStop(InputAction.CallbackContext context)
    {
        if (moveProvider != null)
        {
            moveProvider.moveSpeed = baseMoveSpeed;
            Debug.Log($"InputManager: 대쉬 종료! 원래 속도: {moveProvider.moveSpeed}");
        }
    }
    #endregion

    #region Inventory
    private void ToggleInventory(InputAction.CallbackContext context)
    {
        if (inventoryPanel == null)
        {
            Debug.LogWarning("InputManager: 'Inventory Panel' GameObject가 할당되지 않았습니다.");
            return;
        }

        bool isActive = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isActive);
        Debug.Log($"InputManager: 인벤토리 토글! 현재 상태: {isActive}");

        if (isActive)
        {
            // 인벤토리가 열렸을 때: 플레이어 이동/상호작용 비활성화, 인벤토리 액션 맵만 활성화
            leftInteractionActionMap?.Disable();
            rightInteractionActionMap?.Disable();
            leftLocomotionActionMap?.Disable();
            rightLocomotionActionMap?.Disable();
            inventoryActionMap?.Enable(); // 인벤토리 액션 맵 활성화

            if (moveProvider != null) moveProvider.enabled = false; // 이동 스크립트 비활성화
        }
        else
        {
            // 인벤토리가 닫혔을 때: 인벤토리 액션 맵 비활성화, 플레이어 이동/상호작용 활성화
            inventoryActionMap?.Disable();
            leftInteractionActionMap?.Enable();
            rightInteractionActionMap?.Enable();
            leftLocomotionActionMap?.Enable();
            rightLocomotionActionMap?.Enable();

            if (moveProvider != null) moveProvider.enabled = true; // 이동 스크립트 활성화
        }
    }
    #endregion

    #region GrabItem (Player Hand Grab Events)
    private void OnGrabStart(SelectEnterEventArgs args)
    {
        if (args.interactableObject != null)
        {
            XRBaseInteractable baseInteractable = args.interactableObject as XRBaseInteractable;

            if (baseInteractable != null)
            {
                selectedItem = baseInteractable.gameObject;
                Debug.Log($"InputManager: '{selectedItem.name}'을(를) 잡기 시작함. GetItem 액션 준비!");

                if (getItemAction != null && getItemAction.action != null)
                {
                    getItemAction.action.performed -= GetItem;
                    getItemAction.action.performed += GetItem;
                    getItemAction.action.Enable();
                }
            }
            else
            {
                Debug.LogWarning($"InputManager: 잡은 오브젝트({args.interactableObject.GetType().Name})가 XRBaseInteractable 타입이 아닙니다. GameObject를 가져올 수 없습니다. 😭");
                selectedItem = null; // 초기화
            }
        }
        else Debug.LogWarning("InputManager: 잡은 오브젝트(interactableObject)가 null입니다.");
    }

    private void OnGrabEnd(SelectExitEventArgs args)
    {
        Debug.Log($"InputManager: '{selectedItem?.name}'을(를) 잡기 해제함. GetItem 액션 비활성화...");

        if (getItemAction != null && getItemAction.action != null)
        {
            getItemAction.action.performed -= GetItem;
            getItemAction.action.Disable();
        }
        selectedItem = null;
    }
    #endregion

    #region GetItem (Player specific item action)
    private void GetItem(InputAction.CallbackContext context)
    {
        if (selectedItem == null)
        {
            Debug.LogWarning("InputManager: GetItem 액션이 발동했지만 선택된 아이템이 없습니다.");
            return;
        }

        ObjectInteraction objInteraction = selectedItem.GetComponent<ObjectInteraction>();
        if (objInteraction != null)
        {
            GameManager.Instance.currentHasItem.Add(objInteraction.objType);

            string itemName = selectedItem.name; // 예를 들어 GameObject의 이름을 아이템 이름으로 사용.

            if (InventoryManager.Instance != null)
            {
                Debug.Log($"InputManager: InventoryManager를 통해 '{itemName}'을(를) Inventory에 추가 성공!");
                InventoryManager.Instance.AddItemToInventory(selectedItem);
            }
            else Debug.LogWarning("InputManager: InventoryManager 인스턴스를 찾을 수 없습니다. 아이템 추가 불가.");


            Debug.Log("InputManager: Inventory로 진입 성공!");
        }
        else
        {
            Debug.LogWarning($"InputManager: '{selectedItem.name}'에는 ObjectInteraction 스크립트가 붙어있지 않습니다.");
        }
    }
    #endregion
}