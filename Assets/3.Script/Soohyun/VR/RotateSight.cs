using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RotateSight : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference rightThumbstickAction;

    [Header("XR Origin")]
    public Transform XROriginTransform;

    [Header("Settings")]
    public float turnSpeed = 90f; // 초당 회전 속도 (degrees per second)
    public float deadZone = 0.1f; // 조이스틱 미세한 흔들림 무시

    private void Update()
    {
        Vector2 input = rightThumbstickAction.action.ReadValue<Vector2>();

        // 조이스틱 X축만 사용 (좌우 회전)
        float turnInput = input.x;

        // Dead Zone 처리
        if (Mathf.Abs(turnInput) < deadZone)
            return;

        float rotationAmount = turnInput * turnSpeed * Time.deltaTime;

        XROriginTransform.Rotate(Vector3.up, rotationAmount);
    }
}
