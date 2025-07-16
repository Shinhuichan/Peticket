using System;
using System.Collections.Generic;

[Serializable]
public class GameSaveData
{
    public string selectedPetId;
    public float playerPosX, playerPosY, playerPosZ;
    public List<string> completedEventIds = new();
    public string currentEventId;
    public AllPetData petData;
}