using UnityEngine;
using System.IO;

public class GameSaveManager : MonoBehaviour
{
    public static GameSaveManager Instance { get; private set; }
    public GameSaveData currentSaveData;

    private string filePath;
    public event System.Action<float> OnProgressChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            filePath = Path.Combine(Application.dataPath, "SaveData/savelocation.json"); // 변경됨
            LoadGame(); // 게임 실행 시 자동 로드
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 게임 저장: Player 위치, 선택 펫 ID, 진행도 저장
    /// </summary>
    public void SaveGame(Vector3 playerPosition)
    {
        Debug.Log($"💾 저장 시도 위치: {playerPosition}");

        currentSaveData.playerPosX = playerPosition.x;
        currentSaveData.playerPosY = playerPosition.y;
        currentSaveData.playerPosZ = playerPosition.z;

        string json = JsonUtility.ToJson(currentSaveData, true);

        // 저장 경로 폴더 없으면 생성
        string folder = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        File.WriteAllText(filePath, json);
        Debug.Log($"💾 게임 저장 완료: {filePath}");
    }

    public void LoadGame()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            currentSaveData = JsonUtility.FromJson<GameSaveData>(json);
            Debug.Log("📂 게임 불러오기 완료");
        }
        else
        {
            currentSaveData = new GameSaveData();
            Debug.LogWarning("⚠ 저장 파일 없음 → 새 데이터 생성");
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return new Vector3(
            currentSaveData.playerPosX,
            currentSaveData.playerPosY,
            currentSaveData.playerPosZ
        );
    }

    public void SetSelectedPet(string petId)
    {
        currentSaveData.selectedPetId = petId;
        SaveGame(GetPlayerPosition());
    }

    public void SetPlayerProgress(float delta)
{
    currentSaveData.playerProgress = Mathf.Clamp(
        currentSaveData.playerProgress + delta,
        0f, 100f
    );

    SaveGame(GetPlayerPosition());

    Debug.Log($"📈 진행도 저장됨: {currentSaveData.playerProgress}%");

    OnProgressChanged?.Invoke(currentSaveData.playerProgress); // 🔔 이벤트 발생
}
}
