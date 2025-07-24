using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    public Transform movingTargetTransform;

    void Start()
    {
        movingTargetTransform = FindAnyObjectByType<Player>().transform;
        if (movingTargetTransform == null)
        {
            Debug.LogWarning($"PlayerPositionLoader | movingTargetTransform이 Null입니다.");
            return;
        }
        if (GameSaveManager.Instance != null && movingTargetTransform != null)
        {
            Vector3 savedPos = GameSaveManager.Instance.GetPlayerPosition();
            movingTargetTransform.position = savedPos;
            Debug.Log($"📍 불러오기 위치로 이동: {savedPos}");
        }
    }
}
