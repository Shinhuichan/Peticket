using System.Collections.Generic;

[System.Serializable]
public class GameSaveData
{
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;

    public string selectedPetId;
    public string currentEventId;
    public List<string> completedEventIds = new List<string>();

    public AllPetData petData;
}