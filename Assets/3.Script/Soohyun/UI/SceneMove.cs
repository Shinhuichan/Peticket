using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
public class SceneMove : MonoBehaviour
{
    [SerializeField] private InputActionReference aButton;

    private void OnEnable()
    {
        aButton.action.Enable();
        aButton.action.performed += OnAButtonPressed;
    }

    private void OnDisable()
    {
        aButton.action.Disable();
        aButton.action.performed -= OnAButtonPressed;
    }

    public void OnAButtonPressed(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("0. Tutoriual");
    }
}
