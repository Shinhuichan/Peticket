using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform player;

    private void Start()
    {
        Vector3 savedPos = GameSaveManager.Instance.GetPlayerPosition();
        player.position = savedPos;

        Debug.Log($"📍 불러온 위치: {savedPos}");
    }
}
