using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class UIFollowWithTrigger : MonoBehaviour
{
    [Header("Camera Follow")]
    public Transform targetCam;
    public float baseDistance = 2f;
    public float minSafeDistance = 0.8f;
    public float followSpeed = 5f;
    public Vector3 positionOffset = Vector3.zero;

    private float currentDistance;
    private float targetDistance;
    private bool isAvoid = false;

    void Start()
    {
        currentDistance = baseDistance;
        targetDistance = baseDistance;

        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        if (targetCam == null) return;

        // 현재 UI 위치 기준 장애물 검사
        bool isBlocked = CheckOverlapOffset(currentDistance);

        // 장애물이 앞에 있을 경우: 피함 상태로 진입
        if (isBlocked && !isAvoid)
        {
            isAvoid = true;
            targetDistance = minSafeDistance;
        }

        // 피하고 있는 중일 때만 base 위치 재검사
        if (isAvoid)
        {
            bool baseClear = !CheckOverlapOffset(baseDistance);
            if (baseClear)
            {
                isAvoid = false;
                targetDistance = baseDistance;
            }
        }

        // 거리 부드럽게 보간
        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * followSpeed);

        // 위치 업데이트
        Vector3 dir = targetCam.forward;
        Vector3 targetPos = targetCam.position + dir * currentDistance + positionOffset;
        transform.position = targetPos;

        // 회전 따라가기 (Z축 제외)
        Vector3 euler = targetCam.rotation.eulerAngles;
        euler.z = 0f;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), Time.deltaTime * followSpeed);
    }

    private bool CheckOverlapOffset(float distance)
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 checkPos = targetCam.position + targetCam.forward * distance + positionOffset;
        Vector3 worldCenter = checkPos + transform.rotation * box.center;
        Vector3 worldSize = Vector3.Scale(box.size, transform.lossyScale) * 0.5f; // 반 크기 (OverlapBox 기준)

        Collider[] hits = Physics.OverlapBox(worldCenter, worldSize, transform.rotation, LayerMask.GetMask("Wall"));
        return hits.Length > 0;
    }
}
