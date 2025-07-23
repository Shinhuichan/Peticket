using UnityEngine;

public class VRUIAligner : MonoBehaviour
{
    [Header("캔버스를 카메라 앞에 자동 배치함")]
    public float distanceFromCamera = 1.2f; // 카메라로부터 거리 
    public float scaleFactor = 0.003f;      // 캔버스 전체 크기 조절

    void Start()
    {
        if (Camera.main == null)
        {
            Debug.LogWarning("Main Camera가 존재하지 않습니다.");
            return;
        }

        Transform cam = Camera.main.transform;

        // 위치: 카메라 정면 앞쪽
        transform.position = cam.position + cam.forward * distanceFromCamera;

        // 회전: 카메라를 바라보게
        transform.rotation = Quaternion.LookRotation(transform.position - cam.position);

        // 크기: 지정한 스케일 적용
        transform.localScale = Vector3.one * scaleFactor;
    }
}
