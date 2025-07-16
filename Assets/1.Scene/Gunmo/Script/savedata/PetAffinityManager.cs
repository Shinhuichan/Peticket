using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class PetAffinityData
{
    public string petId;
    public float affinity;
}

[System.Serializable]
public class AllPetData
{
    public List<PetAffinityData> pets = new List<PetAffinityData>();
}

public class PetAffinityManager : MonoBehaviour
{
    private string filePath;
    private AllPetData currentData;

    private void Awake()
    {
        filePath = Path.Combine(Application.dataPath, "SaveData/pet_affinity.json");
        LoadAffinity();
    }

    public void UpdateAffinity(string petId, float amount)
    {
        var pet = currentData.pets.Find(p => p.petId == petId);
        if (pet != null)
        {
            pet.affinity = Mathf.Clamp(pet.affinity + amount, 0, 100);
        }
        else
        {
            currentData.pets.Add(new PetAffinityData
            {
                petId = petId,
                affinity = Mathf.Clamp(amount, 0, 100)
            });
        }
    }

    public float GetAffinity(string petId)
    {
        var pet = currentData.pets.Find(p => p.petId == petId);
        return pet != null ? pet.affinity : 0f;
    }

    public void SaveAffinity()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(filePath, json);
        Debug.Log("🔵 친밀도 저장 완료");
    }

    public void LoadAffinity()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            currentData = JsonUtility.FromJson<AllPetData>(json);
            Debug.Log("🟢 친밀도 불러오기 완료");
        }
        else
        {
            currentData = new AllPetData();
            Debug.LogWarning("⚠ 저장된 친밀도 데이터가 없습니다.");
        }
    }

    private void OnApplicationQuit()
    {
        SaveAffinity(); // 종료 시 자동 저장
    }
    // ✅ PetAffinityManager.cs에 추가할 함수
    public AllPetData GetCurrentData()
    {
        return currentData;
    }

    public void SetFromData(AllPetData data)
    {
        currentData = data;
    }
    //  이벤트용: 변화량 적용 후 자동 저장
    public void ChangeAffinityAndSave(string petId, float amount)
    {
        UpdateAffinity(petId, amount);  // 수치 변경
        SaveAffinity();                // 즉시 저장
    }

}
