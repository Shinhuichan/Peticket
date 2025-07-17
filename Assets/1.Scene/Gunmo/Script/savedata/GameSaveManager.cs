using UnityEngine;
using System.IO;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }

    public GameSaveData currentSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            // 선택된 펫 로드: 선택 씬에서 petId 설정 후 LoadGame(petId) 호출 필요
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private string GetFilePathForPet(string petId)
    {
        return Path.Combine(Application.dataPath, $"SaveData/pet_{petId}_save.json");
    }

    public void SaveGame(Vector3 playerPosition, string petId)
    {
        currentSaveData.playerPosX = playerPosition.x;
        currentSaveData.playerPosY = playerPosition.y;
        currentSaveData.playerPosZ = playerPosition.z;
        currentSaveData.petData = FindObjectOfType<PetAffinityManager>()?.GetCurrentData();

        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(GetFilePathForPet(petId), json);
        Debug.Log($"💾 저장 완료: {petId}");
    }

    public void LoadGame(string petId)
    {
        string path = GetFilePathForPet(petId);

        if (!File.Exists(path))
        {
            currentSaveData = new GameSaveData();
            Debug.LogWarning($"저장 파일 없음: {petId} → 새로 생성");
            return;
        }

        string json = File.ReadAllText(path);
        currentSaveData = JsonUtility.FromJson<GameSaveData>(json);
        Debug.Log($"📂 로드 완료: {petId}");
    }

    public Vector3 GetPlayerPosition()
    {
        return new Vector3(currentSaveData.playerPosX, currentSaveData.playerPosY, currentSaveData.playerPosZ);
    }

    public void AddCompletedEvent(string eventId)
    {
        if (!currentSaveData.completedEventIds.Contains(eventId))
        {
            currentSaveData.completedEventIds.Add(eventId);
        }
    }

    public bool IsEventCompleted(string eventId)
    {
        return currentSaveData.completedEventIds.Contains(eventId);
    }

    public void SetCurrentEvent(string eventId)
    {
        currentSaveData.currentEventId = eventId;
    }

    public string GetCurrentEvent()
    {
        return currentSaveData.currentEventId;
    }
}