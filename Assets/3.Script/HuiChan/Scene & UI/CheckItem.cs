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
    public bool HasAllitem()
    {
        // 필요한 모든 Item을 획득하였는가?
        bool itemAllIncluded = GameManager.Instance.needHasItem.All(item => GameManager.Instance.currentHasItem.Contains(item));
        Debug.Log($"itemAllIncluded : {itemAllIncluded}");
        // 필요한 모든 Item을 장착시켜줬는가?
        bool itemAllEquiped = GameManager.Instance.isCollarEquip && GameManager.Instance.isMuzzleEquip;
        Debug.Log($"itemAllIncluded : {itemAllEquiped}");
        bool allIncluded = itemAllIncluded && itemAllEquiped;
        return allIncluded;
    }
}