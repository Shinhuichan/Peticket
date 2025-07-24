using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ItemUseZoneManager : MonoBehaviour
{
    public static ItemUseZoneManager Instance { get; private set; }

    [Header("플레이어 오브젝트")]
    [SerializeField] public GameObject player;

    [Header("영역 진입 시 표시할 TextUI")]
    [SerializeField] private TextMeshProUGUI areaUI;

    [System.Serializable]
    public class UseZone
    {
        public string zoneName = "Zone";
        public Rect rectXZ = new Rect(-5f, -5f, 10f, 10f);
        public float minY = 0f;
        public float maxY = 5f;
        public Color gizmoColor = Color.green;

        [Header("허용된 아이템 프리팹")]
        public List<GameObject> allowedPrefabs = new List<GameObject>();
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

        if (areaUI != null) areaUI.transform.parent.gameObject.SetActive(false);
        else Debug.LogWarning("ItemUseZoneManager: 'UI To Display' GameObject가 할당되지 않았습니다.");
    }

    private void Update()
    {
        if (player == null || areaUI == null) return;

        Vector3 playerPosition = player.transform.position;
        bool isPlayerNowInZone = IsInsideAnyZone(playerPosition);

        if (isPlayerNowInZone != isPlayerCurrentlyInZone)
        {
            
            isPlayerCurrentlyInZone = isPlayerNowInZone;

            Debug.Log($"ItemUseZoneManager: 플레이어가 영역 {(isPlayerNowInZone ? "안" : "밖")}에 있습니다.");
        }
    }

    public bool IsInsideAnyZone(Vector3 playerWorldPosition) // 매개변수 이름을 명확히 변경
    {
        foreach (var zone in zones)
        {
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
                Debug.Log($"ItemUseZoneManager: 플레이어가 '{zone.zoneName}' 영역 안에 있습니다.");
                areaUI.transform.parent.gameObject.SetActive(true);

                switch (zone.zoneName)
                {
                    case "Food":
                        areaUI.text = "간식 주기";
                        break;
                    case "Ball":
                        areaUI.text = "공놀이";
                        break;
                    case "Bowl":
                        areaUI.text = "간식 주기";
                        break;
                    case "Shovel":
                        areaUI.text = "배변 청소";
                        break;
                }
                return true; // 어떤 한 Zone이라도 만족하면 true 반환
            }
        }
        areaUI.transform.parent.gameObject.SetActive(false);
        Debug.Log($"ItemUseZoneManager: 플레이어가 영역 밖에 있습니다.");
        return false; // 어떤 Zone에도 속하지 않으면 false 반환
    }


    public bool IsPrefabAllowedInZone(Vector3 position, GameObject itemInstance)
    {
        if (itemInstance == null)
        {
            Debug.LogWarning("[ItemUseZoneManager] itemInstance가 null입니다.");
            return false;
        }

        foreach (var zone in zones)
        {
            if (IsPositionInZone(position, zone))
            {
                string itemName = itemInstance.name.Replace("(Clone)", "").Trim();

                foreach (var allowed in zone.allowedPrefabs)
                {
                    if (allowed != null && allowed.name == itemName)
                    {
                        Debug.Log($"✅ '{itemName}'는 zone '{zone.zoneName}'에서 허용됨");
                        return true;
                    }
                }

                Debug.LogWarning($"❌ '{itemName}'는 zone '{zone.zoneName}'에서 허용되지 않음");
                return false;
            }
        }

        Debug.LogWarning($"❌ 현재 위치는 어떤 zone에도 속하지 않음");
        return false;
    }

    private bool IsPositionInZone(Vector3 position, UseZone zone)
    {
        bool isYInside = (position.y >= zone.minY && position.y <= zone.maxY);
        bool isXZInside = (position.x >= zone.rectXZ.xMin && position.x <= zone.rectXZ.xMax &&
                           position.z >= zone.rectXZ.yMin && position.z <= zone.rectXZ.yMax);
        return isYInside && isXZInside;
    }

    private void OnDrawGizmosSelected()
    {
        if (zones == null) return;

        foreach (var zone in zones)
        {
            Gizmos.color = zone.gizmoColor;

            Vector3 center = new Vector3(
                zone.rectXZ.center.x,
                (zone.minY + zone.maxY) / 2f,
                zone.rectXZ.center.y
            );

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
