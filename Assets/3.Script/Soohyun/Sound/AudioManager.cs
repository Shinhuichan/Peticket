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

    [Header("Button")]
    public Button BackButton;
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


        // 슬라이더 이벤트 연결
        if (volumeSlider_bgm != null)
            volumeSlider_bgm.onValueChanged.AddListener(OnBgmVolumeChange);

        // BGM 재생 (중복 방지)
        if (!bgmSource_bgm.isPlaying)
            bgmSource_bgm.Play();

        BackButton.onClick.AddListener(Back);
    }

    void OnBgmVolumeChange(float value)
    {
        bgmSource_bgm.volume = value;
        PlayerPrefs.SetFloat("BGM", value);
        PlayerPrefs.Save();
    }

    public void Back()
    {

    }
}
