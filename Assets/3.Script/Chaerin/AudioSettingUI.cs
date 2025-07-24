using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("슬라이더")]
    public Slider volumeSlider_bgm;
    public Slider volumeSlider_sfx;

    [Header("버튼")]
    public Button applyButton;
    public Button backButton;

    void OnEnable()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager 인스턴스가 없습니다.");
            return;
        }

        // 기존 리스너 제거
        volumeSlider_bgm?.onValueChanged.RemoveAllListeners();
        volumeSlider_sfx?.onValueChanged.RemoveAllListeners();
        applyButton?.onClick.RemoveAllListeners();
        backButton?.onClick.RemoveAllListeners();

        // 슬라이더 초기화
        if (volumeSlider_bgm != null)
        {
            volumeSlider_bgm.value = AudioManager.Instance.BgmVolume;
            volumeSlider_bgm.onValueChanged.AddListener((v) =>
            {
                AudioManager.Instance.BgmVolume = v;
                AudioManager.Instance.ApplyVolumes();
            });
        }

        if (volumeSlider_sfx != null)
        {
            volumeSlider_sfx.value = AudioManager.Instance.SfxVolume;
            volumeSlider_sfx.onValueChanged.AddListener((v) =>
            {
                AudioManager.Instance.SfxVolume = v;
                AudioManager.Instance.ApplyVolumes();
            });
        }

        applyButton?.onClick.AddListener(Apply);
        backButton?.onClick.AddListener(CloseUI);
    }

    public void Apply()
    {
        AudioManager.Instance.SaveVolumes();
        Debug.Log("[AudioSettingsUI] 설정 적용됨");
        CloseUI();
    }

    public void CloseUI()
    {
        Debug.Log("[AudioSettingsUI] 옵션 창 닫기");
        gameObject.SetActive(false);
    }
}
