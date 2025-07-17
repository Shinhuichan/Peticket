using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

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
            "입마개\n강아지가 놀라거나 흥분했을 때,\n다른 사람이나 동물을 보호하기 위해\n이 입마개를 착용해요!\n\n어떻게 착용하나요?\n1. 입마개를 손(컨트롤러)으로 들어요.\n2. 강아지 얼굴에 살짝 갖다 대면,\n자동으로 착용돼요!"
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
    }
}
