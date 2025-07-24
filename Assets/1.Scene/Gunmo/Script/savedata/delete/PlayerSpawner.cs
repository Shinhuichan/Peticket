using CustomInspector;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField, ReadOnly] GameObject player;
    [SerializeField] Transform spawnTransform;

    private void Start()
    {
        Vector3 savedPos = GameSaveManager.Instance.GetPlayerPosition();
        if (savedPos != Vector3.zero)
            player.transform.position = savedPos;
        else
            player.transform.position = spawnTransform.position;

        Debug.Log($"📍 불러온 위치: {savedPos}");
    }
}