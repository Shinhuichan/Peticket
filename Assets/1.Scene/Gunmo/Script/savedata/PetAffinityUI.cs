using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetAffinityUI : MonoBehaviour
{
    public TextMeshProUGUI petNameText;
    public Slider affinitySlider;
    public TextMeshProUGUI affinityValueText;
    public TextMeshProUGUI affinityStageText;
    public Image sliderFillImage;

    public Button selectButton; // ✅ 선택 버튼

    private string petId;
    private System.Action<string> onSelectedCallback;

    public void Initialize(string id, string petName, System.Action<string> onSelected = null)
    {
        petId = id;
        petNameText.text = petName;
        onSelectedCallback = onSelected;

        if (selectButton != null)
{
    selectButton.onClick.AddListener(() =>
    {
        onSelectedCallback?.Invoke(petId);
        selectButton.gameObject.SetActive(false); // ✅ 버튼 숨기기
    });
}

        Refresh();
    }

    public void Refresh()
    {
        float affinity = FindObjectOfType<PetAffinityManager>().GetAffinity(petId);
        affinitySlider.value = affinity / 100f;
        affinityValueText.text = $"{(int)affinity} / 100";
        affinityStageText.text = GetAffinityStage(affinity);
        if (sliderFillImage != null)
            sliderFillImage.color = GetAffinityColor(affinity);
    }

    private string GetAffinityStage(float affinity)
    {
        if (affinity < 20f) return "낯선 사이";
        else if (affinity < 50f) return "호감 있음";
        else if (affinity < 80f) return "친한 사이";
        else return "절친";
    }

    private Color GetAffinityColor(float affinity)
    {
        if (affinity < 20f) return new Color32(136, 136, 136, 255);
        else if (affinity < 50f) return new Color32(58, 141, 255, 255);
        else if (affinity < 80f) return new Color32(255, 165, 0, 255);
        else return new Color32(0, 200, 81, 255);
    }
    public void ResetSelection()
{
    gameObject.SetActive(true); // 숨겨졌던 경우 다시 보이기

    if (selectButton != null)
    {
        selectButton.gameObject.SetActive(true); // 버튼이 꺼졌다면 다시 활성화
    }
}


    public string GetPetId() => petId;
    
}
