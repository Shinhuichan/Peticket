using System.Collections.Generic;
using CustomInspector;
using UnityEngine;

public class GameManager : SingletonBehaviour<GameManager>
{
    protected override bool IsDontDestroy() => true;

    [Title("Room Scene Setting", underlined: true, fontSize = 18, alignment = TextAlignment.Center)]

    [Header("HasItem Setting")]
    public List<CollectableData> needHasItem = new List<CollectableData>();
    [ReadOnly] public List<string> currentHasItem = new List<string>();

    [Title("Always Setting", underlined: true, fontSize = 18, alignment = TextAlignment.Center)]
    [Header("Dog Setting")]
    [ReadOnly] public bool isCollarEquip = false;
}
