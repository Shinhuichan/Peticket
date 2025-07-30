using CustomInspector;
using UnityEngine;

public class PoopEvent : MonoBehaviour
{
    [SerializeField, ReadOnly] bool isExit = false;
    void OnTriggerExit(Collider other)
    {
        // 탈출한 Object가 Player이면서 Player가 이미 한 번이라도 나간 적이 없다면 true
        if (other.tag.Contains("Player") && !isExit)
        {
            isExit = true;

            // 진행도 차감
        }
    }

    void OnDestroy()
    {
        // 청소되어 제거될 때
        // 진행도 회복
    }
}