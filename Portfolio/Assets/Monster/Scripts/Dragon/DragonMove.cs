using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DragonMove : MonoBehaviour
{
    [SerializeField]
    private float traceSpeed = 3.0f;
    [SerializeField]
    private float rotSpeed = 2.0f;

    private NavMeshAgent agent;

    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            agent.isStopped = false;
            agent.destination = value;
        }
    }

    public float speed
    {
        get { return agent.velocity.magnitude; }
    }

    private void Awake()
    {
        agent = GetComponentInChildren<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = 0.0f;
        agent.updateRotation = false;
    }

    private void Update()
    {
        if (agent.isStopped) return;

        Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rotSpeed * Time.deltaTime);
    }

    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
}
