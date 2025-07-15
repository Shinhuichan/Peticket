using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetting : MonoBehaviour
{
    public GameObject inventoryPanel;
    public InputActionReference openMenuAction;

    void Start()
    {
        openMenuAction.action.Enable();
        openMenuAction.action.performed += ToggleInventory;
        InputSystem.onDeviceChange += OnDeviceChange;
    }
    void OnDestroy()
    {
        openMenuAction.action.Disable();
        openMenuAction.action.performed -= ToggleInventory;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    private void ToggleInventory(InputAction.CallbackContext context)
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
    
    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
                openMenuAction.action.Disable();
                openMenuAction.action.performed -= ToggleInventory;
                break;
            case InputDeviceChange.Reconnected:
                openMenuAction.action.Enable();
                openMenuAction.action.performed += ToggleInventory;
                break;
        }
    }
}
