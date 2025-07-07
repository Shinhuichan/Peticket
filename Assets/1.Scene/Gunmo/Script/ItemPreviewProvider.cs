using UnityEngine;

public class ItemPreviewProvider : MonoBehaviour
{
    public GameObject previewModelPrefab; // 슬롯에서 사용할 미리보기용 프리팹
    public float previewScale = 50f; // 👈 슬롯에서 보여질 크기
    public Vector3 previewRotationEuler = new Vector3(25f, -45f, 0f); // 아이솔레이션 각도
    public Vector3 previewOffset = new Vector3(0f, 0f, -0.1f); // 슬롯 안으로 넣기
}
