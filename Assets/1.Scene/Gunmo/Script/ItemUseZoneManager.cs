using System.Collections.Generic;
using UnityEngine;

public class ItemUseZoneManager : MonoBehaviour
{
    public static ItemUseZoneManager Instance { get; private set; }

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool IsInsideAnyZone(Vector3 position)
    {
        foreach (var zone in zones)
        {
            if (position.y >= zone.minY && position.y <= zone.maxY &&
                zone.rectXZ.Contains(new Vector2(position.x, position.z)))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()
    {
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
            UnityEditor.Handles.Label(center + Vector3.up * 0.5f, zone.zoneName);
#endif
        }
    }
}
