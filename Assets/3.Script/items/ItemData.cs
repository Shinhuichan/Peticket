using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public string category;
    [TextArea]
    public string description;
    public GameObject prefab;  // 나중에 연결할 프리팹
}