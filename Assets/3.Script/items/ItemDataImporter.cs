using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ItemDataImporter : EditorWindow
{
    [MenuItem("Tools/Import Item CSV")]
    public static void ImportItemCSV()
    {
        string path = EditorUtility.OpenFilePanel("아이템 CSV 선택", "", "csv");
        if (string.IsNullOrEmpty(path)) return;

        string[] lines = File.ReadAllLines(path, Encoding.UTF8);

        // 저장 경로 (items_data 폴더 기준)
        string savePath = "Assets/3.Script/items_data/";
        if (!Directory.Exists(savePath))
            Directory.CreateDirectory(savePath);

        for (int i = 1; i < lines.Length; i++)
        {
            string[] parts = lines[i].Split(',');
            if (parts.Length < 4) continue;

            int id = int.Parse(parts[0]);
            string name = parts[1];
            string category = parts[2];
            string desc = parts[3];

            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.id = id;
            item.itemName = name;
            item.category = category;
            item.description = desc;
            item.prefab = null; // 프리팹은 수동 연결

            string assetName = $"Item_{id}_{name}.asset";
            AssetDatabase.CreateAsset(item, savePath + assetName);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("✅ 아이템 데이터 ScriptableObject 생성 완료");
    }
}
