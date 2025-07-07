using System.Collections;
using UnityEngine;

public class ObjectInteraction : MonoBehaviour
{
    [SerializeField] GameObject visual;
    Canvas canvas;
    MeshRenderer rend;
    Coroutine coroutine = null;


    [SerializeField] GameObject introduceUI;
    [SerializeField] GameObject getUI;

    void Awake()
    {
        if (visual.TryGetComponent(out rend)) Debug.LogWarning("없어!?");
        canvas = introduceUI.GetComponentInParent<Canvas>();
    }
    public void Select_Enter()
    {
        rend.material.color = Color.green;
        ShowUI(getUI);
    }
    public void Select_Exit()
    {
        // Exit가 호출되면 일정 시간 동안 red의 색으로 변경하였다가
        // 다시 본래의 색으로 돌아옴
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(Exit_Co());
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
}