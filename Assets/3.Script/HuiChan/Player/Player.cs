using UnityEngine;

public class Player : SingletonBehaviour<Player>
{
    protected override bool IsDontDestroy() => false;
    public Transform playerPosition;
    public Transform itemPosition;
    public Transform petPosition;
}