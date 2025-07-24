using System.Collections.Generic;
using CustomInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : SingletonBehaviour<GameManager>
{
    protected override bool IsDontDestroy() => true;

    [Title("Room Scene Setting", underlined: true, fontSize = 18, alignment = TextAlignment.Center)]
    bool _b0;
    [Header("HasItem Setting")]
    public List<CollectableData> needHasItem = new List<CollectableData>();
    [ReadOnly] public List<string> currentHasItem = new List<string>();

    [Header("Dog Setting")]
    [ReadOnly] public bool isCollarEquip = false;
    [ReadOnly] public bool isMuzzleEquip = false;

    [Header("Player Setting")]
    [ReadOnly] public Player player;
    [ReadOnly] public CharacterController origin;
    [ReadOnly] public Camera mainCam;

    void Start()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        player = FindAnyObjectByType<Player>();
        origin = FindAnyObjectByType<CharacterController>();
        mainCam = Camera.main;
    }
    public int FindSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}