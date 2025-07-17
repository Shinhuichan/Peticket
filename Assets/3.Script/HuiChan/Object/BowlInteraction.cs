using UnityEngine;

public class BowlInteraction : ObjectInteraction
{
    // [SerializeField] float angle = 30f;
    // [SerializeField] float force = 10f;
    // public override void UseObject()
    // {
    //     Rigidbody rb = GetComponent<Rigidbody>();
    //     if (rb == null) { return; }
    //     Vector3 forward = transform.forward.normalized;
    //     float rad = angle * Mathf.Deg2Rad;
    //     float horzForce = Mathf.Cos(rad) * force;
    //     float vertForce = Mathf.Sin(rad) * force;
    //     Vector3 init_vel = horzForce * forward + Vector3.up * vertForce;
    //     rb.AddForce(init_vel, ForceMode.VelocityChange);
    // }
}