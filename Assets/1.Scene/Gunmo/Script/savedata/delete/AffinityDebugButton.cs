using UnityEngine;
using UnityEngine.UI;

public class AffinityDebugButton : MonoBehaviour
{
    [Header("연결할 버튼")]
    public Button increaseButton;
    public Button decreaseButton;

    [Header("변화량 설정")]
    public float amount = 10f; // 기본 변화량

    private void Start()
    {
        if (increaseButton != null)
            increaseButton.onClick.AddListener(() => ChangeAffinity(+amount));

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(() => ChangeAffinity(-amount));
    }

    private void ChangeAffinity(float value)
    {
        // 선택된 펫 ID 불러오기
        string selectedId = GameSaveManager.Instance.currentSaveData.selectedPetId;

        if (string.IsNullOrEmpty(selectedId))
        {
            Debug.LogWarning("❌ 선택된 반려동물이 없습니다. 먼저 펫을 선택하세요.");
            return;
        }

        // PetAffinityManager 존재 여부 확인
        var manager = FindObjectOfType<PetAffinityManager>();
        if (manager == null)
        {
            Debug.LogWarning("❌ PetAffinityManager를 찾을 수 없습니다. 씬에 배치되어 있는지 확인하세요.");
            return;
        }

        // 친밀도 반영 및 저장
        manager.ChangeAffinityAndSave(selectedId, value);
        Debug.Log($"✅ {selectedId} 친밀도 {(value > 0 ? "+" : "")}{value} 변화됨");

        // UI 갱신
        var uiManager = FindObjectOfType<PetUIManager>();
        if (uiManager != null)
        {
            uiManager.RefreshAllPetUIs();
        }
        else
        {
            Debug.Log("ℹ PetUIManager를 찾을 수 없어 UI는 수동으로 갱신되지 않았습니다.");
        }
    }
}
