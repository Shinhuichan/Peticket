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

        // Rigidbody가 있다면 제한된 속도를 적용
        if (rb != null)
        {
            rb.velocity = clampedVelocity;
            // rb.angularVelocity = Vector3.zero;
        }
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
}