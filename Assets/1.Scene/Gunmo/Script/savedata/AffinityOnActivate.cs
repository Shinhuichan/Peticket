using UnityEngine;

public class ProgressOnActivate : MonoBehaviour
{
    [Header("이 오브젝트가 활성화될 때 진행도를 증가시킵니다.")]
    [Tooltip("증가시킬 진행도 수치 (0~100 사이)")]
    public float progressAmount = 10f;

    [Header("자동 비활성화 설정")]
    [Tooltip("활성화 후 몇 초 뒤에 이 오브젝트를 비활성화할지 설정")]
    public float autoDisableDelay = 5f;

    private bool hasApplied = false;

    private void OnEnable()
    {
        if (hasApplied) return; // ✅ 이미 적용됐으면 무시
        hasApplied = true;

        if (GameSaveManager.Instance == null)
        {
            Debug.LogWarning("❌ GameSaveManager 인스턴스를 찾을 수 없습니다.");
            return;
        }

        string selectedPetId = GameSaveManager.Instance.currentSaveData?.selectedPetId;

        if (string.IsNullOrEmpty(selectedPetId))
        {
            Debug.LogWarning("❌ 선택된 펫이 없습니다. 진행도 증가 생략");
            return;
        }

        // 진행도 증가 및 저장
        GameSaveManager.Instance.SetPlayerProgress(progressAmount);
        Debug.Log($"✅ 진행도 {progressAmount} 증가 (펫: {selectedPetId})");

        // 🔻 5초 후 비활성화 시작
        Invoke(nameof(DisableSelf), autoDisableDelay);
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
