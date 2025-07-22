[System.Serializable]
public class GameSaveData
{
    public string selectedPetId;
    public float playerProgress;

    // ▶ 씬 이름별로 위치 저장
    public SerializableVector3 roomScenePosition = new SerializableVector3(0, 0, 0);
    public SerializableVector3 parkScenePosition = new SerializableVector3(0, 0, 0);
}
