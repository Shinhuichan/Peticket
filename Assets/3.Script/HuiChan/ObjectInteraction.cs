using System.Collections;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    Canvas canvas;
    MeshRenderer rend;

    [SerializeField] GameObject introduceUI;
    [SerializeField] GameObject getUI;

    [Header("이동 제한 영역 설정")]
    [SerializeField]
    private Vector3 minBounds = new Vector3(-5f, 0f, -5f); // X, Y, Z 최소 좌표
    [SerializeField]
    private Vector3 maxBounds = new Vector3(5f, 5f, 5f);   // X, Y, Z 최대 좌표

    // 만약 이 오브젝트가 Rigidbody를 가지고 있다면 참조
    private Rigidbody rb;

    void Awake()
    {
        canvas = introduceUI.GetComponentInParent<Canvas>();
        rb = GetComponent<Rigidbody>();
    }
    public void Select_Enter()
    {
        ShowUI(getUI);
    }
    public void Select_Exit()
    {
        getUI.SetActive(false);
    }

    IEnumerator Exit_Co()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(3f);
        rend.material.color = Color.green;
    }

    public void Hover_Enter()
    {
        ShowUI(introduceUI);
    }

    public void Hover_Exit()
    {
        introduceUI.SetActive(false);
    }

    void ShowUI(GameObject ui)
    {
        ui.SetActive(true);
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
        LimitPosition();
    }

    private void LimitPosition()
    {
        Vector3 currentPosition = transform.position;

        // 각 축별로 위치를 제한 (Clamp)
        currentPosition.x = Mathf.Clamp(currentPosition.x, minBounds.x, maxBounds.x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minBounds.y, maxBounds.y);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minBounds.z, maxBounds.z);

        // 제한된 위치를 오브젝트에 적용
        transform.position = currentPosition;

        // 만약 Rigidbody가 있다면, 속도도 0으로 만들어줘서 계속 경계 밖으로 밀려나지 않도록 해.
        // 강제로 위치를 옮겼기 때문에 물리 엔진이 이상하게 동작할 수 있거든.
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero; // 회전 속도도 0으로
        }
    }

    // 디버깅을 위해 에디터에서 제한 영역을 시각적으로 보여주기 (선택 사항)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        // 제한 영역의 중심 계산
        Vector3 center = (minBounds + maxBounds) / 2f;
        // 제한 영역의 크기 계산
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}