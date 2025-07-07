using UnityEngine;

public class HandController : MonoBehaviour
{
    private Vector3 previousPosition;
    public Vector3 CurrentVelocity { get; private set; }

    void Start()
    {
        previousPosition = transform.position;
    }

    void FixedUpdate()
    {
        // 현재 프레임의 속도 계산
        CurrentVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;
    }
}