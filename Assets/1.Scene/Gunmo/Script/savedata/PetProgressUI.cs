using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PetProgressUI : MonoBehaviour
{
    public static PetProgressUI Instance { get; private set; }

    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
    }

    private void Start()
    {
        float savedProgress = GameSaveManager.Instance.currentSaveData.playerProgress;
        UpdateProgressUI(savedProgress);

        GameSaveManager.Instance.OnProgressChanged += UpdateProgressUI;
    }

    private void OnDestroy()
    {
        if (GameSaveManager.Instance != null)
            GameSaveManager.Instance.OnProgressChanged -= UpdateProgressUI;
    }

    public void UpdateProgressUI(float value)
    {
        float clamped = Mathf.Clamp01(value / 100f);
        progressSlider.value = clamped;
        progressText.text = $"진행도: {(int)value}%";
    }
}
