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

    [Header("Hit Sound")]
    public AudioSource bgmSource_hit;
    public Slider volumeSlider_hit;

    [Header("Run Sound")]
    public AudioSource bgmSource_run;
    public Slider volumeSlider_run;

    void Awake()
    {
        // 싱글톤 처리
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 중복 방지: 기존 인스턴스 유지
            return;
        }
    }

    void Start()
    {
        // 볼륨 로딩
        float savedBgm = PlayerPrefs.GetFloat("BGM", 1.0f);
        bgmSource_bgm.volume = savedBgm;
        if (volumeSlider_bgm != null) volumeSlider_bgm.value = savedBgm;

        float savedHit = PlayerPrefs.GetFloat("Hit", 1.0f);
        bgmSource_hit.volume = savedHit;
        if (volumeSlider_hit != null) volumeSlider_hit.value = savedHit;

        float savedRun = PlayerPrefs.GetFloat("Run", 1.0f);
        bgmSource_run.volume = savedRun;
        if (volumeSlider_run != null) volumeSlider_run.value = savedRun;

        // 슬라이더 이벤트 연결
        if (volumeSlider_bgm != null)
            volumeSlider_bgm.onValueChanged.AddListener(OnBgmVolumeChange);
        if (volumeSlider_hit != null)
            volumeSlider_hit.onValueChanged.AddListener(OnHitVolumeChange);
        if (volumeSlider_run != null)
            volumeSlider_run.onValueChanged.AddListener(OnRunVolumeChange);

        // BGM 재생 (중복 방지)
        if (!bgmSource_bgm.isPlaying)
            bgmSource_bgm.Play();
    }

    void OnBgmVolumeChange(float value)
    {
        bgmSource_bgm.volume = value;
        PlayerPrefs.SetFloat("BGM", value);
        PlayerPrefs.Save();
    }

    void OnHitVolumeChange(float value)
    {
        bgmSource_hit.volume = value;
        PlayerPrefs.SetFloat("Hit", value);
        PlayerPrefs.Save();
    }

    void OnRunVolumeChange(float value)
    {
        bgmSource_run.volume = value;
        PlayerPrefs.SetFloat("Run", value);
        PlayerPrefs.Save();
    }
}
