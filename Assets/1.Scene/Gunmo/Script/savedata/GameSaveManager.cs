using UnityEngine;
using System.IO;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }

    private string filePath;
    public GameSaveData currentSaveData;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.dataPath, "SaveData/game_save.json");
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame(Vector3 playerPosition)
    {
        currentSaveData.playerPosX = playerPosition.x;
        currentSaveData.playerPosY = playerPosition.y;
        currentSaveData.playerPosZ = playerPosition.z;

        currentSaveData.petData = FindObjectOfType<PetAffinityManager>()?.GetCurrentData();

        string json = JsonUtility.ToJson(currentSaveData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("💾 게임 저장 완료");
    }

    public void LoadGame()
    {
        if (!File.Exists(filePath))
        {
            currentSaveData = new GameSaveData();
            Debug.LogWarning("저장 데이터 없음 → 새로 생성");
            return;
        }

        string json = File.ReadAllText(filePath);
        currentSaveData = JsonUtility.FromJson<GameSaveData>(json);
        Debug.Log("📂 게임 불러오기 완료");
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

