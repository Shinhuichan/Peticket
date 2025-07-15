using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PetAffinityUI : MonoBehaviour
{
    public TextMeshProUGUI petNameText;
    public Slider affinitySlider;
    public TextMeshProUGUI affinityStageText;

    private string petId;

    public void Initialize(string id, string petName)
    {
        petId = id;
        petNameText.text = petName;
        Refresh();
    }

    public void Refresh()
    {
        float affinity = FindObjectOfType<PetAffinityManager>().GetAffinity(petId);
        affinitySlider.value = affinity / 100f;
        affinityStageText.text = GetAffinityStage(affinity);
    }

    private string GetAffinityStage(float affinity)
    {
        if (affinity < 20f) return "낯선 사이";
        else if (affinity < 50f) return "호감 있음";
        else if (affinity < 80f) return "친한 사이";
        else return "절친";
    }
}
