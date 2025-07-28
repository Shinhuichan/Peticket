using System.Collections;
using UnityEngine;

public class VoiceInteraction : MonoBehaviour
{
    [Header("Narration Clip 데이터")]
    public AudioInteraction audioData;            // 오디오 데이터 모음
    public AudioSource narrationSource;           // 재생 오디오소스

    private Coroutine currentRoutine = null;

    /// <summary>
    /// 외부에서 호출하여 index에 맞는 모든 클립 순차 재생
    /// </summary>
    public void PlayNarrationByIndex(int index)
    {
        // 이미 재생 중인 루틴 있으면 정지
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        // 새 루틴 시작
        currentRoutine = StartCoroutine(PlaySequentialNarration(index));
    }

    private IEnumerator PlaySequentialNarration(int index)
    {
        AudioClip[] clips = GetClipsByIndex(index);

        if (clips == null || clips.Length == 0)
        {
            Debug.LogWarning($"[VoiceInteraction] ❌ Index {index}에 해당하는 클립이 없음.");
            yield break;
        }

        foreach (AudioClip clip in clips)
        {
            if (clip == null) continue;

            narrationSource.clip = clip;
            narrationSource.Play();
            Debug.Log($"[VoiceInteraction] ▶ 클립 재생: {clip.name}");

            yield return new WaitForSeconds(clip.length + 0.1f); // 약간의 여유 시간
        }

        currentRoutine = null; // 완료 후 초기화
    }

    private AudioClip[] GetClipsByIndex(int index)
    {
        switch (index)
        {
            case 0: return audioData.Snack_N;
            case 1: return audioData.Shovel_N;
            case 2: return audioData.Muzzle_N;
            case 3: return audioData.Ball_N;
            case 4: return audioData.Leash_N;
            case 5: return audioData.Bowl_N;
            default: return null;
        }
    }
}
