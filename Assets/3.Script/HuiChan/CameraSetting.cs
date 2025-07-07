using UnityEngine;
using CustomInspector;

public class CameraSetting : MonoBehaviour
{
    [ReadOnly, SerializeField] Camera cam;
    [ReadOnly, SerializeField] float playerStartYAxis;

    void Start()
    {
        cam = Camera.main;
        playerStartYAxis = cam.transform.position.y;
    }
    void LateUpdate()
    {
        PositionLock();
        RotationLock();
    }

    void PositionLock()
    {
        if (cam == null) return;
        cam.transform.position = new Vector3(cam.transform.position.x, playerStartYAxis, cam.transform.position.z);
        transform.position = cam.transform.position;
    }
    void RotationLock()
    {
        if (cam == null) return;

        Vector3 currentEulerAngles = cam.transform.eulerAngles;

        currentEulerAngles.x = 0;
        currentEulerAngles.z = 0;

        cam.transform.eulerAngles = currentEulerAngles;
    }
}
