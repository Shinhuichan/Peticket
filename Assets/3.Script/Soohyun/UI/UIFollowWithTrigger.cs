using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class UIFollowWithTrigger : MonoBehaviour
{
    [Header("Camera Follow")]
    public Transform targetCam;
    public float baseDistance;
    public float minSafeDistance;
    public float followSpeed;
    public Vector3 positionOffset = Vector3.zero;

    private float currentDistance;
    private int obstacleCount = 0;
    private bool isOverlapping = false;
    void Start()
    {
        currentDistance = baseDistance;
        GetComponent<Collider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetCam == null) return;

        Vector3 dir = targetCam.forward;

        isOverlapping = obstacleCount > 0;
        float targetDistance = isOverlapping ? minSafeDistance : baseDistance;

        currentDistance = Mathf.Lerp(currentDistance, targetDistance, Time.deltaTime * followSpeed);

        Vector3 targetPos = targetCam.position + dir * currentDistance + positionOffset;
        transform.position = targetPos;

        Vector3 euler = targetCam.rotation.eulerAngles;
        euler.z = 0f;
        Quaternion targetRot = Quaternion.Euler(euler);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * followSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer== LayerMask.NameToLayer("Obstacle")) 
        {
            obstacleCount++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            // 잠시 후에도 장애물이 여전히 존재하면 감소하지 않음
            if (!IsStillOverlappingObstacle())
            {
                obstacleCount = Mathf.Max(0, obstacleCount - 1);
            }
        }
    }


    private bool IsStillOverlappingObstacle()
    {
        BoxCollider box = GetComponent<BoxCollider>();
        Vector3 worldCenter = transform.TransformPoint(box.center);
        Vector3 worldSize = Vector3.Scale(box.size, transform.lossyScale) * 0.5f;
        Collider[] hits = Physics.OverlapBox(worldCenter, worldSize, transform.rotation, LayerMask.GetMask("Obstacle"));
        return hits.Length > 0;
    }

}
