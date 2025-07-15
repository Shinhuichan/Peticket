using UnityEngine;
using UnityEngine.InputSystem;

public class InputSetting : MonoBehaviour
{
    public GameObject inventoryPanel;
    public InputActionReference openInventoryAction;

    void Start()
    {
        openInventoryAction.action.Enable();
        openInventoryAction.action.performed += ToggleInventory;
        InputSystem.onDeviceChange += OnDeviceChange;
    }
    void OnDestroy()
    {
        openInventoryAction.action.Disable();
        openInventoryAction.action.performed -= ToggleInventory;
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
                openInventoryAction.action.Disable();
                openInventoryAction.action.performed -= ToggleInventory;
                break;
            case InputDeviceChange.Reconnected:
                openInventoryAction.action.Enable();
                openInventoryAction.action.performed += ToggleInventory;
                break;
        }
    }
}
