using System.Collections.Generic;
using UnityEngine;

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

        if (uiToDisplay != null) uiToDisplay.SetActive(false);
        else Debug.LogWarning("ItemUseZoneManager: 'UI To Display' GameObject가 할당되지 않았습니다.");
    }

    private void Update()
    {
        if (player == null || uiToDisplay == null) return;

        Vector3 playerPosition = player.transform.position;
        bool isPlayerNowInZone = IsInsideAnyZone(playerPosition);

        if (isPlayerNowInZone != isPlayerCurrentlyInZone)
        {
            uiToDisplay.SetActive(isPlayerNowInZone);
            isPlayerCurrentlyInZone = isPlayerNowInZone;

            Debug.Log($"ItemUseZoneManager: 플레이어가 영역 {(isPlayerNowInZone ? "안" : "밖")}에 있습니다.");
        }
    }

    public bool IsInsideAnyZone(Vector3 playerWorldPosition)
    {
        foreach (var zone in zones)
        {
            if (IsPositionInZone(playerWorldPosition, zone))
                return true;
        }
        return false;
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
