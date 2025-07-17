using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetAffinitySliderUI : MonoBehaviour
{
    public Slider affinitySlider;
    public TextMeshProUGUI affinityValueText;
    public TextMeshProUGUI affinityStageText;
    public Image sliderFillImage;

    private string currentPetId;

    private void Update()
    {
        if (string.IsNullOrEmpty(currentPetId)) return;

        float affinity = FindObjectOfType<PetAffinityManager>().GetAffinity(currentPetId);
        affinitySlider.value = affinity / 100f;
        affinityValueText.text = $"{(int)affinity} / 100";
        affinityStageText.text = GetAffinityStage(affinity);
        sliderFillImage.color = GetAffinityColor(affinity);
    }

    public void SetTargetPet(string petId)
    {
        currentPetId = petId;
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
}
