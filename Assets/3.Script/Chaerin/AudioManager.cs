﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class BgmData
{
    public string sceneName;
    public AudioClip bgmClip;
}

[System.Serializable]
public class SfxData
{
    public string key;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("BGM 설정")]
    public AudioSource bgmSource;
    public List<BgmData> bgmList = new List<BgmData>();

    [Header("SFX 설정")]
    public AudioSource sfxSourcePrefab;
    public List<SfxData> sfxList = new List<SfxData>();
    private List<AudioSource> sfxSources = new List<AudioSource>();

    [Range(0f, 1f)] public float BgmVolume = 0.5f;
    [Range(0f, 1f)] public float SfxVolume = 0.5f;

    private string currentSceneName = "";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitOrLoadVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ApplyVolumes();
        PlaySceneBGM(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode == LoadSceneMode.Single)
        {
            PlaySceneBGM(scene.name);
        }
    }

    private void InitOrLoadVolumes()
    {
        if (!PlayerPrefs.HasKey("HasInitializedAudio"))
        {
            BgmVolume = 0.5f;
            SfxVolume = 0.5f;
            SaveVolumes();
            PlayerPrefs.SetInt("HasInitializedAudio", 1);
            PlayerPrefs.Save();
        }
        else
        {
            BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.5f);
            SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.5f);
        }
    }

    public void PlaySceneBGM(string sceneName)
    {
        var data = bgmList.Find(b => b.sceneName == sceneName);

        if (data == null || data.bgmClip == null)
        {
            Debug.LogWarning($"[AudioManager] {sceneName} 씬에 해당하는 BGM이 없습니다.");
            return;
        }

        if (bgmSource.clip == data.bgmClip && bgmSource.isPlaying)
        {
            Debug.Log($"[AudioManager] 동일한 BGM이 이미 재생 중입니다: {sceneName}");
            return;
        }

        bgmSource.clip = data.bgmClip;
        bgmSource.loop = true;
        bgmSource.volume = BgmVolume;
        bgmSource.Play();
        currentSceneName = sceneName;

        Debug.Log($"[AudioManager] BGM 재생 시작: {sceneName}");
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSourcePrefab == null) return;

        AudioSource newSFX = Instantiate(sfxSourcePrefab, transform);
        newSFX.clip = clip;
        newSFX.volume = SfxVolume;
        newSFX.Play();
        Destroy(newSFX.gameObject, clip.length);

        sfxSources.Add(newSFX);
    }

    public void PlaySFXByKey(string key)
    {
        var data = sfxList.Find(s => s.key == key);
        if (data == null || data.clip == null)
        {
            Debug.LogWarning($"[AudioManager] '{key}' 키에 해당하는 효과음이 없습니다.");
            return;
        }

        PlaySFX(data.clip);
    }

    public void PlayButtonClick()
    {
        PlaySFXByKey("ButtonClick");
    }

    public void SaveVolumes()
    {
        PlayerPrefs.SetFloat("BgmVolume", BgmVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
        PlayerPrefs.Save();
        ApplyVolumes();
    }

    public void ApplyVolumes()
    {
        if (bgmSource != null)
            bgmSource.volume = BgmVolume;

        foreach (var src in sfxSources)
        {
            if (src != null)
                src.volume = SfxVolume;
        }
    }
}
