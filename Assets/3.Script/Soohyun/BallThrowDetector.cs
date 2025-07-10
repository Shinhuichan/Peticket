using UnityEngine;

public class BallThrowDetector : MonoBehaviour
{
    public AnimalLogic dog;
    public float throwVelocityThreshold = 1.5f;

    private Rigidbody rb;
    private bool hasThrown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (hasThrown || dog == null || rb == null)
            return;

        if (rb.velocity.magnitude >= throwVelocityThreshold)
        {
            hasThrown = true;
            dog.OnBallThrown(gameObject);
            Debug.Log("[BallThrowDetector] 공이 던져졌습니다!");
        }
    }
}
