using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM")]
    public AudioSource bgmSource_bgm;
    public Slider volumeSlider_bgm;

    [Header("SFX")]
    public AudioSource sfxSourcePrefab;
    public Slider volumeSlider_sfx;

    [Header("Buttons")]
    public Button applyButton;
    public Button backButton;

    private float currentBgmVolume;
    private float currentSfxVolume;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 저장된 볼륨 값 불러오기
        currentBgmVolume = PlayerPrefs.GetFloat("BGM", 1.0f);
        currentSfxVolume = PlayerPrefs.GetFloat("SFX", 1.0f);

        // 오디오 반영
        bgmSource_bgm.volume = currentBgmVolume;

        // UI에 슬라이더 반영
        if (volumeSlider_bgm != null)
        {
            volumeSlider_bgm.value = currentBgmVolume;
            volumeSlider_bgm.onValueChanged.AddListener((v) =>
            {
                bgmSource_bgm.volume = v;
            });
        }

        if (volumeSlider_sfx != null)
        {
            volumeSlider_sfx.value = currentSfxVolume;
            volumeSlider_sfx.onValueChanged.AddListener((v) =>
            {
                currentSfxVolume = v; // SFX는 현재 AudioSource 생성 시 반영
            });
        }

        // BGM 시작
        if (!bgmSource_bgm.isPlaying)
            bgmSource_bgm.Play();

        // 버튼 연결
        if (applyButton != null)
            applyButton.onClick.AddListener(Apply);

        if (backButton != null)
            backButton.onClick.AddListener(CloseUI);
    }

    /// <summary>
    /// 적용 버튼: 현재 슬라이더 값을 저장하고 UI 닫기
    /// </summary>
    public void Apply()
    {
        float bgm = volumeSlider_bgm.value;
        float sfx = volumeSlider_sfx.value;

        PlayerPrefs.SetFloat("BGM", bgm);
        PlayerPrefs.SetFloat("SFX", sfx);
        PlayerPrefs.Save();

        currentBgmVolume = bgm;
        currentSfxVolume = sfx;

        CloseUI();
    }


    /// 뒤로가기 또는 적용 후: 설정창 닫기
    public void CloseUI()
    {
        this.gameObject.SetActive(false);
    }

       public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSourcePrefab == null) return;

        AudioSource sfx = Instantiate(sfxSourcePrefab, transform);
        sfx.volume = currentSfxVolume;
        sfx.clip = clip;
        sfx.Play();
        Destroy(sfx.gameObject, clip.length + 0.1f);
    }
}
