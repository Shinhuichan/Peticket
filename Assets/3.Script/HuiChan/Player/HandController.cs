using UnityEngine;
using UnityEngine.UI;

public class HandController : MonoBehaviour
{
    private Vector3 previousPosition;
    public Vector3 currentVelocity { get; private set; }

    void Start()
    {
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        // 현재 프레임의 속도 계산
        currentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }
}