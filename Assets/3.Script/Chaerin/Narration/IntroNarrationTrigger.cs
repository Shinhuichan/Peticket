using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRSimpleInteractable))]
public class IntroNarrationTrigger : MonoBehaviour
{
    [Header("연결된 나레이션")]
    public VoiceInteraction voiceInteraction;

    private XRSimpleInteractable interactable;
    private bool hasPlayed = false;

    private void OnEnable()
    {
        interactable = GetComponent<XRSimpleInteractable>();
         Debug.Log("[IntroNarrationTrigger] OnEnable 실행됨");
        interactable.selectEntered.AddListener(OnSelect);
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelect);
        }
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log(" XR 상호작용으로 게시판 선택됨!");
        PlayNarrationFromTrigger();
    }

    public void PlayNarrationFromTrigger()
    {
        if (hasPlayed) return;

        if (voiceInteraction != null)
        {
            Debug.Log(" 나레이션 시작됨");
            voiceInteraction.PlayNarrationByIndex(0); // 첫 번째 나레이션 재생
            hasPlayed = true;
        }
        else
        {
            Debug.LogWarning("VoiceInteraction이 연결되지 않았습니다!");
        }
    }
}
