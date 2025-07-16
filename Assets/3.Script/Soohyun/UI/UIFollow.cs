using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFollow : MonoBehaviour
{
    public Transform targetCamera;
    public Vector3 offset = new Vector3(0, 0, 2f);
    public float followSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (targetCamera == null) return;

        Vector3 targetPos = targetCamera.position + targetCamera.forward * offset.z +
            targetCamera.right * offset.x +
            targetCamera.up * offset.y;

        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(transform.position - targetCamera.position), Time.deltaTime * followSpeed);
    }
}
