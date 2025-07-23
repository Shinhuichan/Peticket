using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PetProgressUI : MonoBehaviour
{
    public static PetProgressUI Instance { get; private set; }

    [Header("진행도 UI")]
    public Slider progressSlider;
    public TextMeshProUGUI progressText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad 제거됨
        }
        else
        {
            Destroy(gameObject); // 중복 방지
            return;
        }
    }

    private void Start()
    {
        // 저장된 진행도 불러와 UI 초기화
        float savedProgress = GameSaveManager.Instance?.currentSaveData?.playerProgress ?? 0f;
        UpdateProgressUI(savedProgress);

        // 진행도 변화 이벤트 연결
        GameSaveManager.Instance.OnProgressChanged += UpdateProgressUI;
    }

    private void OnDestroy()
    {
        if (GameSaveManager.Instance != null)
        {
            GameSaveManager.Instance.OnProgressChanged -= UpdateProgressUI;
        }
    }

    /// <summary>
    /// 외부에서 호출 가능한 진행도 UI 갱신
    /// </summary>
    /// <param name="value">0~100 사이의 진행도 값</param>
    public void UpdateProgressUI(float value)
    {
        float clamped = Mathf.Clamp01(value / 100f);

        if (progressSlider != null)
            progressSlider.value = clamped;

        if (progressText != null)
            progressText.text = $"진행도: {(int)value}%";
    }
}
