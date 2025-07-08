using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemPreviewRotator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float rotationSpeed = 50f;
    public float returnSpeed = 5f;

    private bool isHovered = false;
    private Quaternion initialRotation;
    private bool isReturning = false;

    void Start()
    {
        initialRotation = transform.rotation;
    }

    void Update()
    {
        if (isHovered)
        {
            // 회전 중
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
            isReturning = false;
        }
        else if (!isHovered && !QuaternionApproximately(transform.rotation, initialRotation))
        {
            // 돌아가는 중
            transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, Time.deltaTime * returnSpeed);
            isReturning = true;
        }
        else if (isReturning)
        {
            // 돌아가는 중이었는데 거의 도착했으면 고정
            transform.rotation = initialRotation;
            isReturning = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
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