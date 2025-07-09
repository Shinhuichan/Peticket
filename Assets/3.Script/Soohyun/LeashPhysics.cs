using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LeashPhysics : MonoBehaviour
{
    public Transform leashAnchor;
    public float leashMaxLength = 3f;

    private LineRenderer line;
    private NavMeshAgent agent;

    public bool isLeashed;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        line = gameObject.AddComponent<LineRenderer>();
        line.positionCount = 2;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startColor = Color.cyan;
        line.endColor = Color.cyan;

        isLeashed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (leashAnchor == null) return;

        if (isLeashed == false)
        {
            line.enabled = false;
            return;
        }
        else
        {
            line.enabled = true;
        }

        Vector3 anchorPos = leashAnchor.position;
        Vector3 dogPos = transform.position;

        line.SetPosition(0, anchorPos);
        line.SetPosition(1, dogPos);

        float dist = Vector3.Distance(anchorPos, dogPos);

        if (dist > leashMaxLength * 0.95f)
        {
            line.startColor = Color.red;
            line.endColor = Color.gray;
        }

        else
        {
            line.startColor = Color.cyan;
            line.endColor = Color.cyan;
        }

        if(!agent.pathPending && agent.hasPath)
        {
            Vector3 desiredPos = agent.destination;
            float desiredDist = Vector3.Distance(anchorPos, desiredPos);

            if(desiredDist > leashMaxLength)
            {
                Vector3 dir = (desiredPos - anchorPos).normalized;
                Vector3 correctedPos = anchorPos + dir * leashMaxLength;

                agent.SetDestination(correctedPos);
                Debug.Log("목적지 재설정");
            }
        }
    }
}
