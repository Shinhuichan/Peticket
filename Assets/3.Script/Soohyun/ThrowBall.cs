using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    [Header("Throw Settings")]
    public GameObject ballPrefab;
    public Transform throwPoint;
    public float throwForce = 3f;

    [Header("Dog Target")]
    public AnimalLogic dog;

    [Header("Input")] //temporary Setting
    public KeyCode throwKey = KeyCode.Space;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(throwKey))
        {
            BallThrow();
        }
    }

    public void BallThrow()
    {
        if(ballPrefab == null || throwPoint == null || dog == null)
        {
            Debug.Log("�ʿ��� �� ����! �ν����� â ������");
            return;
        }

        GameObject ball = Instantiate(ballPrefab, throwPoint.position, Quaternion.identity);

        Rigidbody rb = ball.GetComponent<Rigidbody>();

        if(rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
        }
        dog.OnBallSpawned(ball);
    }
}
