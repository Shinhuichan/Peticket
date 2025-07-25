using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InventorySaveManager : MonoBehaviour
{
    public InventorySlot[] inventorySlots; // 인벤토리 슬롯 배열
    private string SavePath => Path.Combine(Application.dataPath, "SaveData/inventory_save.json");

    public void SaveInventory()
    {
        var saveData = new SavedInventoryData();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            var slot = inventorySlots[i];
            if (!slot.IsEmpty && slot.currentItem != null)
            {
                var itemId = slot.currentItem.name.Replace("(Clone)", "").Trim();
                saveData.items.Add(new SavedItemData
                {
                    itemName = itemId,
                    slotIndex = i
                });
            }
        }

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, json);
        Debug.Log("💾 인벤토리 저장 완료: " + SavePath);
    }

    public void LoadInventory()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("❗ 인벤토리 저장 파일이 존재하지 않습니다." + SavePath);
            return;
        }

        string json = File.ReadAllText(SavePath);
        var saveData = JsonUtility.FromJson<SavedInventoryData>(json);

        foreach (var slot in inventorySlots)
        {
            slot.ClearSlot();
        }

        foreach (var savedItem in saveData.items)
        {
            GameObject prefab = Resources.Load<GameObject>("Items/" + savedItem.itemName);
            if (prefab != null)
            {
                GameObject instance = Instantiate(prefab);
                inventorySlots[savedItem.slotIndex].StoreItem(instance);
            }
            else
            {
                Debug.LogWarning("❗ Resources/Items/ 에서 " + savedItem.itemName + " 프리팹을 찾을 수 없습니다.");
            }
        }

        Debug.Log("📥 인벤토리 불러오기 완료");
    }
    public void ResetInventory()
{
    // 슬롯 UI 비우기
    foreach (var slot in inventorySlots)
    {
        slot.ClearSlot();
    }

    // 빈 저장 데이터 작성
    var emptyData = new SavedInventoryData(); // items 리스트 비어있음

    string json = JsonUtility.ToJson(emptyData, true);
    File.WriteAllText(SavePath, json);

    Debug.Log("🗑 인벤토리 초기화 완료: 저장 데이터도 제거됨");
}
}
