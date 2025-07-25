using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public enum CollectableData
{
    None = 0,
    Food,
    Ball,
    Shovel,
    Bowl,
    Muzzle,
    Collar
}
public class CheckItem : MonoBehaviour
{
    [SerializeField] SceneChange sceneChange;

    void Start()
    {
        // sceneChange = FindAnyObjectByType<SceneChange>();
    }
    public bool HasAllitem()
    {
        List<string> itemNames = new List<string>();
        foreach (var item in GameManager.Instance.needHasItem)
            itemNames.Add(item.ToString());
        
        // 필요한 모든 Item을 획득하였는가?
        bool itemAllIncluded = itemNames.All(item => GameManager.Instance.currentHasItem.Contains(item));
        Debug.Log($"itemAllIncluded : {itemAllIncluded}");
        // 필요한 모든 Item을 장착시켜줬는가?
        bool itemAllEquiped = GameManager.Instance.isCollarEquip && GameManager.Instance.isMuzzleEquip;
        Debug.Log($"itemAllEquiped : {itemAllEquiped}");
        bool allIncluded = itemAllIncluded && itemAllEquiped;
        return allIncluded;
    }
}