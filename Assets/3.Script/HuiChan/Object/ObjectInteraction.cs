using CustomInspector;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public abstract class ObjectInteraction : MonoBehaviour
{
    CollectableData objType = CollectableData.None;
    [SerializeField, ReadOnly] protected AnimalLogic dog;
    [SerializeField, ReadOnly] protected XRRayInteractor rayInteractor;

    [Header("이동 제한 영역 설정")]
    [SerializeField]
    private Rect rectXZ = new Rect(-5f, -5f, 10f, 10f); // X, Z 평면 (x, y, width, height) -> xMin, zMin, width, height
    [SerializeField]
    private float minY = 0f; // Y축 최소 좌표
    [SerializeField]
    private float maxY = 5f;   // Y축 최대 좌표

    private Rigidbody rb;
    [SerializeField, ReadOnly] private XRGrabInteractable grabInteractable;

    void Awake()
    {
        dog = FindAnyObjectByType<AnimalLogic>();
        rb = GetComponent<Rigidbody>();

        if (!TryGetComponent(out grabInteractable)) Debug.LogWarning($"ObjectInteraction | {gameObject.name}에 XRGrabInteractable 컴포넌트가 없습니다.");

        // Event 추가
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnObjectSelected);
            grabInteractable.selectExited.AddListener(OnObjectExited);
            grabInteractable.hoverEntered.AddListener(OnObjectHoverSelected);
            grabInteractable.hoverExited.AddListener(OnObjectHoverExited);
        }
        else Debug.LogWarning($"ObjectInteraction | {gameObject.name}의 grabInteractable이 할당되지 않아 XR Interaction 이벤트를 등록할 수 없습니다.");
    }

    void Start()
    {
        if (ItemUseZoneManager.Instance != null) 
            foreach (var zone in ItemUseZoneManager.Instance.zones)
            {
                if (objType.ToString().Contains(zone.zoneName))
                {
                    rectXZ = zone.rectXZ;
                    minY = zone.minY;
                    maxY = zone.maxY;
                }
            }
    }
    void OnDestroy()
    {
        // Event 제거 (AddListener에 해당하는 RemoveListener는 모두 작성하는 것이 중요)
        if (grabInteractable != null) // null 체크는 중요해!
        {
            grabInteractable.selectEntered.RemoveListener(OnObjectSelected);
            grabInteractable.selectExited.RemoveListener(OnObjectExited);
            // hover 이벤트도 OnDestroy에서 RemoveListener를 해줘야 해!
            grabInteractable.hoverEntered.RemoveListener(OnObjectHoverSelected);
            grabInteractable.hoverExited.RemoveListener(OnObjectHoverExited);
        }
    }
    void FixedUpdate()
    {
        Vector3 currentPosition = transform.position;

        float clampedX = Mathf.Clamp(currentPosition.x, rectXZ.xMin, rectXZ.xMax);
        float clampedY = Mathf.Clamp(currentPosition.y, minY, maxY);
        float clampedZ = Mathf.Clamp(currentPosition.z, rectXZ.yMin, rectXZ.yMax);

        Vector3 clampedPosition = new Vector3(clampedX, clampedY, clampedZ);
        if (currentPosition != clampedPosition)
        {
            if (rb != null) rb.position = clampedPosition;
            else transform.position = clampedPosition;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            rectXZ.center.x,
            (minY + maxY) / 2f,
            rectXZ.center.y
        );
        Vector3 size = new Vector3(
            rectXZ.width,
            maxY - minY,
            rectXZ.height
        );
        Gizmos.DrawWireCube(center, size);
    }

    #region Selected
    private void OnObjectSelected(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            Debug.Log($"ObjectInteraction | {gameObject.name} : Direct Grab");

            // Direct Grab 시 발생할 이벤트 처리
            HandleDirectGrabEvent();
        }
        else if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            Debug.Log($"ObjectInteraction | {gameObject.name} : Ray Grab");
            this.rayInteractor = rayInteractor; // Ray Grab 시 rayInteractor 참조 저장 (필요하다면)
            // Ray Grab 시 발생할 이벤트 처리
            HandleRayGrabEvent();
        }
    }
    private void OnObjectExited(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor directInteractor)
        {
            Debug.Log($"ObjectInteraction | {gameObject.name} : Direct Exit");

            HandleDirectExitEvent();
        }
        else if (args.interactorObject is XRRayInteractor rayInteractor)
        {
            Debug.Log($"ObjectInteraction | {gameObject.name} : Ray Exit");
            this.rayInteractor = null; // Ray Exit 시 참조 해제 (필요하다면)
            HandleRayExitEvent();
        }
    }
    #endregion

    #region Hover
    public void OnObjectHoverSelected(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor directInteractor) Debug.Log($"ObjectInteraction | {gameObject.name} : Direct Hover Enter");
        else if (args.interactorObject is XRRayInteractor rayInteractor) Debug.Log($"ObjectInteraction | {gameObject.name} : Ray Hover Enter");
    }
    public void OnObjectHoverExited(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRDirectInteractor directInteractor) Debug.Log($"ObjectInteraction | {gameObject.name} : Direct Hover Exit");
        else if (args.interactorObject is XRRayInteractor rayInteractor) Debug.Log($"ObjectInteraction | {gameObject.name} : Ray Hover Exit");
    }
    #endregion

    public virtual void HandleDirectGrabEvent() { }

    public virtual void HandleRayGrabEvent() { }

    public virtual void HandleDirectExitEvent() { }

    public virtual void HandleRayExitEvent() { }

    public virtual void UseObject() { }
}