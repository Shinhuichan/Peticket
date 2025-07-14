using System.Linq;
using CustomInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField, ReadOnly] Canvas canvas;
    [SerializeField] GameObject introduceUI;
    [SerializeField] GameObject getUI;
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
        getUI = Resources.FindObjectsOfTypeAll<GameObject>()
                         .FirstOrDefault(obj => obj.name.Contains("Get"));

        if (getUI == null || introduceUI == null) return;

        canvas = introduceUI.GetComponentInParent<Canvas>();
        rb = GetComponent<Rigidbody>();

        // XRGrabInteractable 받아오기
        if (TryGetComponent(out grabInteractable)) Debug.LogWarning($"ObjectInteraction | grabInteractable이 Null입니다.");

        // Event 추가
        if (grabInteractable != null) grabInteractable.selectEntered.AddListener(OnObjectSelected);
    }

    void OnDestroy()
    {
        // Event 제거
        if (grabInteractable != null) grabInteractable.selectEntered.RemoveListener(OnObjectSelected);
    }

    bool isSelect = false;
    private XRInteractorLineVisual currentRayLineVisual;
    public void Select_Enter(SelectEnterEventArgs args)
    {
        rayInteractor = args.interactorObject as XRRayInteractor;
        if (rayInteractor != null)
        {
            currentRayLineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();
            if (currentRayLineVisual != null) currentRayLineVisual.enabled = false;

            ShowUI(getUI);
            var pickupButton = getUI.GetComponentInChildren<ItemPickupButton>();
            if (pickupButton != null) pickupButton.itemToPickup = this.gameObject;
            introduceUI.SetActive(false);
            isSelect = true;
        }
    }
    public void Select_Exit()
    {
        getUI.SetActive(false);
    }

    public void Hover_Enter()
    {
        getUI.SetActive(false);
        if (!isSelect) ShowUI(introduceUI);
    }

    public void Hover_Exit()
    {
        introduceUI.SetActive(false);
    }

    void ShowUI(GameObject ui)
    {
        if (ui.activeInHierarchy == false) ui.SetActive(true);

        Renderer currentRenderer = ui.GetComponent<MeshRenderer>();
        if (currentRenderer == null) return;
        Vector3 worldTopRight = currentRenderer.bounds.max * 2;
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(worldTopRight);
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.GetComponent<RectTransform>(),
            screenPosition,
            canvas.worldCamera,
            out localPoint
        );
        ui.GetComponent<RectTransform>().anchoredPosition = localPoint;
    }

    void LateUpdate()
    {
        LimitPositionAndVelocity();
    }

    private void LimitPositionAndVelocity()
    {
        Vector3 currentPosition = transform.position;
        Vector3 currentVelocity = rb != null ? rb.velocity : Vector3.zero;

        // 제한된 위치를 계산할 변수
        Vector3 clampedPosition = currentPosition;
        // 제한된 속도를 계산할 변수
        Vector3 clampedVelocity = currentVelocity;

        // X축 제한
        if (currentPosition.x < minBounds.x)
        {
            clampedPosition.x = minBounds.x;
            if (currentVelocity.x < 0) clampedVelocity.x = 0;
        }
        else if (currentPosition.x > maxBounds.x)
        {
            clampedPosition.x = maxBounds.x;
            if (currentVelocity.x > 0) clampedVelocity.x = 0;
        }

        // Y축 제한
        if (currentPosition.y < minBounds.y)
        {
            clampedPosition.y = minBounds.y;
            // 바닥에 닿았을 때 위로 올라가는 속도만 0으로 (아래로 떨어지는 속도는 유지)
            // 즉, currentVelocity.y가 음수(아래로 떨어지는 중)일 때는 속도를 0으로 만들지 않음
            if (currentVelocity.y < 0) clampedVelocity.y = 0;
        }
        else if (currentPosition.y > maxBounds.y)
        {
            clampedPosition.y = maxBounds.y;
            if (currentVelocity.y > 0) clampedVelocity.y = 0;
        }

        // Z축 제한
        if (currentPosition.z < minBounds.z)
        {
            clampedPosition.z = minBounds.z;
            if (currentVelocity.z < 0) clampedVelocity.z = 0;
        }
        else if (currentPosition.z > maxBounds.z)
        {
            clampedPosition.z = maxBounds.z;
            if (currentVelocity.z > 0) clampedVelocity.z = 0;
        }

        // 제한된 위치를 오브젝트에 적용
        transform.position = clampedPosition;
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

    // Direct Grab 시 실행될 Method
    private void HandleDirectGrabEvent()
    {

    }

    // Ray Grab 시 실행될 Method
    private void HandleRayGrabEvent()
    {
        ShowUI(getUI);
    }
    private void HandleDirectExitEvent()
    {

    }

    // Ray Grab 시 실행될 Method
    private void HandleRayExitEvent()
    {
        getUI.SetActive(false);
    }
}