using CustomInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class InputSetting : MonoBehaviour
{
    [Title("Input Setting", underlined: true, fontSize = 15, alignment = TextAlignment.Center)]
    
    [Header("Inventory Setting")]
    public GameObject inventoryPanel;
    public InputActionReference toggleInventoryAction;

    [Header("Dash Setting")]
    [SerializeField] float dashIncrease = 1.5f;
    [SerializeField, ReadOnly] float baseMoveSpeed;
    public InputActionReference dashAction;
    public DynamicMoveProvider moveProvider;

    void Start()
    {
        moveProvider = FindFirstObjectByType<DynamicMoveProvider>();
        if (moveProvider == null)
        {
            Debug.LogWarning($"Input Setting | moveProvider가 Null입니다.");
            return;
        }
        baseMoveSpeed = moveProvider.moveSpeed;
        toggleInventoryAction.action.Enable();
        dashAction.action.Enable();

        toggleInventoryAction.action.performed += ToggleInventory;
        dashAction.action.performed += Dash;
        dashAction.action.canceled += DashStop;
        InputSystem.onDeviceChange += OnDeviceChange;
    }
    void OnDestroy()
    {
        toggleInventoryAction.action.Disable();
        dashAction.action.Disable();

        toggleInventoryAction.action.performed -= ToggleInventory;
        dashAction.action.performed -= Dash;
        dashAction.action.canceled -= DashStop;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }

    #region Dash
    private void Dash(InputAction.CallbackContext context)
    {
        // Dash Start 기능
        moveProvider.moveSpeed = baseMoveSpeed * dashIncrease;
    }
    private void DashStop(InputAction.CallbackContext context)
    {
        // Dash Stop 기능
        moveProvider.moveSpeed = baseMoveSpeed;
    }
    #endregion

    #region Inventory
    private void ToggleInventory(InputAction.CallbackContext context)
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }
    #endregion



    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        switch (change)
        {
            case InputDeviceChange.Disconnected:
                toggleInventoryAction.action.Disable();
                toggleInventoryAction.action.performed -= ToggleInventory;
                break;
            case InputDeviceChange.Reconnected:
                toggleInventoryAction.action.Enable();
                toggleInventoryAction.action.performed += ToggleInventory;
                break;
        }
    }
}
