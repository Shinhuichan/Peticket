using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomRayGrabHandler : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private XRInteractorLineVisual currentRayLineVisual;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable == null)
        {
            Debug.LogError("XRGrabInteractable 컴포넌트를 찾을 수 없어요. 이 스크립트는 XRGrabInteractable이 있는 오브젝트에 붙여야 해요!");
            enabled = false;
        }
    }

    void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        XRRayInteractor rayInteractor = args.interactorObject as XRRayInteractor;

        if (rayInteractor != null)
        {
            currentRayLineVisual = rayInteractor.GetComponent<XRInteractorLineVisual>();
            if (currentRayLineVisual != null) currentRayLineVisual.enabled = false;

            // 이제 잡은 물건을 컨트롤러(손)의 위치로 이동시켜야겠지?
            // 물건의 부모를 Ray Interactor의 Transform으로 설정해서 손을 따라다니게 해
            transform.SetParent(rayInteractor.transform);
            // 그리고 물건의 로컬 위치와 회전을 초기화해서 손의 정확한 위치에 딱! 붙게 만들어
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            // 물건이 손에 잡혀있는 동안 물리적인 영향을 받지 않도록 Rigidbody의 isKinematic을 true로 바꿔주는 게 좋아
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true; // 물리 작용 비활성화
            }
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        // 만약 이전에 Ray Line Visual을 껐었다면, 다시 켜줘야겠지?
        if (currentRayLineVisual != null)
        {
            currentRayLineVisual.enabled = true; // 다시 Line Renderer를 켜줘
            currentRayLineVisual = null; // 참조는 다시 초기화해주는 게 깔끔해!
        }

        // 물건을 놓았으니, 부모 관계를 해제해서 다시 독립적인 오브젝트로 만들어줘
        transform.SetParent(null);

        // 물리 작용을 다시 활성화 (isKinematic을 false로)
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false; // 물리 작용 활성화
            // (참고: 놓을 때 자연스러운 움직임을 위해 velocity 등을 조절할 수도 있어!)
        }
    }
}
