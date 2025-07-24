using UnityEngine;
using UnityEngine.InputSystem;

public class LeashInput : MonoBehaviour
{
    public InputActionReference toggleLeashAction; // 인스펙터에서 A 버튼 등 연결
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
