using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HelpUISetting : MonoBehaviour
{
    [Header("UI Sprite Setting")]
    public Image ImageSetting;
    public Sprite[] images;

    [Header("UI Text Setting")]
    public TextMeshProUGUI textSetting;

    private string[] texts; // 각 이미지에 대응하는 텍스트

    [Header("Input Actions")]
    public InputActionReference nextHelpAction;  // 오른손 트리거
    public InputActionReference prevHelpAction;  // 왼손 트리거

    [Header("Voice Narration")]
    public AudioSource audioSource;
    public AudioClip[] voiceClips;

    private int currentIndex = 0;

    private void OnEnable()
    {
        nextHelpAction.action.performed += OnNextHelp;
        prevHelpAction.action.performed += OnPrevHelp;
        nextHelpAction.action.Enable();
        prevHelpAction.action.Enable();
    }

    private void OnDisable()
    {
        nextHelpAction.action.performed -= OnNextHelp;
        prevHelpAction.action.performed -= OnPrevHelp;
        nextHelpAction.action.Disable();
        prevHelpAction.action.Disable();
    }

    private void Start()
    {
        texts = new string[]
           {
            // Snack
            "먹이\n강아지가 배고플 때\n주는 맛있는 간식이에요!\n\n어떻게 주나요?\n1. 먹이를 손(컨트롤러)으로 들어요.\n2. 밥그릇 위에 올리고 버튼을 꾹 눌러요!",
            
            // Shovel
            "삽\n강아지가 배변을 하면\n이 삽으로 깨끗하게 정리할 수 있어요!\n\n어떻게 치울까요?\n1. 삽을 손(컨트롤러)으로 들어요.\n2. 배변물 위에 살짝 갖다 대면,\n자동으로 말끔히 치워져요!",

            // Muzzle
            "입마개\n강아지가 놀라거나 흥분했을 때,\n다른 사람이나 동물을 보호하기 위해\n이 입마개를 착용해요!\n\n어떻게 착용하나요?\n1. 입마개를 손(컨트롤러)으로 들어요.\n2. 강아지 얼굴에 살짝 갖다 대면,\n자동으로 착용돼요!",
            // ball
            "공 \n 강아지와 놀아줄 때 사용할 수 있어요! \n 1. 오른손으로 아이템을 들어요. \n 2. 손으로 들고 앞으로 던지는 느낌을 주면서 스윙을 해요! \n 3.스윙을 하면서 버튼을 떼요. \n 4. 강아지가 가져옵니다!",
            //leash            
            "목줄 \n 강아지와 산책갈 때 필수 아이템입니다! \n 1.오른손으로 아이템을 들어요. \n 2.강아지 목에 가져다 대면 자동으로 착용이 됩니다!",
            //muzzle
            "입마개 \n 강아지와 산책할때 필수 아이템입니다! \n 1.오른손으로 아이템을 들어요. \n 2.강아지 입에 가져다 대면 자동으로 입에 착용이 됩니다!",
            //bowl
            "밥그릇 \n 강아지의 사료를 담는 그릇입니다! \n 1.오른손으로 아이템을 들어요. \n 2. 원하는 위치에 가져다 두어요."
           };
        UpdateHelpUI();
    }

    private void OnNextHelp(InputAction.CallbackContext context)
    {
        Debug.Log("Next");
        NextButtonClick();
    }

    private void OnPrevHelp(InputAction.CallbackContext context)
    {
        Debug.Log("Prev");
        PrevButtonClick();
    }

    public void PrevButtonClick()
    {
        currentIndex = Mathf.Max(0, currentIndex - 1);
        UpdateHelpUI();
    }

    public void NextButtonClick()
    {
        currentIndex = Mathf.Min(images.Length - 1, currentIndex + 1);
        UpdateHelpUI();
    }

    private void UpdateHelpUI()
    {
        if (ImageSetting != null && images.Length > currentIndex)
            ImageSetting.sprite = images[currentIndex];

        if (textSetting != null && texts.Length > currentIndex)
            textSetting.text = texts[currentIndex];

        // 🎯 특정 씬일 때만 재생
        if (IsVoiceNarrationAllowed())
        {
            if (audioSource != null && voiceClips.Length > currentIndex && voiceClips[currentIndex] != null)
            {
                audioSource.Stop();
                audioSource.clip = voiceClips[currentIndex];
                audioSource.Play();
            }
        }
    }

    private bool IsVoiceNarrationAllowed()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        return currentSceneName == "0. Tutoriual 1";
    }
}
