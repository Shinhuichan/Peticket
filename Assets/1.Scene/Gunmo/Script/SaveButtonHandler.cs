using UnityEngine;

public class SaveButtonHandler : MonoBehaviour
{
    public Transform movingTargetTransform; // XR Origin 또는 움직이는 루트

    public void OnClick_SaveManually()
    {
        if (GameSaveManager.Instance == null || movingTargetTransform == null)
        {
            Debug.LogError("❌ 저장 실패: 대상이 없습니다.");
            return;
        }

        Vector3 pos = movingTargetTransform.position;
        GameSaveManager.Instance.SaveGame(pos);
        Debug.Log($"💾 저장됨: {pos}");
    }
}
