using UnityEngine;
using CustomInspector;

public class CameraSetting : MonoBehaviour
{
    [ReadOnly, SerializeField] float playerStartYAxis;

    void Start()
    {
        playerStartYAxis = transform.position.y;
    }

    void LateUpdate()
    {
        PositionLock();
        RotationLock();
    }

    void PositionLock()
    {
        Vector3 currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, playerStartYAxis, currentPosition.z);

        // Debug.Log($"XR Origin Y: {transform.position.y}");
    }

    void RotationLock()
    {
        Vector3 currentEulerAngles = transform.eulerAngles;
        currentEulerAngles.x = 0;
        currentEulerAngles.z = 0;
        transform.eulerAngles = currentEulerAngles;
    }
}