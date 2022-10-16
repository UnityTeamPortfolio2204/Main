using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterMove: MonoBehaviour
{
    [SerializeField]
    protected float patrolSpeed;
    [SerializeField]
    protected float patrolRange;
    [SerializeField]
    protected float traceSpeed;
    [SerializeField]
    protected float rotSpeed;

    protected NavMeshAgent agent;

    protected Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
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

    virtual protected void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.speed = 0.0f;
        agent.updateRotation = false;
    }

    virtual protected void Update()
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
