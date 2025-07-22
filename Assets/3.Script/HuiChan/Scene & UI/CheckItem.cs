using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public struct CollectableData
{
    public string itemName; // 획득해야 하는 아이템의 이름
    public Sprite itemSprite; // 아이템의 UI 이미지
}
public class CheckItem : MonoBehaviour
{
    [SerializeField] SceneChange sceneChange;

    void Start()
    {
        sceneChange = FindAnyObjectByType<SceneChange>();
    }
    public bool HasAllitem()
    {
        List<string> itemNames = new List<string>();
        foreach (var item in GameManager.Instance.needHasItem)
            itemNames.Add(item.itemName);
        
        // 필요한 모든 Item을 획득하였는가?
        bool itemAllIncluded = GameManager.Instance.currentHasItem.All(item => itemNames.Contains(item)); 
        // 필요한 모든 Item을 장착시켜줬는가?
        bool itemAllEquiped = GameManager.Instance.isCollarEquip && GameManager.Instance.isMuzzleEquip;
        
        bool allIncluded = itemAllIncluded && itemAllEquiped;
        return allIncluded;
    }
}