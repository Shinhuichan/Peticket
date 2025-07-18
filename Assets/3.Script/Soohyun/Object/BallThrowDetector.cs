using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class BallThrowDetector : MonoBehaviour
{
    public AnimalLogic dog;
    public float throwVelocityThreshold = 1.5f;
    public float throwDistanceThreshold = 2f;

    private Rigidbody rb;
    private Transform player;
    private bool hasThrown = false;
    private bool isHeld = true;
    private XRGrabInteractable interactable;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        interactable = GetComponent<XRGrabInteractable>();
        interactable.selectEntered.AddListener(args => 
        {
            isHeld = true;
            hasThrown = false;

            if(dog != null && player == null)
            {
                player = dog.Player;
            }
        });
        interactable.selectExited.AddListener(args =>
        {
            isHeld = false;
        });
    }

    private void Update()
    {
        if (hasThrown || player == null)
            return;

        float velocity = rb.velocity.magnitude;
        float distance = Vector3.Distance(transform.position, player.position);

        if (velocity > throwVelocityThreshold || distance > throwDistanceThreshold)
        {
            hasThrown = true;

            dog.OnBallSoundDetected(gameObject);

            Debug.Log("°ø ´øÁü");
        }
    }
}
