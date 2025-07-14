using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSoundSensor : MonoBehaviour
{
    [SerializeField] private AnimalLogic animal;
    public void NotifyBallThrown(GameObject ball)
    {
        animal.OnBallSoundDetected(ball);
    }
}
