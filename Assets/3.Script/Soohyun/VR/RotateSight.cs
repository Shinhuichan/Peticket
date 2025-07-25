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
    public float turnSpeed = 90f; // �ʴ� ȸ�� �ӵ� (degrees per second)
    public float deadZone = 0.1f; // ���̽�ƽ �̼��� ��鸲 ����

    private void Update()
    {
        Vector2 input = rightThumbstickAction.action.ReadValue<Vector2>();

        // ���̽�ƽ X�ุ ��� (�¿� ȸ��)
        float turnInput = input.x;

        // Dead Zone ó��
        if (Mathf.Abs(turnInput) < deadZone)
            return;

        float rotationAmount = turnInput * turnSpeed * Time.deltaTime;

        XROriginTransform.Rotate(Vector3.up, rotationAmount);
    }
}
