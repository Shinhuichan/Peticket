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

    private string[] texts; // �� �̹����� �����ϴ� �ؽ�Ʈ

    [Header("Input Actions")]
    public InputActionReference nextHelpAction;  // ������ Ʈ����
    public InputActionReference prevHelpAction;  // �޼� Ʈ����

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
            "����\n�������� ����� ��\n�ִ� ���ִ� �����̿���!\n\n��� �ֳ���?\n1. ���̸� ��(��Ʈ�ѷ�)���� ����.\n2. ��׸� ���� �ø��� ��ư�� �� ������!",
            
            // Shovel
            "��\n�������� �躯�� �ϸ�\n�� ������ �����ϰ� ������ �� �־��!\n\n��� ġ����?\n1. ���� ��(��Ʈ�ѷ�)���� ����.\n2. �躯�� ���� ��¦ ���� ���,\n�ڵ����� ������ ġ������!",

            // Muzzle
            "�Ը���\n�������� ���ų� ������� ��,\n�ٸ� ����̳� ������ ��ȣ�ϱ� ����\n�� �Ը����� �����ؿ�!\n\n��� �����ϳ���?\n1. �Ը����� ��(��Ʈ�ѷ�)���� ����.\n2. ������ �󱼿� ��¦ ���� ���,\n�ڵ����� ����ſ�!"
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
