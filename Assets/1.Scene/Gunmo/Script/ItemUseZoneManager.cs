using System.Collections.Generic;
using UnityEngine;
// using CustomInspector; // 사용하지 않는다면 제거

public class ItemUseZoneManager : MonoBehaviour
{
    public static ItemUseZoneManager Instance { get; private set; }

    [Header("플레이어 오브젝트")]
    [SerializeField] public GameObject player;

    [Header("영역 진입 시 표시할 UI")]
    [SerializeField] private GameObject uiToDisplay;

    [System.Serializable]
    public class UseZone
    {
        public string zoneName = "Zone";
        public Rect rectXZ = new Rect(-5f, -5f, 10f, 10f); // XZ 평면 기준
        public float minY = 0f;
        public float maxY = 5f;
        public Color gizmoColor = Color.green;
    }

    [Header("아이템 사용 가능한 영역 목록")]
    public List<UseZone> zones = new List<UseZone>();

    private bool isPlayerCurrentlyInZone = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (uiToDisplay != null) uiToDisplay.SetActive(false);
        else  Debug.LogWarning("ItemUseZoneManager: 'UI To Display' GameObject가 할당되지 않았습니다. UI 표시 기능이 작동하지 않습니다.");
    }

    private void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("ItemUseZoneManager: 'Player' GameObject가 할당되지 않았습니다. 영역 체크가 불가능합니다.");
            return;
        }
        if (uiToDisplay == null) return;

        Vector3 playerPosition = player.transform.position;
        bool isPlayerNowInZone = IsInsideAnyZone(playerPosition);

        // 이전과 현재 상태가 다를 때만 로그 출력 및 UI 변경
        if (isPlayerNowInZone != isPlayerCurrentlyInZone)
        {
            uiToDisplay.SetActive(isPlayerNowInZone);
            isPlayerCurrentlyInZone = isPlayerNowInZone;

            if (isPlayerNowInZone)
            {
                Debug.Log($"ItemUseZoneManager: 플레이어가 지정된 영역 안으로 진입했습니다. UI 활성화.");
                Debug.Log($"플레이어 월드 위치: X={playerPosition.x:F2}, Y={playerPosition.y:F2}, Z={playerPosition.z:F2}");
            }
            else
            {
                Debug.Log($"ItemUseZoneManager: 플레이어가 지정된 영역 밖으로 나갔습니다. UI 비활성화.");
                Debug.Log($"플레이어 월드 위치: X={playerPosition.x:F2}, Y={playerPosition.y:F2}, Z={playerPosition.z:F2}");
            }
        }
    }

    public bool IsInsideAnyZone(Vector3 playerWorldPosition) // 매개변수 이름을 명확히 변경
    {
        foreach (var zone in zones)
        {
            // 굳건희! 여기서 Zone의 월드 범위 값들을 직접 출력해봐!
            float zoneXMin = zone.rectXZ.xMin;
            float zoneXMax = zone.rectXZ.xMax;
            float zoneZMin = zone.rectXZ.yMin; // Rect의 yMin은 3D의 ZMin에 해당
            float zoneZMax = zone.rectXZ.yMax; // Rect의 yMax는 3D의 ZMax에 해당

            // Y축(높이) 범위 먼저 확인
            bool isYInside = (playerWorldPosition.y >= zone.minY && playerWorldPosition.y <= zone.maxY);
            
            // XZ 평면(Rect) 범위 확인
            bool isXZInside = (playerWorldPosition.x >= zoneXMin && playerWorldPosition.x <= zoneXMax &&
                               playerWorldPosition.z >= zoneZMin && playerWorldPosition.z <= zoneZMax);

            if (isYInside && isXZInside) // isXZInsideUsingContains로 바꿔도 결과는 같아야 함
            {
                Debug.Log($"ItemUseZoneManager: 플레이어가 '{zone.zoneName}' 영역 안에 있습니다. 💖");
                return true; // 어떤 한 Zone이라도 만족하면 true 반환
            }
        }
        return false; // 어떤 Zone에도 속하지 않으면 false 반환
    }

    private void OnDrawGizmosSelected()
    {
        if (zones == null) return;

        foreach (var zone in zones)
        {
            Gizmos.color = zone.gizmoColor;

            // Gizmo의 중심점 계산
            Vector3 center = new Vector3(
                zone.rectXZ.center.x,
                (zone.minY + zone.maxY) / 2f,
                zone.rectXZ.center.y
            );

            // Gizmo의 크기 계산
            Vector3 size = new Vector3(
                zone.rectXZ.width,
                zone.maxY - zone.minY,
                zone.rectXZ.height
            );

            Gizmos.DrawWireCube(center, size);

#if UNITY_EDITOR
            UnityEditor.Handles.Label(center + Vector3.up * (size.y / 2f + 0.1f), zone.zoneName);
#endif
        }
    }
}