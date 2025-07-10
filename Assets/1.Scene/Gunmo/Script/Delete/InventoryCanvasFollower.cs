using UnityEngine;

public class InventoryCanvasFollower : MonoBehaviour
{
    [SerializeField] private Transform targetCamera; // 플레이어 카메라 (VR 헤드)
    [SerializeField] private bool followXZOnly = true;
    [SerializeField] private Vector3 fixedOffset = new Vector3(0, 1.5f, 2f); // 플레이어 기준 위치

    void LateUpdate()
    {
        if (targetCamera == null) return;

        // 1. 위치 설정 (XZ만 따라가고 Y는 고정)
        Vector3 cameraPos = targetCamera.position;
        Vector3 followPos = cameraPos + targetCamera.forward * fixedOffset.z;
        if (followXZOnly)
        {
            followPos.y = transform.position.y; // Y는 기존 위치 유지
        }
        transform.position = followPos;

        // 2. 회전 설정 (Yaw만 따라감)
        Vector3 cameraEuler = targetCamera.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0, cameraEuler.y, 0); // Yaw만 반영
    }
}
