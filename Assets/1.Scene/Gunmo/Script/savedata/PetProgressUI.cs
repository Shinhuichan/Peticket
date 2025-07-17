using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PetProgressUI : MonoBehaviour
{
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    private void Start()
    {
        float savedProgress = GameSaveManager.Instance.currentSaveData.playerProgress;
        UpdateProgressUI(savedProgress);
    }

    public void UpdateProgressUI(float value)
    {
        float clamped = Mathf.Clamp01(value / 100f);
        progressSlider.value = clamped;
        progressText.text = $"진행도: {(int)value}%";
    }
}
