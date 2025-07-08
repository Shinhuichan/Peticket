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

        currentPosition.x = Mathf.Clamp(currentPosition.x, minBounds.x, maxBounds.x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, minBounds.y, maxBounds.y);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minBounds.z, maxBounds.z);

        transform.position = currentPosition;

        if (currentPosition.x >= maxBounds.x || currentPosition.y >= maxBounds.y || currentPosition.z >= maxBounds.z)
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
    }

    //  에디터에서 제한 영역을 시각적으로 보여주기
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