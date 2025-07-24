using System.Collections;
using System.Linq;
using UnityEngine;

public abstract class BaseNarrationPlayer : MonoBehaviour
{
    [Header("오디오 소스")]
    public AudioSource audioSource;

    private AudioClip[] narrationClips;
    private Coroutine playRoutine;

    protected abstract string FolderName { get; }

    void Awake()
    {
        LoadNarrationClips();
    }

    private void LoadNarrationClips()
    {
        narrationClips = Resources.LoadAll<AudioClip>($"voice/{FolderName}")
                                  .OrderBy(c => c.name)
                                  .ToArray();

        if (narrationClips.Length == 0)
            Debug.LogWarning($"[{FolderName}] 나레이션 클립이 없습니다.");
    }

    public void PlayNarration(int index)
    {
        if (index < 0 || index >= narrationClips.Length)
        {
            Debug.LogWarning($"[{FolderName}] 잘못된 인덱스: {index}");
            return;
        }

        if (playRoutine != null)
            StopCoroutine(playRoutine);

        playRoutine = StartCoroutine(PlaySingleClip(index));
    }

    private IEnumerator PlaySingleClip(int index)
    {
        audioSource.clip = narrationClips[index];
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length + 0.1f);
        playRoutine = null;
    }

    public void StopNarration()
    {
        if (playRoutine != null)
            StopCoroutine(playRoutine);

        audioSource.Stop();
        playRoutine = null;
    }
}
