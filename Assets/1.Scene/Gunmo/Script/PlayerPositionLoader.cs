using UnityEngine;

public class PlayerPositionLoader : MonoBehaviour
{
    public Transform movingTargetTransform; // XR Origin

    void Start()
    {
        if (GameSaveManager.Instance != null && movingTargetTransform != null)
        {
            Vector3 savedPos = GameSaveManager.Instance.GetPlayerPosition();
            movingTargetTransform.position = savedPos;
            Debug.Log($"📍 불러오기 위치로 이동: {savedPos}");
        }
    }
}
