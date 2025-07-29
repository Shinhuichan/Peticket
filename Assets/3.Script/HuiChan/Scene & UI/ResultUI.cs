using CustomInspector;
using TMPro;
using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [Header("UI Setting")]
    [SerializeField] TextMeshProUGUI petIDUI;
    [SerializeField] TextMeshProUGUI progressUI;
    [Header("Save Data Setting")]
    // 선택된 Pet
    [SerializeField, ReadOnly] string petId;
    // 진행도
    [SerializeField, ReadOnly] float progress;
    void OnEnable()
    {
        // Data 불러오기
        petId = GameSaveManager.Instance.currentSaveData.selectedPetId;
        progress = GameSaveManager.Instance.currentSaveData.playerProgress;
        UISet();
    }

    void UISet()
    {
        switch (petId)
        {
            case "small":
                petIDUI.text = "소형견";
                break;
            case "middle":
                petIDUI.text = "중형견";
                break;
            case "large":
                petIDUI.text = "대형견";
                break;
        }
        progressUI.text = $"{progress}%";
    }
}
