using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class AIMove : MonoBehaviour
{
    private Animator anim;
    private NavMeshAgent nav;

    private float RandomMoveTimer;
    private float CheckTime;

    private Vector3 rndPos;
    public float moveRadius = 5f;
    void Start()
    {
        RandomMoveTimer = Random.Range(3f, 5f);
        anim = GetComponent<Animator>();
        nav = GetComponent<NavMeshAgent>();
        SetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        CheckTime += Time.deltaTime;

        if(!nav.hasPath || nav.velocity.sqrMagnitude < 0.01f)
        {
            if (CheckTime >= RandomMoveTimer)
            {
                SetPosition();
                CheckTime = 0f;
                RandomMoveTimer = Random.Range(3f, 5f);
            }
        }
    }

    private void SetPosition()
    {
        rndPos = SetRandomPosition(transform.position, moveRadius);
        nav.SetDestination(rndPos);

        if (transform.position == rndPos)
        {
            anim.SetBool("IsWalking", false);
        }
        else
        {
            anim.SetBool("IsWalking", true);
        }
    }

    private Vector3 SetRandomPosition(Vector3 currentPos, float radius)
    {
        Vector3 rndDir = Random.insideUnitSphere * radius;
        rndDir += currentPos;
        if(NavMesh.SamplePosition(rndDir,out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return currentPos;
        }
    }
}
