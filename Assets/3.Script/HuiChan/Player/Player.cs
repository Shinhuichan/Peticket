using UnityEngine;

public class Player : SingletonBehaviour<Player>
{
    protected override bool IsDontDestroy() => true;
    public Transform playerPosition;
}