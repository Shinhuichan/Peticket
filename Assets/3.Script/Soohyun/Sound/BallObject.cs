using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


[RequireComponent(typeof(AudioSource), typeof(XRGrabInteractable))]
public class BallObject : MonoBehaviour
{
    public bool isFromInventory = false;
    public AnimalSoundSensor soundSensor;

    private XRGrabInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();
        interactable.selectExited.AddListener(OnReleased);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (isFromInventory && soundSensor != null)
        {
            if(GetComponent<Rigidbody>().velocity.magnitude > 0.5f)
            {
                GetComponent<AudioSource>()?.Play();
                soundSensor.NotifyBallThrown(gameObject);
            }
        }
    }
}
