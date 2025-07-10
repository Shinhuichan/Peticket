using UnityEngine;

public class BallInteraction : ObjectInteraction
{
    void Update()
    {
        dog.OnBallSpawned(gameObject);
    }
}