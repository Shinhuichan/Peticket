using UnityEngine;
using UnityEngine.UI; // UI 표시용, 필요 시 제거 가능

public class LeashControllerManager : MonoBehaviour
{
    private AnimalLogic dog;

    private bool isLeashed = false;

    void Start()
    {
        dog = FindObjectOfType<AnimalLogic>();
        if (dog == null)
        {
            Debug.LogWarning("개 없음!");
        }
    }

    public void ToggleLeash()
    {
        if (dog == null) return;

        isLeashed = !isLeashed;
        dog.SetLeashed(isLeashed);

        Debug.Log($"[LeashControllerManager] 줄 상태: {(isLeashed ? "연결됨" : "해제됨")}");
    }
}
