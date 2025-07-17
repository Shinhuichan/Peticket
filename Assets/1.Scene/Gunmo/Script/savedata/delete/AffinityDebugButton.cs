using UnityEngine;
using UnityEngine.UI;

public class AffinityDebugButton : MonoBehaviour
{
    [Header("연결할 버튼")]
    public Button increaseButton;
    public Button decreaseButton;

    [Header("변화량 설정")]
    public float amount = 10f; // 기본 변화량 (진행도 % 기준)

    private void Start()
    {
        if (increaseButton != null)
            increaseButton.onClick.AddListener(() => ChangeProgress(+amount));

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(() => ChangeProgress(-amount));
    }

    private void ChangeProgress(float value)
{
    if (string.IsNullOrEmpty(GameSaveManager.Instance.currentSaveData.selectedPetId))
    {
        Debug.LogWarning("❌ 선택된 펫 ID가 없습니다.");
        return;
    }

    // ✅ 진행도 변경 + 자동 저장
    GameSaveManager.Instance.SetPlayerProgress(value);

    // UI 갱신
    var progressUI = FindObjectOfType<PetProgressUI>();
    if (progressUI != null)
    {
        float progress = GameSaveManager.Instance.currentSaveData.playerProgress;
        progressUI.UpdateProgressUI(progress);
    }
}
}
