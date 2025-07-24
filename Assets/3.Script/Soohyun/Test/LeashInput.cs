using UnityEngine;
using UnityEngine.InputSystem;

public class LeashInput : MonoBehaviour
{
    public InputActionReference toggleLeashAction; // �ν����Ϳ��� A ��ư �� ����
    public LeashControllerManager leashManager;

    void OnEnable()
    {
        toggleLeashAction.action.performed += OnLeashToggle;
        toggleLeashAction.action.Enable();
    }

    void OnDisable()
    {
        toggleLeashAction.action.performed -= OnLeashToggle;
        toggleLeashAction.action.Disable();
    }

    private void OnLeashToggle(InputAction.CallbackContext context)
    {
        leashManager.ToggleLeash();
    }
}
