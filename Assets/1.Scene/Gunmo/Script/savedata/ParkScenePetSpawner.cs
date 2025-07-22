using UnityEngine;

public class ParkScenePetSpawner : MonoBehaviour
{
    [Header("펫 프리팹들")]
    public GameObject smallDogPrefab;
    public GameObject middleDogPrefab;
    public GameObject largeDogPrefab;

    [Header("생성 위치")]
    public Transform spawnPoint;

    private void Start()
    {
        string selectedPetId = GameSaveManager.Instance?.currentSaveData?.selectedPetId;

        if (string.IsNullOrEmpty(selectedPetId))
        {
            Debug.LogWarning("❌ 선택된 펫이 없습니다.");
            return;
        }

        GameObject prefabToSpawn = null;

        switch (selectedPetId)
        {
            case "small":
                prefabToSpawn = smallDogPrefab;
                break;
            case "middle":
                prefabToSpawn = middleDogPrefab;
                break;
            case "large":
                prefabToSpawn = largeDogPrefab;
                break;
        }

        if (prefabToSpawn != null && spawnPoint != null)
        {
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"🏞 공원에서 펫 생성 완료: {selectedPetId}");
        }
        else
        {
            Debug.LogError("❌ 펫 생성 실패 (프리팹 또는 위치 누락)");
        }
    }
}
