using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;

public class GrabEvent : MonoBehaviour
{
    [SerializeField] GameObject visual;
    Renderer rend;
    Coroutine coroutine = null;

    void Awake()
    {
        if (visual.TryGetComponent(out rend))
            Debug.LogWarning("없어!?");
    }
    public void Select_Enter()
    {
        rend.material.color = Color.green;
    }
    public void Select_Exit()
    {
        // Exit가 호출되면 일정 시간 동안 red의 색으로 변경하였다가
        // 다시 본래의 색으로 돌아옴
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(Exit_Co());
    }

    IEnumerator Exit_Co()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(3f);
        rend.material.color = Color.green;
    }
}
