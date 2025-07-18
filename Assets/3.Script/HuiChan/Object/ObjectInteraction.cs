using System.Linq;
using CustomInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class ObjectInteraction : MonoBehaviour
{
    [SerializeField, ReadOnly] Canvas canvas;
    [SerializeField] GameObject introduceUI;
    [SerializeField, ReadOnly] protected AnimalLogic dog;
    [SerializeField, ReadOnly] protected XRRayInteractor rayInteractor;

    [Header("이동 제한 영역 설정")]
    [SerializeField]
    private Vector3 minBounds = new Vector3(-5f, 0f, -5f); // X, Y, Z 최소 좌표
    [SerializeField]
    private Vector3 maxBounds = new Vector3(5f, 5f, 5f);   // X, Y, Z 최대 좌표

    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        dog = FindAnyObjectByType<AnimalLogic>();

        introduceUI = Resources.FindObjectsOfTypeAll<GameObject>()
                         .FirstOrDefault(obj => obj.name.Contains("Introduce"));

        canvas = introduceUI.GetComponentInParent<Canvas>();
        rb = GetComponent<Rigidbody>();

        // XRGrabInteractable 받아오기
        if (TryGetComponent(out grabInteractable)) Debug.LogWarning($"ObjectInteraction | grabInteractable이 Null입니다.");

        // Event 추가
        if (grabInteractable != null) grabInteractable.selectEntered.AddListener(OnObjectSelected);
        if (grabInteractable != null) grabInteractable.selectExited.AddListener(OnObjectExited);

        // Event 추가
        if (grabInteractable != null) grabInteractable.hoverEntered.AddListener(OnObjectHoverSelected);
        if (grabInteractable != null) grabInteractable.hoverExited.AddListener(OnObjectHoverExited);
    }

    void OnDestroy()
    {
        // Event 제거
        if (grabInteractable != null) grabInteractable.selectEntered.RemoveListener(OnObjectSelected);
        if (grabInteractable != null) grabInteractable.selectExited.RemoveListener(OnObjectExited);

        // Event 추가
        if (grabInteractable != null) grabInteractable.hoverEntered.RemoveListener(OnObjectHoverSelected);
        if (grabInteractable != null) grabInteractable.hoverExited.RemoveListener(OnObjectHoverExited);
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // 제한 영역의 중심 계산
        Vector3 center = (minBounds + maxBounds) / 2f;
        // 제한 영역의 크기 계산
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
    #region Selected
    private void OnObjectSelected(SelectEnterEventArgs args)
    {
        // args.interactorObject를 활용해 어떤 Interactor가 이 오브젝트를 선택했는지 판단 가능

        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            Debug.Log("Direct Grab");

            // Direct Grab 시 발생할 이벤트 처리
            HandleDirectGrabEvent();
        }
        else if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            Debug.Log("Ray Grab");

            // Ray Grab 시 발생할 이벤트 처리
            HandleRayGrabEvent();
        }
    }
    private void OnObjectExited(SelectExitEventArgs args)
    {
        // args.interactorObject를 활용해 어떤 Interactor가 이 오브젝트를 선택했는지 판단 가능

        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            Debug.Log("Direct Grab");

            // Direct Grab 시 발생할 이벤트 처리
            HandleDirectExitEvent();
        }
        else if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            Debug.Log("Ray Grab");

            // Ray Grab 시 발생할 이벤트 처리
            HandleRayExitEvent();
        }
    }
    #endregion

    #region Hover
    public void OnObjectHoverSelected(HoverEnterEventArgs args)
    {
        // args.interactorObject를 활용해 어떤 Interactor가 이 오브젝트를 선택했는지 판단 가능

        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            Debug.Log("Direct Grab");

            // Direct Grab 시 발생할 이벤트 처리
            HandleDirectGrabEvent();
        }
        else if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            Debug.Log("Ray Grab");

            // Ray Grab 시 발생할 이벤트 처리
            HandleRayGrabEvent();
        }
    }
    public void OnObjectHoverExited(HoverExitEventArgs args)
    {
        // args.interactorObject를 활용해 어떤 Interactor가 이 오브젝트를 선택했는지 판단 가능

        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            Debug.Log("Direct Grab");

            // Direct Grab 시 발생할 이벤트 처리
            HandleDirectExitEvent();
        }
        else if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            Debug.Log("Ray Grab");

            // Ray Grab 시 발생할 이벤트 처리
            HandleRayExitEvent();
        }
    }
    #endregion
    // Direct Grab 시 실행될 Method
    public void HandleDirectGrabEvent()
    {
        // ShowUI(introduceUI);
        // if (input.selectedItem != null) input.selectedItem = this.gameObject;
    }

    // Ray Grab 시 실행될 Method
    public void HandleRayGrabEvent()
    {
        // ShowUI(introduceUI);
    }
    private void HandleDirectExitEvent()
    {
        introduceUI.SetActive(false);
    }

    // Ray Grab 시 실행될 Method
    public void HandleRayExitEvent()
    {
        introduceUI.SetActive(false);
    }
}



    // public void Select_Enter(SelectEnterEventArgs args)
    // {
    //     rayInteractor = args.interactorObject as XRRayInteractor;
    //     if (rayInteractor != null)
    //     {
    //         currentRayLineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();
    //         if (currentRayLineVisual != null) currentRayLineVisual.enabled = false;

    //         ShowUI(getUI);
    //         var pickupButton = getUI.GetComponentInChildren<ItemPickupButton>();
    //         if (pickupButton != null) pickupButton.itemToPickup = this.gameObject;
    //         introduceUI.SetActive(false);
    //         isSelect = true;
    //     }
    // }
    // public void Select_Exit()
    // {
    //     getUI.SetActive(false);
    // }

    // public void Hover_Enter()
    // {
    //     getUI.SetActive(false);
    //     if (!isSelect) ShowUI(introduceUI);
    // }

    // public void Hover_Exit()
    // {
    //     introduceUI.SetActive(false);
    // }
        // void ShowUI(GameObject ui)
    // {
    //     if (ui.activeInHierarchy == false) ui.SetActive(true);

    //     Renderer currentRenderer = ui.GetComponent<MeshRenderer>();
    //     if (currentRenderer == null) return;
    //     Vector3 worldTopRight = currentRenderer.bounds.max * 2;
    //     Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldTopRight);
    //     Vector2 localPoint;
    //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //         canvas.GetComponent<RectTransform>(),
    //         screenPosition,
    //         canvas.worldCamera,
    //         out localPoint
    //     );
    //     ui.GetComponent<RectTransform>().anchoredPosition = localPoint;
    // }