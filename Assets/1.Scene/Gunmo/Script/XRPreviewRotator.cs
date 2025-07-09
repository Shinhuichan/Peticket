using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRPreviewRotator : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public float returnSpeed = 5f;

    private bool isHovered = false;
    private Quaternion initialRotation;
    private bool isReturning = false;

    private void Start()
    {
        initialRotation = transform.rotation;

        // ⚠️ XR Interaction Events는 Interactable에 존재해야 함
        var interactable = GetComponent<XRBaseInteractable>();
        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEnter);
            interactable.hoverExited.AddListener(OnHoverExit);
        }
    }

    private void Update()
    {
        if (isHovered)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            isReturning = false;
        }
        else if (!QuaternionApproximately(transform.rotation, initialRotation))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * returnSpeed);
            isReturning = true;
        }
        else if (isReturning)
        {
            transform.rotation = initialRotation;
            isReturning = false;
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        isHovered = true;
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        isHovered = false;
    }

    private bool QuaternionApproximately(Quaternion a, Quaternion b, float tolerance = 0.01f)
    {
        return Quaternion.Angle(a, b) < tolerance;
    }

    public IEnumerator Shake(float intensity = 15f, float duration = 0.4f)
    {
        float elapsed = 0f;
        float speed = 40f;
        Quaternion originalRotation = transform.localRotation;

        while (elapsed < duration)
        {
            float angle = Mathf.Sin(elapsed * speed) * intensity;
            transform.localRotation = originalRotation * Quaternion.Euler(0f, angle, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localRotation = originalRotation;
    }
}
